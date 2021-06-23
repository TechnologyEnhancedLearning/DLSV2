namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System.Linq;

    public static class QueryableHelper
    {
        public static IQueryable<SortableItem> GetListOfSortableItems(string name1, int number1, string name2, int number2, string name3, int number3)
        {
            return new[] { new SortableItem(name1, number1), new SortableItem(name2, number2), new SortableItem(name3, number3) }.AsQueryable();
        }
    }
}
