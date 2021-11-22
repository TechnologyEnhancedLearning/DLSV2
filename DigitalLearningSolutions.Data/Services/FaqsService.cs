namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Support;

    public interface IFaqsService
    {
        IEnumerable<Faq> GetPublishedFaqsForTargetGroup(int targetGroup);
    }

    public class FaqsService : IFaqsService
    {
        private readonly IFaqsDataService faqsDataService;

        public FaqsService(IFaqsDataService faqsDataService)
        {
            this.faqsDataService = faqsDataService;
        }

        public IEnumerable<Faq> GetPublishedFaqsForTargetGroup(int targetGroup)
        {
            return faqsDataService.GetPublishedFaqsForTargetGroup(targetGroup);
        }
    }
}
