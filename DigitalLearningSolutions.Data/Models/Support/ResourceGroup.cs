namespace DigitalLearningSolutions.Data.Models.Support
{
    using System.Collections.Generic;

    public class ResourceGroup
    {
        public ResourceGroup(string category, IEnumerable<Resource> resources)
        {
            Category = category;
            Resources = resources;
        }

        public string Category { get; }
        public IEnumerable<Resource> Resources { get; } 
    }
}
