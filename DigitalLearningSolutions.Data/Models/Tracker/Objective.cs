namespace DigitalLearningSolutions.Data.Models.Tracker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Objective : ObjectiveBase
    {
        public Objective(int tutorialId, IEnumerable<int> interactions, int possible) : base(tutorialId, possible)
        {
            Interactions = interactions.ToList();
        }

        /// <summary>
        /// Constructor for Dapper with signature matching object returned from get-objectives query.
        /// </summary>
        /// <param name="tutorialId"></param>
        /// <param name="interactions">A comma-separated list of interactions, as stored in the db.</param>
        /// <param name="possible"></param>
        public Objective(int tutorialId, string? interactions, int possible) : base(tutorialId, possible)
        {
            Interactions = interactions?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList() ??
                           new List<int>();
        }

        public List<int> Interactions { get; set; }
    }
}
