namespace DigitalLearningSolutions.Data.Models.SessionData.Frameworks
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    public sealed class SessionNewFramework
    {
        public SessionNewFramework()
        {
            Id = new Guid();
            DetailFramework = new DetailFramework();
        }
        public Guid Id { get; set; }
        public DetailFramework DetailFramework { get; set; }
    }
}
