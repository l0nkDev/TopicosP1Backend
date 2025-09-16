using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using TopicosP1Backend.Cache;

namespace TopicosP1Backend.Scripts
{

    public class QueueWorker : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly APIQueue _queue;
        public string Status = "";
        private CancellationTokenSource cts = new();

        public QueueWorker(IServiceScopeFactory scopeFactory, APIQueue aPIQueue)
        {
            this.scopeFactory = scopeFactory;
            _queue = aPIQueue;
            Start();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool usecache = _queue.isasync;
            await Task.Run(async () =>
            {
                IServiceScope? scope = null;
                Context? context = null;
                CacheContext? cache = null;
                QueuedFunction? a = null;
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (_queue.Count() != 0 && (a = _queue.Dequeue()) != null)
                    {
                        Console.WriteLine($"Running Worker {this.GetHashCode()}");
                        Status = "Running";
                        if (scope == null)
                        {
                            scope = scopeFactory.CreateScope();
                            context = scope.ServiceProvider.GetService<Context>();
                            if (usecache) cache = scope.ServiceProvider.GetService<CacheContext>();
                        }
                        QueuedFunction.DBItem? exiting = null;
                        if (usecache) exiting = await cache.QueuedFunctions.FirstOrDefaultAsync(_=>_.Hash == a.Hash);
                        object res = await a.Execute(context);
                        if (exiting != null) cache.QueuedFunctions.Remove(exiting);
                        if (usecache) cache.SaveChanges();
                        _queue.AddResponse(a.Hash, res);
                        string dn = a.Function.GetDisplayName();
                        _queue.thingsdone.AddOrUpdate(dn, 1, (key, oldValue) => oldValue + 1);
                        await Task.Yield();
                    }
                    else { scope?.Dispose(); scope = null; Status = "Idle"; Console.WriteLine($"Idling Worker {this.GetHashCode()}"); await Task.Delay(1000, stoppingToken); }
                }
                scope?.Dispose(); scope = null; await Task.Delay(1000, stoppingToken);
            });
        }
        public async void Stop()
        {
            if (cts == null) return; cts.Cancel(); await StopAsync(cts.Token); cts.Dispose();  cts = null; Status = "Stopped";
        }
        public async void Start()
        {
            cts = new(); await StartAsync(cts.Token);
        }
    }
}
