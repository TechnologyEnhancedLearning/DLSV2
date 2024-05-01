using System;

namespace DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre
{
    public class ConfirmationViewModel
    {
        public ConfirmationViewModel(string delegateNumber, DateTime? welcomeEmailDate)
        {
            DelegateNumber = delegateNumber;
            if (welcomeEmailDate != null)
            {
                Day = welcomeEmailDate.Value.Day;
                Month = welcomeEmailDate.Value.Month;
                Year = welcomeEmailDate.Value.Year;
            }
        }

        public string DelegateNumber { get; set; }
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
    }
}
