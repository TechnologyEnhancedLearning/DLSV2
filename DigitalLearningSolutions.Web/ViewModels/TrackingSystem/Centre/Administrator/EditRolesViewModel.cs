namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using DigitalLearningSolutions.Data.Models.User;

    public class EditRolesViewModel
    {
        public EditRolesViewModel() { }

        public EditRolesViewModel(AdminUser user)
        {
            FullName = user.FirstName + " " + user.LastName; // QQ better way of doing this?
            IsCentreAdmin = user.IsCentreAdmin;
            IsSupervisor = user.IsSupervisor;
            IsTrainer = user.IsTrainer;
            IsContentCreator = user.IsContentCreator;
            if (user.ImportOnly && user.IsContentManager)
            {
                ContentManagementRole = 2;
            }
            else if (user.IsContentManager)
            {
                ContentManagementRole = 1;
            }
            else
            {
                ContentManagementRole = 0;
            }

            LearningCategory = user.CategoryId;
        }

        public string? FullName { get; set; }
        public bool IsCentreAdmin { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsTrainer { get; set; }
        public bool IsContentCreator { get; set; }
        public int ContentManagementRole { get; set; }
        public int LearningCategory { get; set; }
    }
}
