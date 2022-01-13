namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.Support;

    public static class FaqTestHelper
    {
        /// <remarks>When createdDate is NULL, value defaults to new DateTime(2021, 11, 25, 9, 0, 0)</remarks>
        public static Faq GetDefaultFaq(
            int faqId = 1,
            string aHtml = "<p>Helpful content.<p>",
            DateTime? createdDate = null,
            bool published = true,
            string qAnchor = "QuestionAnchor",
            string qText = "A common question?",
            int targetGroup = 0,
            int weighting = 90
        )
        {
            return new Faq
            {
                FaqId = faqId,
                AHtml = aHtml,
                CreatedDate = createdDate ?? new DateTime(2021, 11, 25, 9, 0, 0),
                Published = published,
                QAnchor = qAnchor,
                QText = qText,
                TargetGroup = targetGroup,
                Weighting = weighting,
            };
        }
    }
}
