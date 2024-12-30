namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Import
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Attributes;
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;

    public class ImportCompetenciesViewModel : ImportCompetenciesFormData
    {
        public DetailFramework Framework { get; set; }
        public bool IsNotBlank { get; set; }
    }
}
