namespace DigitalLearningSolutions.Data.Mappers
{
    using Dapper.FluentMap.Mapping;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class AvailableCourseMap : EntityMap<AvailableCourse>
    {
        public AvailableCourseMap()
        {
            Map(course => course.Id).ToColumn("CustomisationID");
            Map(course => course.Name).ToColumn("CourseName");
        }
    }
}
