namespace DigitalLearningSolutions.Data.Models.SessionData.RoleProfiles
{
    using System;
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    public class SessionNewRoleProfile
    {
        public SessionNewRoleProfile()
        {
            Id = new Guid();
            RoleProfile = new RoleProfile();
        }
        public Guid Id { get; set; }
        public RoleProfile RoleProfile { get; set; }
    }
}
