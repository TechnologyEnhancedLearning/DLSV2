namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class RemoveFromCourseViewModel : IValidatableObject
    {
        public RemoveFromCourseViewModel() { }

        public RemoveFromCourseViewModel(
            DetailedCourseProgress progress,
            bool confirm,
            DelegateAccessRoute accessedVia,
            ReturnPageQuery? returnPageQuery
        )
        {
            DelegateId = progress.DelegateId;
            Name = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                progress.DelegateFirstName,
                progress.DelegateLastName
            );
            CustomisationId = progress.CustomisationId;
            ProgressId = progress.ProgressId;
            CourseName = progress.CourseName;
            Confirm = confirm;
            AccessedVia = accessedVia;
            ReturnPageQuery = returnPageQuery;
        }

        public int DelegateId { get; set; }
        public string Name { get; set; }
        public int CustomisationId { get; set; }
        public int ProgressId { get; set; }
        public string CourseName { get; set; }
        public bool Confirm { get; set; }
        public DelegateAccessRoute AccessedVia { get; set; }
        public ReturnPageQuery? ReturnPageQuery { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

            if (!Confirm)
            {
                validationResults.Add(
                    new ValidationResult(
                        "Confirm you wish to remove this delegate from this course",
                        new[]
                        {
                            nameof(Confirm)
                        }
                    )
                );
            }

            return validationResults;
        }
    }
}
