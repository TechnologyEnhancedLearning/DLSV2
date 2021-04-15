namespace DigitalLearningSolutions.Data.Mappers
{
    using Dapper.FluentMap.Mapping;
    using DigitalLearningSolutions.Data.Models.User;

    public class DelegateUserMap : EntityMap<DelegateUser>
    {
        public DelegateUserMap()
        {
            // Columns derived from User abstract class
            Map(delegateUser => delegateUser.Id).ToColumn("CandidateID");
            Map(delegateUser => delegateUser.CentreId).ToColumn("CentreID");
            Map(delegateUser => delegateUser.CentreName).ToColumn("CentreName");
            Map(delegateUser => delegateUser.FirstName).ToColumn("FirstName");
            Map(delegateUser => delegateUser.LastName).ToColumn("LastName");
            Map(delegateUser => delegateUser.EmailAddress).ToColumn("EmailAddress");
            Map(delegateUser => delegateUser.Password).ToColumn("Password");
            Map(delegateUser => delegateUser.ResetPasswordId).ToColumn("ResetPasswordID");

            // Columns specific to DelegateUser class
            Map(delegateUser => delegateUser.Approved).ToColumn("Approved");
            Map(delegateUser => delegateUser.CandidateNumber).ToColumn("CandidateNumber");
        }
    }
}
