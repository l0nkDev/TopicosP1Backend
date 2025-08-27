using System.Diagnostics.CodeAnalysis;

namespace CareerApi.Models
{
    public class Career
    {
        public long Id { get; set; }
        required public string Name { get; set; }
        public IEnumerable<StudyPlan> StudyPlans { get; } = new List<StudyPlan>();
    }

    public class CareerDTO
    {
        public long Id { get; set; }
        required public string Name { get; set; }
        public IEnumerable<StudyPlanDTO> StudyPlans { get; } = new List<StudyPlanDTO>();

        [SetsRequiredMembers]
        public CareerDTO(Career career)
        {
            Id = career.Id;
            Name = career.Name;
            StudyPlans = from a in career.StudyPlans select new StudyPlanDTO(a);
        }
    }
}
