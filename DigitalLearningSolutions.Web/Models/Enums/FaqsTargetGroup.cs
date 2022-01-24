namespace DigitalLearningSolutions.Web.Models.Enums
{
    using DigitalLearningSolutions.Data.Enums;

    public class FaqsTargetGroup : Enumeration
    {
        public static readonly FaqsTargetGroup TrackingSystem = new FaqsTargetGroup(0, "Tracking System");

        public static readonly FaqsTargetGroup ProspectiveCentre = new FaqsTargetGroup(1, "Prospective Centre");

        public static readonly FaqsTargetGroup Learners = new FaqsTargetGroup(2, "Learners");

        public static readonly FaqsTargetGroup Frameworks = new FaqsTargetGroup(2, "Frameworks");

        private FaqsTargetGroup(int id, string name) : base(id, name) { }
    }
}
