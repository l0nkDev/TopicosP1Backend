namespace CareerApi.Models
{
    public class SpSubject
    {
        public long Id { get; set; }
        required public StudyPlan StudyPlan { get; set; } 
        required public Subject Subject { get; set; }
        required public int Credits { get; set; }
        required public int Level {  get; set; }
        required public int Type { get; set; }
    }
}
