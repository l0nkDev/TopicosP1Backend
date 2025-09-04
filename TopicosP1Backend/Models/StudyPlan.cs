using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TopicosP1Backend.Scripts;

namespace CareerApi.Models
{
    public class StudyPlan
    {
        [Key]
        required public string Code { get; set; }
        public Career Career { get; set; } = null!;
        public IEnumerable<SpSubject> SpSubjects { get; set; } = new List<SpSubject>();
        public IEnumerable<Subject> Subjects { get; set; } = new List<Subject>();
        public IEnumerable<Student> Students { get; set; } = new List<Student>();
        public StudyPlanDTO Simple() => new(this);

        public class StudyPlanDTO(StudyPlan studyPlan)
        {
            public string Code { get; set; } = studyPlan.Code;
            public string Career { get; set; } = studyPlan.Career.Name;
            public IEnumerable<SpSubject.SpSubjectDTO> Subjects { get; set; } = 
                from a in studyPlan.SpSubjects select a.SimpleList();
        }
        public static async Task<ActionResult<StudyPlanDTO>> Get(Context context, string id)
        {
            var studyPlan = await context.StudyPlans.FirstOrDefaultAsync(i => i.Code == id);
            if (studyPlan == null) return new NotFoundResult();
            StudyPlan.StudyPlanDTO res = new(studyPlan);
            return res;
        }
        public static async Task<IEnumerable<StudyPlanDTO>> GetAll(Context context)
        {
            var db = await context.StudyPlans.ToListAsync();
            var studyplans = from sp in db select new StudyPlanDTO(sp);
            return studyplans;
        }
    }
}
