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
                case Function.GetStudyPlan: return await GetStudyPlan(context, ItemIds[0]);
                case Function.GetStudyPlans: return await GetStudyPlans(context);
                case Function.GetStudentHistory: return await GetStudentHistory(context, long.Parse(ItemIds[0]));
                case Function.GetStudentAvailable: return await GetStudentAvailable(context, long.Parse(ItemIds[0]));
                case Function.GetGestion: return await GetGestion(context, long.Parse(ItemIds[0]));
                case Function.GetGestions: return await GetGestions(context);
                case Function.PostGestion: return await PostGestion(context, JsonSerializer.Deserialize<Gestion>(Body));
                case Function.GetPeriod: return await GetPeriod(context, long.Parse(ItemIds[0]));
                case Function.GetPeriods: return await GetPeriods(context);
                case Function.PostPeriod: return await PostPeriod(context, JsonSerializer.Deserialize<Period.PostDTO>(Body));
                case Function.GetSubject: return await GetSubject(context, ItemIds[0]);
                case Function.GetSubjects: return await GetSubjects(context);
                case Function.PostSubject: return await PostSubject(context, JsonSerializer.Deserialize<Subject.PostSubject>(Body));
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
        public async Task<ActionResult<IEnumerable<Period.PeriodDTO>>> GetPeriods(Context _context)
        {
            var periods = await _context.Periods.ToListAsync();
            return (from a in periods select a.Simple()).ToList();
        }
        public async Task<ActionResult<Period.PeriodDTO>> GetPeriod(Context _context, long id)
        {
            var period = await _context.Periods.FindAsync(id);

            if (period == null)
            {
                return new NotFoundResult();
            }

            return period.Simple();
        }
        public async Task<ActionResult<Period>> PostPeriod(Context _context, Period.PostDTO period)
        {
            Gestion? g = await _context.Gestions.FindAsync(period.Gestion);
            if (g == null) { g = new() { Year = period.Gestion }; _context.Gestions.Add(g); }
            Period p = new() { Id = period.Id, Number = period.Number, Gestion = g };
            _context.Periods.Add(p);
            await _context.SaveChangesAsync();
            return new OkResult();
        }
        public async Task<ActionResult<IEnumerable<Subject.SubjectSimple>>> GetSubjects(Context _context)
        {
            var l = await _context.Subjects.ToListAsync();
            return (from a in l select a.Simple()).ToList();
        }

        // GET: api/Subjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subject.SubjectSimple>> GetSubject(Context _context, string id)
        {
            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
            {
                return new NotFoundResult();
            }

            return subject.Simple();
        }

        public async Task<ActionResult<Subject>> PostSubject(Context _context, Subject.PostSubject s)
        {
            Subject subject = new Subject() { Code = s.Code, Title = s.Title };
            _context.Subjects.Add(subject);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch { return new ConflictResult(); }
            return subject;
        }
    }
}
