using System.Diagnostics.CodeAnalysis;
using static CareerApi.Models.StudyPlan;

namespace CareerApi.Models
{
    public class Career
    {
        public long Id { get; set; }
        required public string Name { get; set; }
        public IEnumerable<StudyPlan> StudyPlans { get; } = new List<StudyPlan>();
        public CareerDTO SimpleList() => new CareerDTO(this);
        public class CareerDTO(Career career)
        {
            public long Id { get; set; } = career.Id;
            public string Name { get; set; } = career.Name;
            public IEnumerable<StudyPlanDTO> StudyPlans { get; } = from a in career.StudyPlans select a.Simple();
        }
    }
}
