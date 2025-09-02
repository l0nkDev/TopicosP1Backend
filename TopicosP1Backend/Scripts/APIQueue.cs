using CareerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace TopicosP1Backend.Scripts
{
    public class APIQueue
    {
        private readonly IServiceScopeFactory scopeFactory;
        private Context? context;
        private readonly Thread? Thread = null;
        private Queue<Action> q = [];
        private HashSet<int> queued = new HashSet<int>();
        private Dictionary<int, object> responses = [];

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
                        };
                        Action a = q.Dequeue();
                        a();
                    }
                    else { scope?.Dispose(); scope = null; Thread.Yield(); }
                }
            }
        }
        public void Add(Action action) { q.Enqueue(action); }
        public void AddResponse(int id, object obj) { responses.Add(id, obj); queued.Remove(id); }
        public object Get(int id, bool delete) { object obj = responses[id]; if (delete) responses.Remove(id); return obj; }
        public int? IsQueued(int id) => queued.Contains(id) ? id : null;
        public ActionResult<StudyPlan.StudyPlanDTO> GetStudyPlan(string id, int hash)
        {
            if (queued.Contains(hash)) return new OkResult();
            var studyPlan = context.StudyPlans.Include(_ => _.Career).Include(_ => _.Subjects).ThenInclude(_ => _.Prerequisites).FirstOrDefault(i => i.Code == id);
            if (studyPlan == null) return new NotFoundResult();
            StudyPlan.StudyPlanDTO res = new(studyPlan);
            try { AddResponse(hash, res); } catch { }
            return res;
        }
        public IEnumerable<StudyPlan.StudyPlanDTO> GetStudyPlans(int hash)
        {
            var db = context.StudyPlans.Include(_ => _.Career).Include(_ => _.Subjects).ThenInclude(_ => _.Prerequisites).ToListAsync();
            var studyplans = from sp in db.Result select new StudyPlan.StudyPlanDTO(sp);
            try { AddResponse(hash, studyplans); } catch { }
            return studyplans;
        }

        public ActionResult<List<StudentGroups.HistoryEntry>> GetStudentHistory(long id, int hash)
        {
            var student = context.Students.Find(id);
            if (student == null) { return new NotFoundResult(); }
            var history = context.StudentGroups.Include(_ => _.Group).ThenInclude(_ => _.Subject).Where(_ => _.Student.Id == id && _.Status != 2).ToList();
            List<StudentGroups.HistoryEntry> res = [.. (from a in history select a.Simple())];
            try { AddResponse(hash, res); } catch { }
            return res;
        }

        public ActionResult<List<Subject.SubjectSimple>> GetStudentAvailable(long id, int hash)
        {
            var student = context.Students
                .Include(_ => _.StudentGroups).ThenInclude(_ => _.Group).ThenInclude(_ => _.Subject)
                .Include(_ => _.StudyPlans).ThenInclude(_ => _.SpSubjects).ThenInclude(_ => _.Subject).ThenInclude(_ => _.Prerequisites)
                .First(_ => _.Id == id);
            if (student == null) { return new NotFoundResult(); }
            var spsubjects = (from sp in student.StudyPlans select sp.SpSubjects).SelectMany(_ => _).ToList();
            var passed = (from sub in student.StudentGroups where sub.Grade >= 51 && sub.Status == 1 select sub.Group.Subject).ToList();
            List<Subject> available = new List<Subject>();
            foreach (var sp in spsubjects)
                if (sp.Subject.Prerequisites.All(_ => passed.Contains(_))) available.Add(sp.Subject);
            var res = (from a in available.Except(passed) select a.Simple()).Distinct().ToList();
            try { AddResponse(hash, res); } catch { }
            return res;
        }
    }
}
