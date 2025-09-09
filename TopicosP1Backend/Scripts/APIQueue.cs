using TopicosP1Backend.Cache;

namespace TopicosP1Backend.Scripts
{
    public class APIQueue
    {
        private readonly IServiceScopeFactory scopeFactory;
        private List<Queue<QueuedFunction>> queues = [new()];
        private HashSet<string> queued = new HashSet<string>();
        private Dictionary<string, object> responses = [];

        public APIQueue(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
            IServiceScope? scope = scopeFactory.CreateScope();
            var cache = scope.ServiceProvider.GetService<CacheContext>();
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
                queued.Add(action.Hash);
                Emptier().Enqueue(action);  
            }
        }
        public void AddResponse(string id, object obj) 
        { 
            responses.Add(id, obj); 
            queued.Remove(id); 
        }

        public object Get(string id, bool delete) 
        { 
            object obj = responses[id]; 
            if (delete) responses.Remove(id); 
            return obj; 
        }

        public string? IsQueued(string id) => queued.Contains(id) ? id : null;

        public int Count() 
        { 
            int c = 0; 
            foreach (var q in queues) c += q.Count; 
            return c; 
        }

        public QueuedFunction? Dequeue() 
        { 
            try 
            { 
                return Fuller().Dequeue(); 
            } 
            catch 
            { 
                return null; 
            }
        }

        public Queue<QueuedFunction>? Emptier()
        {
            if (queues.Count == 0) return null;
            Queue<QueuedFunction> res = queues[0];
            int c = queues[0].Count;
            foreach (var q in queues) if (q.Count < c)
                {
                    res = q;
                    c = q.Count;
                }
            return res;
        }

        public Queue<QueuedFunction>? Fuller()
        {
            if (queues.Count == 0) return null;
            Queue<QueuedFunction> res = queues[0];
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
            if (IsQueued(tranid) != null) return tranid;
            try { return Get(tranid, delete); } catch { Console.WriteLine("Failed!"); }
            Add(new QueuedFunction()
            { Queue = queues.IndexOf(Emptier()) ,Function = function, ItemIds = itemIds, Hash = tranid, Body = body });
            return tranid;
        }

        public List<Queue<QueuedFunction>> GetQueues() => queues;
        public Queue<QueuedFunction> GetQueue(int id) => queues[id];
        public void SetQueuesCount(int n)
        {
            if (queues.Count == n) return;
            if (queues.Count > n) while (queues.Count > n) queues.Remove(Emptier());
            if (queues.Count < n) while (queues.Count < n) queues.Add(new());
        }

        public object getTranStatus(string id)
        {
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
            return new { Status = "Processing...", Item = "Out of Queue. Item is in a function and its information will be unavailable until it finishes processing." };
        }
    }
}
