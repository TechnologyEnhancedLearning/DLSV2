namespace DigitalLearningSolutions.Data.Models.Courses
{
    public interface ICourseOptions
    {
        public bool Active { get; set; }
        public bool SelfRegister { get; set; }
        public bool HideInLearnerPortal { get; set; }
        public bool DiagObjSelect { get; set; }
        public bool DiagAssess { get; set; }
    }

    public class CourseOptions : ICourseOptions
    {
        public bool Active { get; set; }
        public bool SelfRegister { get; set; }
        public bool HideInLearnerPortal { get; set; }
        public bool DiagObjSelect { get; set; }
        public bool DiagAssess { get; set; }
    }
}
