namespace CareerApi.Models
{
    public class CareerDTO
    {
        public long Id { get; set; }
        required public string Name { get; set; }
        public ICollection<StudyPlanDTO> StudyPlans { get; } = new List<StudyPlanDTO>();
    }
}
