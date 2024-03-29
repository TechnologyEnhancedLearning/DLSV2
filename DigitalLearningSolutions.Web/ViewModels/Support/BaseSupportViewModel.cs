﻿namespace DigitalLearningSolutions.Web.ViewModels.Support
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public abstract class BaseSupportViewModel : ISupportViewModel
    {
        protected BaseSupportViewModel(
            DlsSubApplication dlsSubApplication,
            SupportPage currentPage,
            string currentSystemBaseUrl
        )
        {
            CurrentPage = currentPage;
            DlsSubApplication = dlsSubApplication;
            CurrentSystemBaseUrl = currentSystemBaseUrl;
        }

        public SupportPage CurrentPage { get; set; }
        public DlsSubApplication DlsSubApplication { get; set; }
        public string CurrentSystemBaseUrl { get; set; }
    }
}
