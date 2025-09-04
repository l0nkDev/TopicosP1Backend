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
        GetGestion, GetGestions, PostGestion
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
                case Function.GetStudyPlan: return await GetStudyPlan(context, ItemIds[0]);
                case Function.GetStudyPlans: return await GetStudyPlans(context);
                case Function.GetStudentHistory: return await GetStudentHistory(context, long.Parse(ItemIds[0]));
                case Function.GetStudentAvailable: return await GetStudentAvailable(context, long.Parse(ItemIds[0]));
                case Function.GetGestion: return await GetGestion(context, long.Parse(ItemIds[0]));
                case Function.GetGestions: return await GetGestions(context);
                case Function.PostGestion: return await PostGestion(context, JsonSerializer.Deserialize<Gestion>(Body));
            }
            return null;
        }
        private static async Task<ActionResult<StudyPlan.StudyPlanDTO>> GetStudyPlan(Context context, string id)
        {
            var studyPlan = await context.StudyPlans.FirstOrDefaultAsync(i => i.Code == id);
            if (studyPlan == null) return new NotFoundResult();
            StudyPlan.StudyPlanDTO res = new(studyPlan);
            return res;
        }
        private static async Task<IEnumerable<StudyPlan.StudyPlanDTO>> GetStudyPlans(Context context)
        {
            var db = await context.StudyPlans.ToListAsync();
            var studyplans = from sp in db select new StudyPlan.StudyPlanDTO(sp);
            return studyplans;
        }

        private static async Task<ActionResult<List<StudentGroups.HistoryEntry>>> GetStudentHistory(Context context, long id)
        {
            var student = await context.Students.FindAsync(id);
            if (student == null) { return new NotFoundResult(); }
            var history = await context.StudentGroups.Where(_ => _.Student.Id == id && _.Status != 2).ToListAsync();
            List<StudentGroups.HistoryEntry> res = [.. (from a in history select a.Simple())];
            return res;
        }

        private static async Task<ActionResult<List<Subject.SubjectWithGroups>>> GetStudentAvailable(Context context, long id)
        {
            var student = await context.Students.FirstAsync(_ => _.Id == id);
            if (student == null) { return new NotFoundResult(); }
            var spsubjects = (from sp in student.StudyPlans select sp.SpSubjects).SelectMany(_ => _).ToList();
            var passed = (from sub in student.StudentGroups where sub.Grade >= 51 && sub.Status == 1 select sub.Group.Subject).ToList();
            List<Subject> available = new List<Subject>();
            foreach (var sp in spsubjects)
                if (sp.Subject.Prerequisites.All(_ => passed.Contains(_))) available.Add(sp.Subject);
            var res = (from a in available.Except(passed).Distinct().ToList() select a.WithGroups()).ToList();
            return res;
        }
        public async Task<IEnumerable<Gestion.GestionDTO>> GetGestions(Context context)
        {
            var l = await context.Gestions.ToListAsync();
            return from a in l select a.Simple();
        }

        public async Task<ActionResult<Gestion.GestionDTO>> GetGestion(Context context, long id)
        {
            var gestion = await context.Gestions.FirstOrDefaultAsync(_ => _.Year == id);
            if (gestion == null) return new NotFoundResult();
            return gestion.Simple();
        }
        public async Task<ActionResult<Gestion>> PostGestion(Context _context, Gestion gestion)
        {
            _context.Gestions.Add(gestion);
            await _context.SaveChangesAsync();
            return gestion;
        }
    }
}
