namespace DigitalLearningSolutions.Data.Models.SessionData.RoleProfiles
{
    using System;
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    public class SessionNewRoleProfile
    {
        public SessionNewRoleProfile()
        {
            Id = new Guid();
            RoleProfileBase = new RoleProfileBase();
        }
        public Guid Id { get; set; }
        public RoleProfileBase RoleProfileBase { get; set; }
    }
}
