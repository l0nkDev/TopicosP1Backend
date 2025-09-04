using CareerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using System.Text.Json;
using TopicosP1Backend.Cache;

namespace TopicosP1Backend.Scripts
{

    public class APIQueue
    {
        private readonly IServiceScopeFactory scopeFactory;
        private Context? context;
        private CacheContext? cache;
        private readonly Thread? Thread = null;
        private Queue<QueuedFunction> q = [];
        private HashSet<int> queued = new HashSet<int>();
        private Dictionary<int, object> responses = [];

        public APIQueue(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
            IServiceScope? scope = scopeFactory.CreateScope();
            cache = scope.ServiceProvider.GetService<CacheContext>();
            List<QueuedFunction.DBItem> saved = cache.QueuedFunctions.ToList();
            foreach (QueuedFunction.DBItem item in saved) q.Enqueue(item.ToQueueItem());
            scope?.Dispose(); scope = null; cache = null;
            Thread = new(BackgroundProcess);
            Thread.Start();
        }

        public void BackgroundProcess()
        {
            {
                IServiceScope? scope = null;
                while (true)
                {
                    if (q.Count != 0)
                    {
                        if (scope == null )
                        {
                            scope = scopeFactory.CreateScope();
                            context = scope.ServiceProvider.GetService<Context>();
                            cache = scope.ServiceProvider.GetService<CacheContext>();
                        }
                        QueuedFunction a = q.Dequeue();
                        object res = a.Execute(context);
                        cache.QueuedFunctions.Remove(cache.QueuedFunctions.First());
                        cache.SaveChanges();
                        AddResponse(a.Hash, res);

                    }
                    else { scope?.Dispose(); scope = null; Thread.Yield(); }
                }
            }
        }
        public void Add(QueuedFunction action) 
        {
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                CacheContext _context = scope.ServiceProvider.GetService<CacheContext>();
                _context.QueuedFunctions.Add(action.ToDBItem());
                _context.SaveChanges();
                q.Enqueue(action);
            }
        }
        public void AddResponse(int id, object obj) { responses.Add(id, obj); queued.Remove(id); }
        public object Get(int id, bool delete) { object obj = responses[id]; if (delete) responses.Remove(id); return obj; }
        public int? IsQueued(int id) => queued.Contains(id) ? id : null;
    }
}
