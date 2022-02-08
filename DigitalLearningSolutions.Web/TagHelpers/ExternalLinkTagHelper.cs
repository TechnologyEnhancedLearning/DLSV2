namespace DigitalLearningSolutions.Web.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;

    [HtmlTargetElement("A", Attributes = CustomAttribute.UseExternalTabOpener)]
    [HtmlTargetElement("FORM", Attributes = CustomAttribute.UseExternalTabOpener)]
    public class ExternalLinkTagHelper : TagHelper
    {
        [HtmlAttributeName(CustomAttribute.UseExternalTabOpener)]
        public bool UseExternalTabOpener { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (UseExternalTabOpener)
            {
                output.Attributes.Add("target", "_blank");
                output.Attributes.Add("rel", "noopener noreferrer");
            }
        }
    }
}
