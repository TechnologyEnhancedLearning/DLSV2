namespace DigitalLearningSolutions.Web.Models.Enums
{
    using System.Collections.Generic;

    public class DelegateGroupTab : BaseTabEnumeration
    {
        public static DelegateGroupTab Delegates = new DelegateGroupTab(
            1,
            nameof(Delegates),
            "GroupDelegates",
            "Index",
            "Delegates"
        );

        public static DelegateGroupTab Courses = new DelegateGroupTab(
            2,
            nameof(Courses),
            "GroupCourses",
            "Index",
            "Courses"
        );

        public DelegateGroupTab(int id, string name, string controller, string action, string linkText) : base(
            id,
            name,
            controller,
            action,
            linkText
        ) { }

        public override IEnumerable<BaseTabEnumeration> GetAllTabs()
        {
            return GetAll<DelegateGroupTab>();
        }
    }
}
