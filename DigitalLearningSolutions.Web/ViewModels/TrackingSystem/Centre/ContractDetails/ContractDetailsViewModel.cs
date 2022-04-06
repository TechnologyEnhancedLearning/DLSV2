namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.ContractDetails
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;

    public class ContractDetailsViewModel
    {
        public ContractDetailsViewModel(List<AdminUser> adminUsers, Centre centreDetails, int numberOfCourses)
        {
            Administrators = adminUsers.Count(a => a.IsCentreAdmin).ToString();
            Supervisors = adminUsers.Count(a => a.IsSupervisor).ToString();

            var trainers = adminUsers.Count(a => a.IsTrainer);
            var cmsAdministrators = adminUsers.Count(a => a.IsCmsAdministrator);
            var cmsManagers = adminUsers.Count(a => a.IsCmsManager);
            var contentCreators = adminUsers.Count(a => a.IsContentCreator);

            Trainers = DisplayStringHelper.FormatNumberWithLimit(trainers, centreDetails.TrainerSpots);
            TrainersColour = DisplayColourHelper.GetDisplayColourForPercentage(trainers, centreDetails.TrainerSpots);

            CmsAdministrators = DisplayStringHelper.FormatNumberWithLimit(
                cmsAdministrators,
                centreDetails.CmsAdministratorSpots
            );
            CmsAdministratorsColour = DisplayColourHelper.GetDisplayColourForPercentage(
                cmsAdministrators,
                centreDetails.CmsAdministratorSpots
            );

            CmsManagers = DisplayStringHelper.FormatNumberWithLimit(
                cmsManagers,
                centreDetails.CmsManagerSpots
            );
            CmsManagersColour = DisplayColourHelper.GetDisplayColourForPercentage(
                cmsManagers,
                centreDetails.CmsManagerSpots
            );

            ContentCreators = DisplayStringHelper.FormatNumberWithLimit(
                contentCreators,
                centreDetails.CcLicenceSpots
            );
            ContentCreatorsColour = DisplayColourHelper.GetDisplayColourForPercentage(
                contentCreators,
                centreDetails.CcLicenceSpots
            );

            CustomCourses = DisplayStringHelper.FormatNumberWithLimit(
                numberOfCourses,
                centreDetails.CustomCourses
            );
            CustomCoursesColour = DisplayColourHelper.GetDisplayColourForPercentage(numberOfCourses, centreDetails.CustomCourses);

            ServerSpace = DisplayStringHelper.FormatBytesWithLimit(
                centreDetails.ServerSpaceUsed,
                centreDetails.ServerSpaceBytes
            );
            ServerSpaceColour = DisplayColourHelper.GetDisplayColourForPercentage(
                centreDetails.ServerSpaceUsed,
                centreDetails.ServerSpaceBytes
            );
        }

        public string Administrators { get; set; }

        public string CmsAdministrators { get; set; }
        public string CmsAdministratorsColour { get; set; }

        public string CmsManagers { get; set; }
        public string CmsManagersColour { get; set; }

        public string ContentCreators { get; set; }
        public string ContentCreatorsColour { get; set; }

        public string Trainers { get; set; }
        public string TrainersColour { get; set; }

        public string Supervisors { get; set; }

        public string CustomCourses { get; set; }
        public string CustomCoursesColour { get; set; }

        public string ServerSpace { get; set; }
        public string ServerSpaceColour { get; set; }
    }
}
