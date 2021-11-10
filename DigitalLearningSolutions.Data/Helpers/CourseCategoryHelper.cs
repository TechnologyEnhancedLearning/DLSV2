namespace DigitalLearningSolutions.Data.Helpers
{
    public static class CourseCategoryHelper
    {
        /// <summary>
        ///     Converts a category ID into a category filter to match the
        ///     data service convention of not filtering on NULL category filter
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static int? GetCourseCategoryFilter(int categoryId)
        {
            return categoryId == 0 ? null : (int?)categoryId;
        }
    }
}
