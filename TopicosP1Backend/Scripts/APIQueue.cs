using TopicosP1Backend.Cache;

namespace TopicosP1Backend.Scripts
{

    public class APIQueue
    {
        private readonly IServiceScopeFactory scopeFactory;
        private Queue<QueuedFunction> q = [];
        private HashSet<string> queued = new HashSet<string>();
        private Dictionary<string, object> responses = [];

        public APIQueue(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
            IServiceScope? scope = scopeFactory.CreateScope();
            var cache = scope.ServiceProvider.GetService<CacheContext>();
            List<QueuedFunction.DBItem> saved = cache.QueuedFunctions.ToList();
            foreach (QueuedFunction.DBItem item in saved) q.Enqueue(item.ToQueueItem());
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
                q.Enqueue(action);  
            }
        }
        public void AddResponse(string id, object obj) { responses.Add(id, obj); queued.Remove(id); }
        public object Get(string id, bool delete) { object obj = responses[id]; if (delete) responses.Remove(id); return obj; }
        public string? IsQueued(string id) => queued.Contains(id) ? id : null;
        public int Count() => q.Count;
        public QueuedFunction? Dequeue() { try { return q.Dequeue(); } catch { return null; } }
        public void Enqueue(QueuedFunction func) => q.Enqueue(func);
        public object Request(Function function, List<string> itemIds, string body, string hashtarget, bool delete = false)
        {
            string tranid = Util.Hash(hashtarget);
            if (IsQueued(tranid) != null) return tranid;
            try { return Get(tranid, delete); } catch { Console.WriteLine("Failed!"); }
            Add(new QueuedFunction()
            { Function = function, ItemIds = itemIds, Hash = tranid, Body = body });
            return tranid;
        }
    }
}
