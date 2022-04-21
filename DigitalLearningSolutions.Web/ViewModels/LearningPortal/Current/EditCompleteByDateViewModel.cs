namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class EditCompleteByDateViewModel : EditCompleteByDateFormData
    {
        public EditCompleteByDateViewModel() { }

        public EditCompleteByDateViewModel(
            int id,
            string name,
            LearningItemType type,
            DateTime? completeByDate,
            ReturnPageQuery returnPageQuery,
            int? progressId = null,
            bool? apiIsAccessible = null
        )
        {
            Id = id;
            Name = name;
            Type = type;
            Day = completeByDate?.Day;
            Month = completeByDate?.Month;
            Year = completeByDate?.Year;
            ProgressId = progressId;
            ApiIsAccessible = apiIsAccessible;
            ReturnPageQuery = returnPageQuery;
        }

        public EditCompleteByDateViewModel(
            EditCompleteByDateFormData formData,
            int id,
            int? progressId = null
        ) : base(formData)
        {
            Id = id;
            ProgressId = progressId;
        }

        public int Id { get; set; }
    }
}
