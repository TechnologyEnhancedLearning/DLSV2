namespace DigitalLearningSolutions.Data.Mappers
{
    using Dapper.FluentMap.Mapping;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class CompletedCourseMap : EntityMap<CompletedCourse>
    {
        public CompletedCourseMap()
        {
            Map(course => course.Id).ToColumn("CustomisationID");
            Map(course => course.Name).ToColumn("CourseName");
        }
    }
}
