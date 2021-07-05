namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;

    public class SearchableAdminViewModel
    {
        public SearchableAdminViewModel(AdminUser adminUser)
        {
            Id = adminUser.Id;
            Name = adminUser.SearchableName;
            CategoryName = adminUser.CategoryName ?? "All";
            EmailAddress = adminUser.EmailAddress;
            IsLocked = adminUser.IsLocked;
            Tags = FilterableTagHelper.GetCurrentTagsForAdminUser(adminUser);
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string CategoryName { get; set; }

        public string? EmailAddress { get; set; }

        public bool IsLocked { get; set; }

        public IEnumerable<(string, string)> Tags { get; set; }
    }
}
