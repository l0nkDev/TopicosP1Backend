using CareerApi.Models;

namespace TopicosP1Backend.Scripts
{
    public class Transaction
    {
        required public string Id { get; set; }
        required public Action Action { get; set; }
    }

    public class APIQueue
    {
        private readonly IServiceScopeFactory scopeFactory;
        private Context? context;
        private readonly Thread? Thread = null;
        private Queue<Transaction> q = [];
        private Dictionary<string, object> responses = [];

        public APIQueue(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
            Thread = new(BackgroundProcess);
            Thread.Start();
        }

        public void BackgroundProcess()
        {
            IServiceScope? scope = null;
            {
                while (true)
                {
                    if (q.Count() != 0)
                    {
                        if (scope == null)
                        {
                            scope = scopeFactory.CreateScope();
                            context = scope.ServiceProvider.GetService<Context>();
                        }
                        Transaction i = q.Dequeue();
                        Action a = i.Action;
                        a();
                    }
                    else { scope?.Dispose(); scope = null; Thread.Yield(); }
                }
            }
        }
        public void Add(string id, Action action) { q.Enqueue(new Transaction() { Id = id, Action = action }); }
        public void AddResponse(string id, object obj) => responses.Add(id, obj);
        public object Get(string id) { object obj = responses[id]; responses.Remove(id); return obj; }
        public void GetStudyPlan(string id, string tranid)
        {
            var studyPlan = context.StudyPlans.Include(_ => _.Career).Include(_ => _.Subjects).ThenInclude(_ => _.Prerequisites).FirstOrDefaultAsync(i => i.Code == id);
            if (studyPlan == null) return;
            StudyPlan.StudyPlanDTO res = new(studyPlan.Result);
            AddResponse(tranid, res);
        }
    }
}
