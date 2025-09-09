using TopicosP1Backend.Cache;

namespace TopicosP1Backend.Scripts
{

    public class WorkerManager: IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly APIQueue _queue;
        List<QueueWorker> _workers = [];
        public WorkerManager(IServiceScopeFactory scope, APIQueue queue)
        {
            _queue = queue;
            _scopeFactory = scope;
            //_workers.Add(new(_scopeFactory, _queue));
        }
        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public object GetWorkers() => (from i in _workers select new { Worker = _workers.IndexOf(i), i.Status });
        public object GetWorker(int id) => new { Worker = id, _workers[id].Status };
        
        public void SetCountTo(int n)
        {
            if (n == _workers.Count) return;
            if (n > _workers.Count) while (n > _workers.Count) _workers.Add(new(_scopeFactory, _queue));
            if (n < _workers.Count) while (n < _workers.Count) { var w = _workers.Last(); _workers.Remove(w); w.Stop(); w.Dispose(); }
        }

        public void Stop(int n) => _workers[n].Stop();
        public void Start(int n) => _workers[n].Start();
    }
}
