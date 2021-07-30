namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;

    public class AddAdminFieldData
    {
        public AddAdminFieldData()
        {
            Id = Guid.NewGuid();
            ConfigureAnswersViewModel = new AdminFieldAnswersViewModel();
        }

        public Guid Id { get; set; }
        public AdminFieldAnswersViewModel ConfigureAnswersViewModel { get; set; }
    }
}
