namespace DigitalLearningSolutions.Data.Models.CourseDelegates
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class CourseDelegate : DelegateCourseInfo
    {
        public CourseDelegate() { }

        public CourseDelegate(DelegateCourseInfo delegateCourseInfo) :
            base(delegateCourseInfo) { }

        public string FullNameForSearchingSorting =>
            NameQueryHelper.GetSortableFullName(DelegateFirstName, DelegateLastName);

        public bool HasCompleted => Completed.HasValue;
        public bool Removed => RemovedDate.HasValue;

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? FullNameForSearchingSorting;
            set => SearchableNameOverrideForFuzzySharp = value;
        }

        public override string?[] SearchableContent => new[] { SearchableName, DelegateEmail, CandidateNumber };
    }
}
