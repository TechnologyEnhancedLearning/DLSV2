﻿namespace DigitalLearningSolutions.Data.DataServices
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
        private const string PublishedFaqsForTargetGroupSql =
            @"SELECT
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
                AND Published = 1";

        private readonly IDbConnection connection;

        public FaqsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public Faq? GetPublishedFaqByIdForTargetGroup(int faqId, int targetGroup)
        {
            return connection.Query<Faq>(
                @$"{PublishedFaqsForTargetGroupSql} AND FAQID = @faqId",
                new { faqId, targetGroup }
            ).SingleOrDefault();
        }

        public IEnumerable<Faq> GetPublishedFaqsForTargetGroup(int targetGroup)
        {
            return connection.Query<Faq>(
                PublishedFaqsForTargetGroupSql,
                new { targetGroup }
            );
        }
    }
}
