using CareerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using System.Text.Json;
using System.Threading;
using TopicosP1Backend.Cache;

namespace TopicosP1Backend.Scripts
{

    public class QueueWorker : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly APIQueue _queue;

        public QueueWorker(IServiceScopeFactory scopeFactory, APIQueue queue)
        {
            this.scopeFactory = scopeFactory;
            _queue = queue;
            IServiceScope? scope = scopeFactory.CreateScope();
            var cache = scope.ServiceProvider.GetService<CacheContext>();
            List<QueuedFunction.DBItem> saved = cache.QueuedFunctions.ToList();
            foreach (QueuedFunction.DBItem item in saved) _queue.Enqueue(item.ToQueueItem());
            scope?.Dispose(); scope = null; cache = null;
            ExecuteAsync(new CancellationToken());
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                IServiceScope? scope = null;
                Context? context = null;
                CacheContext? cache = null;
                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine("Running!");
                    if (_queue.Count() != 0)
                    {
                        if (scope == null)
                        {
                            scope = scopeFactory.CreateScope();
                            context = scope.ServiceProvider.GetService<Context>();
                            cache = scope.ServiceProvider.GetService<CacheContext>();
                        }
                        QueuedFunction a = _queue.Dequeue();
                        var exiting = cache.QueuedFunctions.First();
                        object res = await a.Execute(context);
                        cache.QueuedFunctions.Remove(exiting);
                        cache.SaveChanges();
                        _queue.AddResponse(a.Hash, res);
                    }
                    else { scope?.Dispose(); scope = null; await Task.Delay(1000, stoppingToken); }
                }
            });
        }
    }
}
