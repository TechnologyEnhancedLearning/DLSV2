namespace DigitalLearningSolutions.Data.Mappers
{
    using Dapper.FluentMap;

    public static class MapperHelper
    {
        public static void SetUpFluentMapper()
        {
            FluentMapper.Initialize(fluentMapperConfig =>
            {
                fluentMapperConfig.AddMap(new CurrentCourseMap());
                fluentMapperConfig.AddMap(new CompletedCourseMap());
                fluentMapperConfig.AddMap(new AvailableCourseMap());

                fluentMapperConfig.AddMap(new AdminUserMap());
                fluentMapperConfig.AddMap(new DelegateUserMap());
            });
        }
    }
}
