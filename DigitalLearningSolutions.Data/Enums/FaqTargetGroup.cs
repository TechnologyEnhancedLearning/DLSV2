namespace DigitalLearningSolutions.Data.Enums
{
    public class FaqTargetGroup : Enumeration
    {
        public static FaqTargetGroup TrackingSystem = new FaqTargetGroup(
            0,
            nameof(TrackingSystem),
            0
        );

        public static FaqTargetGroup Frameworks = new FaqTargetGroup(
            3,
            nameof(Frameworks),
            1
        );

        private readonly int dlsSubApplicationId;

        private FaqTargetGroup(int id, string name, int dlsSubApplicationId) : base(id, name)
        {
            this.dlsSubApplicationId = dlsSubApplicationId;
        }

        public static FaqTargetGroup? FromDlsSubApplicationId(int dlsSubApplicationId)
        {
            return TryParse<FaqTargetGroup>(
                targetGroup => targetGroup.dlsSubApplicationId == dlsSubApplicationId,
                out var parsedEnum
            )
                ? parsedEnum
                : null;
        }
    }
}
