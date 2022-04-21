namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class RemoveFromCourseViewModel : IValidatableObject
    {
        public RemoveFromCourseViewModel() { }

        public RemoveFromCourseViewModel(
            int delegateId,
            string name,
            int customisationId,
            string courseName,
            bool confirm,
            DelegateAccessRoute accessedVia,
            ReturnPageQuery? returnPageQuery
        )
        {
            DelegateId = delegateId;
            Name = name;
            CustomisationId = customisationId;
            CourseName = courseName;
            Confirm = confirm;
            AccessedVia = accessedVia;
            ReturnPageQuery = returnPageQuery;
        }

        public int DelegateId { get; set; }
        public string Name { get; set; }
        public int CustomisationId { get; set; }
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
