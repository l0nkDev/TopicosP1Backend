using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using TopicosP1Backend.Cache;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using System.Collections.Concurrent;
using System.Linq;

namespace TopicosP1Backend.Scripts
{
    public class qcount
    {
        public long Id { get; set; }
        required public int Count { get; set; }
    }
    public class APIQueue
    {
        private readonly IServiceScopeFactory scopeFactory;
        private List<ConcurrentQueue<QueuedFunction>> queues = [new()];
        private ConcurrentDictionary<string, byte> queued = [];
        private ConcurrentDictionary<string, object> responses = [];
        public ConcurrentDictionary<string, int> thingsdone = [];
        public ConcurrentDictionary<string, int> thingsreceived = [];
        public bool isasync = true;

        public APIQueue(IServiceScopeFactory scopeFactory)
        {
            thingsdone.TryAdd("Total", 0);
            thingsreceived.TryAdd("Total", 0);
            this.scopeFactory = scopeFactory;
            IServiceScope? scope = scopeFactory.CreateScope();
            CacheContext cache = scope.ServiceProvider.GetService<CacheContext>();
            qcount qc = cache.qcounts.FirstOrDefault();
            if (qc == null) { qc = new() { Count = 1 }; cache.qcounts.Add(qc); cache.SaveChanges(); }
            SetQueuesCount(qc.Count);
            List<QueuedFunction.DBItem> saved = cache.QueuedFunctions.ToList();
            foreach (QueuedFunction.DBItem item in saved) queues[item.Queue].Enqueue(item.ToQueueItem());
            scope?.Dispose(); scope = null; cache = null;
        }

        public void Add(QueuedFunction action)
        {
                using (IServiceScope scope = scopeFactory.CreateScope())
                {
                    CacheContext _context = scope.ServiceProvider.GetService<CacheContext>();
                    _context.QueuedFunctions.Add(action.ToDBItem());
                    _context.SaveChanges();
                    queued.TryAdd(action.Hash, 0);
                    Emptier().Enqueue(action);
                }
        }
        public void AddResponse(string id, object obj) 
        { 
            responses.TryAdd(id, obj);
            queued.Remove(id, out _);
        }

        public object Get(string id, bool delete) 
        { 
            object obj = responses[id]; 
            if (delete) responses.Remove(id, out _);
            if (responses.Count <= 0) responses = new(); 
            return obj; 
        }

        public string? IsQueued(string id) => queued.ContainsKey(id) ? id : null;

        public int Count() 
        { 
            int c = 0; 
            foreach (var q in queues) c += q.Count; 
            return c; 
        }

        public QueuedFunction? Dequeue() 
        {
            if (Fuller().TryDequeue(out QueuedFunction deqr)) return deqr;
            else return null;
        }

        public ConcurrentQueue<QueuedFunction>? Emptier()
        {
            if (queues.Count == 0) return null;
            ConcurrentQueue<QueuedFunction> res = queues[0];
            int c = queues[0].Count;
            foreach (var q in queues) if (q.Count < c)
                {
                    res = q;
                    c = q.Count;
                }
            return res;
        }

        public ConcurrentQueue<QueuedFunction>? Fuller()
        {
            if (queues.Count == 0) return null;
            ConcurrentQueue<QueuedFunction> res = queues[0];
            int c = queues[0].Count;
            foreach (var q in queues) if (q.Count > c)
                {
                    res = q;
                    c = q.Count;
                }
            return res;
        }

        public object Request(Function function, List<string> itemIds, string body, string hashtarget, bool delete = false)
        {
            string tranid = Util.Hash(hashtarget);
            QueuedFunction qf = new QueuedFunction()
            { Queue = queues.IndexOf(Emptier()), Function = function, ItemIds = itemIds, Hash = tranid, Body = body };
            string dn = function.GetDisplayName();
            thingsreceived.AddOrUpdate(dn, 1, (key, oldValue) => oldValue + 1);


            if (!isasync)
                using (IServiceScope scope = scopeFactory.CreateScope())
                    return qf.Execute(scope.ServiceProvider.GetService<Context>());


            if (IsQueued(tranid) != null) return tranid;
            try { return Get(tranid, delete); } catch { }
            if (queues.Count == 0) return "No queues available.";
            Add(qf);
            return tranid;
        }

        public List<ConcurrentQueue<QueuedFunction>> GetQueues() => queues;
        public ConcurrentQueue<QueuedFunction> GetQueue(int id) => queues[id];
        public void SetQueuesCount(int n)
        {
            if (queues.Count == n) return;
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                CacheContext _context = scope.ServiceProvider.GetService<CacheContext>();
                qcount qc = _context.qcounts.First();
                qc.Count = n;
                _context.Entry(qc).State = EntityState.Modified;
                _context.SaveChanges();
                if (queues.Count > n)
                if (n == 0)
                {
                    queues = new List<ConcurrentQueue<QueuedFunction>>();
                    _context.QueuedFunctions.ExecuteDelete();
                    queued = new ConcurrentDictionary<string, byte>();
                }
                while (queues.Count > n)
                {
                    ConcurrentQueue<QueuedFunction> q = Emptier();
                    queues.Remove(q);
                    while (q.Count > 0)
                    {
                        int i = queues.IndexOf(Emptier());
                        q.TryDequeue(out QueuedFunction item);
                        item.Queue = i;
                        queues[i].Enqueue(item);
                        QueuedFunction.DBItem dbi = _context.QueuedFunctions.FirstOrDefault(_=>_.Hash == item.Hash);
                        dbi.Queue = i;
                        _context.Entry(dbi).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }
            }
            if (queues.Count < n) while (queues.Count < n) queues.Add(new());
        }

        public object getTranStatus(string id)
        {
            try 
            {
                dynamic r = responses[id];
                object value = r.Value;
                if (value != null) return new { Status = "Done", Result = value };
                int status = r.Result.StatusCode;
                switch (status)
                {
                    case 404: return new { Status = "Done", Result = "404 Not Found" };
                    case 400: return new { Status = "Done", Result = "400 Bad Request" };
                }
                return new { Status = "Done", Result = r };
            } catch { }
            try { return new { Status = "Done", Result = responses[id] }; } catch { }
            bool isInQueue = false;
            QueuedFunction? tmp = null;
            foreach (var q in queues)
            {
                tmp = q.ToList().FirstOrDefault(_ => _.Hash == id);
                if (tmp != null)
                {
                    isInQueue = true;
                    break;
                }
            }
            if (isInQueue && tmp != null) return new { Status = "Queued", Item = tmp};
            if (queued.ContainsKey(id)) return new { Status = "Processing...", Item = "Out of Queue. Item is in a function and its information will be unavailable until it finishes processing." };
            return new { Status = "Not found", Item = "Item is not queued and no result has been saved. ID may be wrong or the result may have already expired." };
        }
    }
}
