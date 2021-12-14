namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableAdminViewModel : BaseFilterableViewModel
    {
        public SearchableAdminViewModel(AdminUser adminUser, AdminUser loggedInAdminUser)
        {
            Id = adminUser.Id;
            Name = adminUser.SearchableName;
            CategoryName = adminUser.CategoryName ?? "All";
            EmailAddress = adminUser.EmailAddress;
            IsLocked = adminUser.IsLocked;

            if (loggedInAdminUser.IsUserAdmin)
            {
                CanShowDeactivateAdminButton = adminUser.Id != loggedInAdminUser.Id;
            }else if (loggedInAdminUser.IsCentreManager)
            {
                CanShowDeactivateAdminButton = !adminUser.IsUserAdmin
                                               && !adminUser.IsCentreManager
                                               && adminUser.Id != loggedInAdminUser.Id;
            }
            
            Tags = FilterableTagHelper.GetCurrentTagsForAdminUser(adminUser);
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string CategoryName { get; set; }

        public string CategoryFilter => nameof(AdminUser.CategoryName) + FilteringHelper.Separator +
                                        nameof(AdminUser.CategoryName) +
                                        FilteringHelper.Separator + CategoryName;

        public string? EmailAddress { get; set; }

        public bool IsLocked { get; set; }

        public bool CanShowDeactivateAdminButton { get; set; }
    }
}
