namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using DigitalLearningSolutions.Data.Models.User;

    public class SearchableAdminViewModel
    {
        public SearchableAdminViewModel(AdminUser adminUser)
        {
            Id = adminUser.Id;
            Name = adminUser.SearchableName;
            CategoryName = adminUser.CategoryName;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string? CategoryName { get; set; }
    }
}
