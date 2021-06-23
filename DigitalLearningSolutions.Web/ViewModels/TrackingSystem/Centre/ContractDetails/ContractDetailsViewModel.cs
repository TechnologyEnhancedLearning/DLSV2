namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.ContractDetails
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;

    public class ContractDetailsViewModel
    {
        public ContractDetailsViewModel(List<AdminUser> adminUsers, Centre centreDetails, int numberOfCourses)
        {
            Administrators = adminUsers.Count(a => a.IsCentreAdmin).ToString();
            Supervisors = adminUsers.Count(a => a.IsSupervisor).ToString();

            var trainers = adminUsers.Count(a => a.IsTrainer);
            var cmsAdministrators = adminUsers.Count(a => a.ImportOnly);
            var cmsManagers = adminUsers.Count(a => a.IsContentManager) - cmsAdministrators;
            var contentCreators = adminUsers.Count(a => a.IsContentCreator);

            Trainers = DisplayStringHelper.GenerateNumberWithLimitDisplayString(trainers, centreDetails.TrainerSpots);
            TrainersColour = GetColourFromPercentageFilled(trainers, centreDetails.TrainerSpots);

            CmsAdministrators = DisplayStringHelper.GenerateNumberWithLimitDisplayString(
                cmsAdministrators,
                centreDetails.CmsAdministratorSpots
            );
            CmsAdministratorsColour = GetColourFromPercentageFilled(
                cmsAdministrators,
                centreDetails.CmsAdministratorSpots
            );

            CmsManagers = DisplayStringHelper.GenerateNumberWithLimitDisplayString(
                cmsManagers,
                centreDetails.CmsManagerSpots
            );
            CmsManagersColour = GetColourFromPercentageFilled(
                cmsManagers,
                centreDetails.CmsManagerSpots
            );

            ContentCreators = DisplayStringHelper.GenerateNumberWithLimitDisplayString(
                contentCreators,
                centreDetails.CcLicenceSpots
            );
            ContentCreatorsColour = GetColourFromPercentageFilled(
                contentCreators,
                centreDetails.CcLicenceSpots
            );

            CustomCourses = DisplayStringHelper.GenerateNumberWithLimitDisplayString(
                numberOfCourses,
                centreDetails.CustomCourses
            );
            CustomCoursesColour = GetColourFromPercentageFilled(numberOfCourses, centreDetails.CustomCourses);

            ServerSpace = DisplayStringHelper.GenerateBytesLimitDisplayString(
                centreDetails.ServerSpaceUsed,
                centreDetails.ServerSpaceBytes
            );
            ServerSpaceColour = GetColourFromPercentageFilled(
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

        private string GetColourFromPercentageFilled(long number, long limit)
        {
            if (limit == 0 && number == 0)
            {
                return "grey";
            }

            var usage = (double)number / limit;

            if (0 <= usage && usage < 0.6)
            {
                return "green";
            }

            if (0.6 <= usage && usage < 1)
            {
                return "yellow";
            }

            if (usage >= 1)
            {
                return "red";
            }

            return "blue";
        }
    }
}
