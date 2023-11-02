namespace DigitalLearningSolutions.Web.Models.Enums
{
    using DigitalLearningSolutions.Data.Enums;

    public class DelegateAccessRoute : Enumeration
    {
        public static readonly DelegateAccessRoute ActivityDelegates =
            new DelegateAccessRoute(0, "ActivityDelegates");

        public static readonly DelegateAccessRoute ViewDelegate =
            new DelegateAccessRoute(1, "ViewDelegate");

        private DelegateAccessRoute(int id, string name) : base(id, name) { }
    }
}
