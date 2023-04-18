namespace DigitalLearningSolutions.Web.Helpers
{
    public static class AdminCategoryHelper
    {
        public static int? AdminCategoryToCategoryId(int adminCategory)
        {
            return adminCategory == 0 ? (int?)null : adminCategory;
        }

        public static int CategoryIdToAdminCategory(int? categoryId)
        {
            return categoryId ?? 0;
        }
    }
}
