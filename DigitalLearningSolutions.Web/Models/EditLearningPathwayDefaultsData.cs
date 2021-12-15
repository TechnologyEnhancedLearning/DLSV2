namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;

    public class EditLearningPathwayDefaultsData
    {
        public EditLearningPathwayDefaultsData(EditLearningPathwayDefaultsViewModel model)
        {
            Id = Guid.NewGuid();
            LearningPathwayDefaultsModel = model;
        }

        public Guid Id { get; set; }

        public EditLearningPathwayDefaultsViewModel LearningPathwayDefaultsModel { get; set; }
    }
}
