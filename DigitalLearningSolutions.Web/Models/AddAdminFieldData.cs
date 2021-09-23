namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;

    public class AddAdminFieldData
    {
        public AddAdminFieldData(AddAdminFieldViewModel model)
        {
            Id = Guid.NewGuid();
            AddModel = model;
        }

        public Guid Id { get; set; }

        public AddAdminFieldViewModel AddModel { get; set; }
    }
}
