using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{

    public class ChangeRoleSelectionViewModel()
    {
        public string DelegateFirstName { get; set; }
        public string DelegateLastName { get; set; }
        public int CandidateAssessmentID { get; set; }
        public int SupervisorDelegateID { get; set; }
        public int SelfAssessmentID { get; set; }
        public OptionViewModel<string> SupervisorRoleOptions { get; set; } = new();
    }


    public class OptionViewModel<T>
    {
        public string GroupName { get; set; } = "RoleOptions"; // Radio button group name
        public List<SelectOption<T>> Options { get; set; } = new();
        public T SelectedValue { get; set; } = default!;
    }

    public class SelectOption<T>
    {
        public T Value { get; set; } = default!;
        public string Text { get; set; } = string.Empty;
    }
}
