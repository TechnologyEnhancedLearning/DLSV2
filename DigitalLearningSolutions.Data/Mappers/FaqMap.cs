namespace DigitalLearningSolutions.Data.Mappers
{
    using Dapper.FluentMap.Mapping;
    using DigitalLearningSolutions.Data.Models.Support;

    public class FaqMap : EntityMap<Faq>
    {
        public FaqMap()
        {
            Map(faq => faq.FaqId).ToColumn("FAQID");
            Map(faq => faq.Ahtml).ToColumn("AHTML");
            Map(faq => faq.Qanchor).ToColumn("QAnchor");
            Map(faq => faq.Qtext).ToColumn("QText");
        }
    }
}
