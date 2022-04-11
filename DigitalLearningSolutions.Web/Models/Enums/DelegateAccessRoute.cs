namespace DigitalLearningSolutions.Web.Models.Enums
{
    using DigitalLearningSolutions.Data.Enums;

    public class DelegateAccessRoute : Enumeration
    {
        public static readonly DelegateAccessRoute CourseDelegates =
            new DelegateAccessRoute(0, "CourseDelegates");

        public static readonly DelegateAccessRoute ViewDelegate =
            new DelegateAccessRoute(1, "ViewDelegate");

        public static readonly DelegateAccessRoute TopCourses =
            new DelegateAccessRoute(2, "TopCourses");

        public static readonly DelegateAccessRoute DelegateCourses =
            new DelegateAccessRoute(3, "DelegateCourses");

        private DelegateAccessRoute(int id, string name) : base(id, name) { }
    }
}
