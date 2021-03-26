namespace DigitalLearningSolutions.Data.Mappers
{
    using Dapper.FluentMap.Mapping;
    using DigitalLearningSolutions.Data.Models.User;

    public class DelegateUserMap : EntityMap<DelegateUser>
    {
        public DelegateUserMap()
        {
            Map(delegateUser => delegateUser.Id).ToColumn("CandidateID");
            Map(delegateUser => delegateUser.FirstName).ToColumn("FirstName");
            Map(delegateUser => delegateUser.Surname).ToColumn("LastName");
            Map(delegateUser => delegateUser.EmailAddress).ToColumn("EmailAddress");
            Map(delegateUser => delegateUser.ResetPasswordId).ToColumn("ResetPasswordID");
        }
    }
}
