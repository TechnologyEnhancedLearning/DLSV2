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
            Collaborators = new List<Collaborator>();
        }
        public Guid Id { get; set; }
        public DetailFramework DetailFramework { get; set; }
        public List<Collaborator> Collaborators { get; set; }
    }
}
