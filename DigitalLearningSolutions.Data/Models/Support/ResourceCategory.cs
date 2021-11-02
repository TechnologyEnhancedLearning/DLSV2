namespace DigitalLearningSolutions.Data.Models.Support
{
    using System.Collections.Generic;

    public class ResourceCategory
    {
        public ResourceCategory(string categoryName, IEnumerable<Resource> resources)
        {
            CategoryName = categoryName;
            Resources = resources;
        }

        public string CategoryName { get; }
        public IEnumerable<Resource> Resources { get; } 
    }
}
