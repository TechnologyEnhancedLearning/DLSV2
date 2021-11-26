namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Support;

    public interface IFaqsDataService
    {
        Faq? GetPublishedFaqByIdForTargetGroup(int faqId, int targetGroup);

        IEnumerable<Faq> GetPublishedFaqsForTargetGroup(int targetGroup);
    }

    public class FaqsDataService : IFaqsDataService
    {
        private readonly IDbConnection connection;

        public FaqsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public Faq? GetPublishedFaqByIdForTargetGroup(int faqId, int targetGroup)
        {
            return connection.Query<Faq>(
                @$"SELECT
                        FAQID,
	                    AHTML,
	                    CreatedDate,
	                    Published,
	                    QAnchor,
	                    QText,
	                    TargetGroup,
	                    Weighting
                    FROM FAQs
                    WHERE FAQID = @faqId
                    AND TargetGroup = @targetGroup
                    AND Published = 1",
                new { faqId, targetGroup }
            ).SingleOrDefault();
        }

        public IEnumerable<Faq> GetPublishedFaqsForTargetGroup(int targetGroup)
        {
            return connection.Query<Faq>(
                @$"SELECT
                        FAQID,
	                    AHTML,
	                    CreatedDate,
	                    Published,
	                    QAnchor,
	                    QText,
	                    TargetGroup,
	                    Weighting
                    FROM FAQs
                    WHERE TargetGroup = @targetGroup
                    AND Published = 1",
                new { targetGroup }
            );
        }
    }
}
