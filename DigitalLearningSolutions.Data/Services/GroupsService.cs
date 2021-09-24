namespace DigitalLearningSolutions.Data.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;

    public interface IGroupsService
    {
        int AddDelegateGroup(int centreId, string groupLabel, string? groupDescription, int adminUserId);
    }

    public class GroupsService : IGroupsService
    {
        private readonly IGroupsDataService groupsDataService;
        private readonly IClockService clockService;

        public GroupsService(IGroupsDataService groupsDataService, IClockService clockService)
        {
            this.groupsDataService = groupsDataService;
            this.clockService = clockService;
        }

        public int AddDelegateGroup(int centreId, string groupLabel, string? groupDescription, int adminUserId)
        {
            var groupDetails = new GroupDetails
            {
                CentreId = centreId,
                GroupLabel = groupLabel,
                GroupDescription = groupDescription,
                AdminUserId = adminUserId,
                CreatedDate = clockService.UtcNow,
                LinkedToField = 0,
                SyncFieldChanges = false,
                AddNewRegistrants = false,
                PopulateExisting = false
            };

            return groupsDataService.AddDelegateGroup(groupDetails);
        }
    }
}
