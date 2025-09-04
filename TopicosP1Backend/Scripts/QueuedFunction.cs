using CareerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TopicosP1Backend.Scripts
{
    public enum Function
    {
        GetStudyPlan, GetStudyPlans,
        GetStudentHistory, GetStudentAvailable,
        GetGestion, GetGestions, PostGestion,
        PostPeriod, GetPeriods, GetPeriod,
        GetSubject, GetSubjects, PostSubject
    }

    public class QueuedFunction
    {
        required public string Hash { get; set; }
        required public Function Function { get; set; }
        required public List<string> ItemIds { get; set; }
        required public string Body { get; set; }
        public DBItem ToDBItem() => new(this);
        public class DBItem
        {
            public long Id { get; set; }
            public string Hash { get; set; }
            public int Function { get; set; }
            public string ItemIds { get; set; }
            public string Body { get; set; }
            public QueuedFunction ToQueueItem() => new()
            {
                Hash = Hash,
                Function = (Function)Function,
                ItemIds = JsonSerializer.Deserialize<List<string>>(ItemIds),
                Body = Body
            };

            public DBItem(QueuedFunction qf)
            {
                Hash = qf.Hash;
                Function = (int)qf.Function;
                ItemIds = JsonSerializer.Serialize(qf.ItemIds);
                Body = qf.Body;
            }

            [JsonConstructor]
            public DBItem(string Hash, int Function, string ItemIds, string Body)
            {
                this.Hash = Hash;
                this.Function = Function;
                this.ItemIds = ItemIds;
                this.Body = Body;
            }
        }

        public async Task<object?> Execute(Context context)
        {
            switch (Function)
            {
                case Function.GetStudyPlan: return await StudyPlan.Get(context, ItemIds[0]);
                case Function.GetStudyPlans: return await StudyPlan.GetAll(context);
                case Function.GetStudentHistory: return await Student.History(context, long.Parse(ItemIds[0]));
                case Function.GetStudentAvailable: return await Student.Available(context, long.Parse(ItemIds[0]));
                case Function.GetGestion: return await Gestion.Get(context, long.Parse(ItemIds[0]));
                case Function.GetGestions: return await Gestion.GetAll(context);
                case Function.PostGestion: return await Gestion.Post(context, JsonSerializer.Deserialize<Gestion>(Body));
                case Function.GetPeriod: return await Period.Get(context, long.Parse(ItemIds[0]));
                case Function.GetPeriods: return await Period.GetAll(context);
                case Function.PostPeriod: return await Period.Post(context, JsonSerializer.Deserialize<Period.PostDTO>(Body));
                case Function.GetSubject: return await Subject.Get(context, ItemIds[0]);
                case Function.GetSubjects: return await Subject.GetAll(context);
                case Function.PostSubject: return await Subject.Post(context, JsonSerializer.Deserialize<Subject.PostSubject>(Body));
            }
            return null;
        }
    }
}
