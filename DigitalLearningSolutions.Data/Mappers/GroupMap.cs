namespace DigitalLearningSolutions.Data.Mappers
{
    using Dapper.FluentMap.Mapping;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;

    public class GroupMap : EntityMap<Group>
    {
        public GroupMap()
        {
            Map(group => group.ShouldAddNewRegistrantsToGroup).ToColumn("AddNewRegistrants");
            Map(group => group.ChangesToRegistrationDetailsShouldChangeGroupMembership).ToColumn("SyncFieldChanges");
        }
    }
}
