namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System;

    public class AttemptStats
    {
        public AttemptStats(int totalAttempts, int attemptsPassed)
        {
            TotalAttempts = totalAttempts;
            AttemptsPassed = attemptsPassed;
        }

        public int TotalAttempts { get; set; }
        public int AttemptsPassed { get; set; }
        public double PassRate => TotalAttempts == 0 ? 0 : Math.Round(100 * AttemptsPassed / (double)TotalAttempts);
    }
}
