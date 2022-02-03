namespace DigitalLearningSolutions.Web.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;

    [HtmlTargetElement("A", Attributes = CustomAttribute.ExternalLink)]
    public class ExternalLinkTagHelper : TagHelper
    {
        [HtmlAttributeName(CustomAttribute.ExternalLink)]
        public bool ShowExternalLinkAttributes { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ShowExternalLinkAttributes)
            {
                output.Attributes.Add("target", "_blank");
                output.Attributes.Add("rel", "noopener noreferrer");
            }
        }
    }
}
