namespace DigitalLearningSolutions.Web.Models.Enums
{
    using DigitalLearningSolutions.Data.Enums;

    public class FaqsTargetGroup : Enumeration
    {
        public static readonly FaqsTargetGroup TrackingSystem = new FaqsTargetGroup(0, nameof(TrackingSystem), "Tracking System");

        public static readonly FaqsTargetGroup ProspectiveCentre = new FaqsTargetGroup(1, nameof(ProspectiveCentre), "Prospective Centre");

        public static readonly FaqsTargetGroup Learners = new FaqsTargetGroup(2, nameof(Learners), "Learners");

        public static readonly FaqsTargetGroup Frameworks = new FaqsTargetGroup(3, nameof(Frameworks), "Frameworks");

        public readonly string DisplayName;

        private FaqsTargetGroup(int id, string name, string displayName) : base(id, name)
        {
            DisplayName = displayName;
        }
    }
}
