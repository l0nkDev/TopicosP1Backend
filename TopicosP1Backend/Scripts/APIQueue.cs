using TopicosP1Backend.Cache;
using Microsoft.OpenApi.Extensions;
using System.Collections.Concurrent;

namespace TopicosP1Backend.Scripts
{

    public class Qcount
    {
        public long Id { get; set; }

        public List<int> Endpoints { get; set; } = [-1];
    }

    public class CustomQueue: ConcurrentQueue<QueuedFunction>
    {
        public List<int> Endpoints { get; set; } = [-1];
    }
    public class APIQueue
    {
        private readonly IServiceScopeFactory scopeFactory;
        private List<CustomQueue> queues = [];
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
            List<Qcount> qcs = cache.qcounts.ToList();
            foreach (Qcount qc in qcs) queues.Add(new() { Endpoints = qc.Endpoints });
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
                CustomQueue emptier = queues[action.Queue];
                emptier.Enqueue(action);
            }
        }

        public void AddQueue(List<int> Endpoints)
        {
            if (Endpoints == null) Endpoints = [-1];
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                CacheContext _context = scope.ServiceProvider.GetService<CacheContext>();
                _context.qcounts.Add(new() { Endpoints = Endpoints});
                _context.SaveChanges();
                queues.Add(new() { Endpoints = Endpoints });
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

        public CustomQueue Dequeue(int take, int q)
        {
            CustomQueue deq = [];
            while (take > 0)
            {
                QueuedFunction deqr;
                bool taken;
                if (q == 0)
                    taken = Emptier().TryDequeue(out deqr);
                else
                    taken = queues[q - 1].TryDequeue(out deqr);
                if (taken) deq.Enqueue(deqr);
                else break;
                take--;
            }
            return deq;
        }

        public CustomQueue? Emptier(int function = -1)
        {
            if (queues.Count == 0) return null;
            CustomQueue? res = null;
            int? c = null;
            foreach (var q in queues)
                if (c == null || q.Count < c)
                    if (function == -1 || q.Endpoints.Contains(function))
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
            { Queue = queues.IndexOf(Emptier((int)function)), Function = function, ItemIds = itemIds, Hash = tranid, Body = body };
            if (qf.Queue == -1) return "No queue available for this function.";
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

        public List<CustomQueue> GetQueues() => queues;
        public CustomQueue GetQueue(int id) => queues[id-1];
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
