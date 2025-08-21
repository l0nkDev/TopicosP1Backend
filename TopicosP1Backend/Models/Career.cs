namespace CareerApi.Models
{
    public class Career
    {
        public long Id { get; set; }
        required public string Name { get; set; }
        public ICollection<StudyPlan> StudyPlans { get; } = new List<StudyPlan>();
    }
}
