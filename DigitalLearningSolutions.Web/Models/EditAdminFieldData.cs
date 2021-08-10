namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;

    public class EditAdminFieldData
    {
        public EditAdminFieldData(EditAdminFieldViewModel model)
        {
            Id = Guid.NewGuid();
            EditModel = model;
        }

        public Guid Id { get; set; }

        public EditAdminFieldViewModel EditModel { get; set; }
    }
}
