namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    public class EditCompleteByDateViewModel : EditCompleteByDateFormData
    {
        public EditCompleteByDateViewModel() { }

        public EditCompleteByDateViewModel(
            int id,
            string name,
            string type,
            int? day = null,
            int? month = null,
            int? year = null,
            int? progressId = null
        )
        {
            Id = id;
            Name = name;
            Type = type;
            Day = day;
            Month = month;
            Year = year;
            ProgressId = progressId;
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
