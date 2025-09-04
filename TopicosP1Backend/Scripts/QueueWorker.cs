using TopicosP1Backend.Cache;

namespace TopicosP1Backend.Scripts
{

    public class QueueWorker : BackgroundService, IQueueWorkerStopper
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly APIQueue _queue;
        private bool running = true;

        public QueueWorker(IServiceScopeFactory scopeFactory, APIQueue aPIQueue)
        {
            this.scopeFactory = scopeFactory;
            _queue = aPIQueue;
            ExecuteAsync(default);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                IServiceScope? scope = null;
                Context? context = null;
                CacheContext? cache = null;
                QueuedFunction? a = null;
                while (running && !stoppingToken.IsCancellationRequested)
                {
                    if (_queue.Count() != 0 && (a = _queue.Dequeue()) != null)
                    {
                        Console.WriteLine($"Running Worker {this.GetHashCode()}");
                        if (scope == null)
                        {
                            scope = scopeFactory.CreateScope();
                            context = scope.ServiceProvider.GetService<Context>();
                            cache = scope.ServiceProvider.GetService<CacheContext>();
                        }
                        var exiting = cache.QueuedFunctions.First();
                        object res = await a.Execute(context);
                        cache.QueuedFunctions.Remove(exiting);
                        cache.SaveChanges();
                        _queue.AddResponse(a.Hash, res);
                    }
                    else { scope?.Dispose(); scope = null; await Task.Delay(1000, stoppingToken); }
                }
                Console.WriteLine($"Idling Worker {this.GetHashCode()}");
                scope?.Dispose(); scope = null; await Task.Delay(1000, stoppingToken);
            });
        }
        void IQueueWorkerStopper.StopAsync() { running = false; Console.WriteLine($"Stopping Worker {this.GetHashCode()}"); return; }
        void IQueueWorkerStopper.StartAsync() { running = true; ExecuteAsync(default); Console.WriteLine($"Starting Worker {this.GetHashCode()}"); return; }
    }
}
