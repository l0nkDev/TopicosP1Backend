using TopicosP1Backend.Cache;

namespace TopicosP1Backend.Scripts
{
    public class wcount
    {
        public long Id { get; set; }
        required public int Count { get; set; }
    }

    public class WorkerManager: IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly APIQueue _queue;
        List<QueueWorker> _workers = [];
        public WorkerManager(IServiceScopeFactory scope, APIQueue queue)
        {
            _queue = queue;
            _scopeFactory = scope;
            CacheContext c = scope.CreateScope().ServiceProvider.GetService<CacheContext>();
            wcount wc = c.wcounts.FirstOrDefault();
            if (wc == null) { wc = new() { Count = 1 }; c.wcounts.Add(wc); c.SaveChanges(); }
            SetCountTo(wc.Count);
        }
        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public object GetWorkers() => (from i in _workers select new { Worker = _workers.IndexOf(i), i.Status });
        public object GetWorker(int id) => new { Worker = id, _workers[id].Status };
        
        public void SetCountTo(int n)
        {
            if (n == _workers.Count) return;
            CacheContext c = _scopeFactory.CreateScope().ServiceProvider.GetService<CacheContext>();
            wcount wc = c.wcounts.First();
            wc.Count = n;
            c.Entry(wc).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            c.SaveChanges();
            if (n > _workers.Count) while (n > _workers.Count) _workers.Add(new(_scopeFactory, _queue));
            if (n < _workers.Count) while (n < _workers.Count) { var w = _workers.Last(); _workers.Remove(w); w.Stop(); w.Dispose(); }
        }

        public void Stop(int n) => _workers[n].Stop();
        public void Start(int n) => _workers[n].Start();
    }
}
