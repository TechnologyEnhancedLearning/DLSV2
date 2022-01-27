namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Support;

    public interface IFaqsDataService
    {
        Faq? GetFaqById(int faqId);

        IEnumerable<Faq> GetAllFaqs();
    }

    public class FaqsDataService : IFaqsDataService
    {
        private const string FaqsSql =
            @"SELECT
                    FAQID,
	                AHTML,
	                CreatedDate,
	                Published,
	                QAnchor,
	                QText,
	                TargetGroup,
	                Weighting
                FROM FAQs";

        private readonly IDbConnection connection;

        public FaqsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public Faq? GetFaqById(int faqId)
        {
            return connection.Query<Faq>(
                @$"{FaqsSql} WHERE FAQID = @faqId",
                new { faqId }
            ).SingleOrDefault();
        }

        public IEnumerable<Faq> GetAllFaqs()
        {
            return connection.Query<Faq>($@"{FaqsSql}");
        }
    }
}
