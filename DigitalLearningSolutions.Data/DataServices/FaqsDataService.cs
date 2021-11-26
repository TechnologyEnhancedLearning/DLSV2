namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Support;

    public interface IFaqsDataService
    {
        Faq? GetFaqByIdForTargetGroup(int faqId, bool onlyIfPublished, int? targetGroup);

        IEnumerable<Faq> GetFaqsForTargetGroup(bool onlyIfPublished, int? targetGroup);
    }

    public class FaqsDataService : IFaqsDataService
    {
        private readonly IDbConnection connection;

        public FaqsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public Faq? GetFaqByIdForTargetGroup(int faqId, bool onlyIfPublished, int? targetGroup)
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
                    AND (@targetGroup IS NULL OR TargetGroup = @targetGroup)
                    AND (@onlyIfPublished = 0 OR Published = 1)",
                new { faqId, onlyIfPublished, targetGroup }
            ).SingleOrDefault();
        }

        public IEnumerable<Faq> GetFaqsForTargetGroup(bool onlyIfPublished, int? targetGroup)
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
                    WHERE (@targetGroup IS NULL OR TargetGroup = @targetGroup)
                    AND (@onlyIfPublished = 0 OR Published = 1)
                    ORDER BY Weighting DESC, FAQID DESC",
                new { onlyIfPublished, targetGroup }
            );
        }
    }
}
