namespace DigitalLearningSolutions.Web.Models.Enums
{
    using DigitalLearningSolutions.Data.Enums;

    public class DelegateProgressAccessRoute : Enumeration
    {
        public static readonly DelegateProgressAccessRoute CourseDelegates =
            new DelegateProgressAccessRoute(0, "CourseDelegates");

        public static readonly DelegateProgressAccessRoute ViewDelegate =
            new DelegateProgressAccessRoute(1, "ViewDelegate");

        private DelegateProgressAccessRoute(int id, string name) : base(id, name) { }
    }
}
