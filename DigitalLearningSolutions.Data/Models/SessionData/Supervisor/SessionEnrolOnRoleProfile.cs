namespace DigitalLearningSolutions.Data.Models.SessionData.Supervisor
{
    using System;
    public class SessionEnrolOnRoleProfile
    {
        public SessionEnrolOnRoleProfile()
        {
            Id = new Guid();
        }
        public Guid Id { get; set; }
        public int? SelfAssessmentID { get; set; }
        public DateTime? CompleteByDate { get; set; }
        public int? SelfAssessmentSupervisorRoleId { get; set; }
    }
}
