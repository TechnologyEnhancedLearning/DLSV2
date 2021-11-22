namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Support;

    public interface IFaqsDataService
    {
        IEnumerable<Faq> GetPublishedFaqsForTargetGroup(int targetGroup);
    }

    public class FaqsDataService : IFaqsDataService
    {
        private readonly IDbConnection connection;

        public FaqsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<Faq> GetPublishedFaqsForTargetGroup(int targetGroup)
        {
            return connection.Query<Faq>(
                @$"SELECT
                        FAQID
	                    AHTML,
	                    CreatedDate,
	                    Published,
	                    QAnchor,
	                    QText,
	                    TargetGroup,
	                    Weighting
                    FROM FAQs AS f
                    WHERE TargetGroup = @targetGroup
                    AND Published = 1
                    ORDER BY Weighting DESC, FAQID DESC",
                new { targetGroup }
            );
        }
    }
}
