using FluentMigrator;
namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202307181724)]
    public class UpdateAccessibilityStatementRecord : Migration
    {
        public override void Up()
        {
            var accessibilityStatement = @"<h1 class=""nhsuk-u-text-align-centre nhsuk-heading-xl"">ACCESSIBILITY STATEMENT FOR NHS ENGLAND DIGITAL LEARNING SOLUTIONS</h1>
            <section>
	            <p class=""policy-text-justify"">NHS England (<strong>NHSE</strong>) is committed to making its websites accessible, in accordance with the Public Sector Bodies (Websites and Mobile Applications) (No. 2) Accessibility Regulations 2018 (<strong>Accessibility Regulations</strong>).</p>
	            <p class=""policy-text-justify"">This accessibility statement applies to Digital Learning Solutions (<strong>DLS</strong>). DLS is a free of charge resource for public sector health and care organisations in England and can be accessed by users at home or work. It provides diagnostic tools to assess your current skill level and then subsequently suggests a learning path.</p>
	            <p class=""policy-text-justify"">This website is run by NHSE. We want as many people as possible to be able to use this website.</p>
	            <p class=""policy-text-justify"">For example, that means you should be able to:</p>
	            <ul class=""nhsuk-u-padding-left-6"">
	              <li>change colours, contrast levels, and fonts</li>
	              <li>zoom in up to 300% without the text spilling off the screen</li>
	              <li>navigate most of the website using a keyboard</li>
	              <li>navigate most of the website using speech recognition software</li>
	              <li>listen to most of the website using a screen reader (including the latest versions of JAWS, NVDA, and VoiceOver)</li>
	            </ul>
	            <p class=""policy-text-justify"">We’ve also made the website text as simple as possible to understand.  However, some of our content is technical, and we use technical terms where there is no easier wording which we could use without changing what the text means.</p>
	            <p class=""policy-text-justify"">AbilityNet has advice on making your device easier to use if you have a disability, which can be found here: <a class=""policy-word-break""  href=""https://abilitynet.org.uk/"">https://abilitynet.org.uk/</a>.<p class=""policy-text-justify"">
	            <h2 class=""nhsuk-heading-l"">1.	HOW ACCESSIBLE THIS WEBSITE IS</h2>
	            <p class=""policy-text-justify"">We know some parts of this website are not fully accessible:</p>
	            <p class=""policy-text-justify"">1.1	some pages are not written in plain English</p>
	            <p class=""policy-text-justify"">1.2	most older PDF documents are not fully accessible to screen reader software </p>
	            <p class=""policy-text-justify"">1.3	some of our online forms are difficult to navigate using just a keyboard</p>
	            <p class=""policy-text-justify"">1.4	you cannot skip to the main content when using a screen reader</p>
	            <h2 class=""nhsuk-heading-l"">2.	ACCESSIBILITY HELP</h2>
	            <p class=""policy-text-justify"">We have provided accessibility support on the website, which can be found here: <a class=""policy-word-break"" href=""https://www.dls.nhs.uk/v2/LearningSolutions/AccessibilityHelp"">https://www.dls.nhs.uk/v2/LearningSolutions/AccessibilityHelp</a>.</p>
	            <h2 class=""nhsuk-heading-l"">3.	THIRD PARTY ELEARNING CONTENT</h2>
	            <p class=""policy-text-justify"">DLS also hosts a variety of digital resources such as Word documents, PDF documents, videos and e-learning content developed by third parties. We also link to websites and resources hosted on third party platforms.</p>
	            <p class=""policy-text-justify"">The Accessibility Regulations do not apply to third-party content that is neither funded nor developed by, nor under the control of NHSE.</p>
	            <p class=""policy-text-justify"">Some of the third-party organisations whose content we host have provided their own accessibility statements covering their e-learning content and their contact details should be available on their help pages.</p>
	            <h2 class=""nhsuk-heading-l"">4.	FEEDBACK AND CONTACT INFORMATION</h2>
	            <p class=""policy-text-justify"">If you need information on this website in a different format, then contact us at <a class=""policy-word-break"" href=""support@dls.nhs.uk"">support@dls.nhs.uk</a> and tell us:</p>
	            <p class=""policy-text-justify"">4.1	your name and email address</p>
	            <p class=""policy-text-justify"">4.2	which e-learning resource you are enquiring about</p>
	            <p class=""policy-text-justify"">4.3	the format you need, for example, easy read, audio CD, Braille, BSL or large print, accessible PDF</p>
	            <p class=""policy-text-justify"">We’ll review your request and aim to respond within 10 days.</p>
	            <p class=""policy-text-justify"">You can also view the accessible document policy of the organisation that published the document to report any problems or request documents in an alternative format.</p>
	            <h2 class=""nhsuk-heading-l"">5.	REPORTING ACCESSIBILITY PROBLEMS WITH THIS WEBSITE</h2>
	            <p class=""policy-text-justify"">We’re always looking to improve the accessibility of this website. If you find any problems not listed on this page or think we’re not meeting accessibility requirements, contact <a class=""policy-word-break"" href=""support@dls.nhs.uk"">support@dls.nhs.uk</a>.</p>
	            <h2 class=""nhsuk-heading-l"">6.	ENFORCEMENT PROCEDURE</h2>
	            <p class=""policy-text-justify"">The Equality and Human Rights Commission (EHRC) is responsible for enforcing the Accessibility Regulations. If you’re not happy with how we respond to your complaint, contact the Equality Advisory and Support Service (EASS) here: <a class=""policy-word-break"" href=""https://www.equalityadvisoryservice.com/"">https://www.equalityadvisoryservice.com/</a>.</p>
	            <h2 class=""nhsuk-heading-l"">7.	TECHNICAL INFORMATION ABOUT THIS WEBSITE’S ACCESSIBILITY</h2>
	            <p class=""policy-text-justify"">NHSE is committed to making its website accessible, in accordance with the Public Sector Bodies (Websites and Mobile Applications) (No. 2) Accessibility Regulations 2018, which can be found here: <a class=""policy-word-break"" href=""https://www.legislation.gov.uk/uksi/2018/952/regulation/4/made"">https://www.legislation.gov.uk/uksi/2018/952/regulation/4/made</a>.</p>
	            <h2 class=""nhsuk-heading-l"">8.	COMPLIANCE STATUS</h2>
	            <p class=""policy-text-justify"">This website is partially compliant with the Web Content Accessibility Guidelines version 2.1 AA standard, which can be found here: <a class=""policy-word-break"" href=""https://www.w3.org/TR/WCAG21/"">https://www.w3.org/TR/WCAG21/</a>, due to the non-compliances and exemptions listed below.</p>
	            <h2 class=""nhsuk-heading-l"">9.	NON-ACCESSIBLE CONTENT</h2>
	            <p class=""policy-text-justify"">The content listed below is non-accessible for the following reasons:</p>
	            <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-4"">9.1	Non-compliance with the Accessibility Regulations</h3>
	            <p class=""policy-text-justify nhsuk-u-margin-left-4"">We recognise that some of the DLS e-learning content doesn’t comply with the Accessibility Regulations. Some content is not screen reader compatible, some content is not keyboard navigable, some content does not follow Web Content Accessibility Guidelines accessibility standards, and some video content does not contain closed captions or transcripts.</p>
	            <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-4"">9.2	Disproportionate burden</h3>
	            <p class=""policy-text-justify nhsuk-u-margin-left-4"">While we have endeavoured to ensure the DLS website is as compliant as possible, we believe that auditing the e-learning content specifically for its compliance with the Accessibility Regulations would be a disproportionate burden.</p>
	            <p class=""policy-text-justify nhsuk-u-margin-left-4"">You can read the disproportionate burden statement here: <a class=""policy-word-break"" href=""https://www.hee.nhs.uk/disproportionate-burden-statement"">https://www.hee.nhs.uk/disproportionate-burden-statement</a></p>
	            <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-4"">9.3	Content that’s not within the scope of the Accessibility Regulations</h3>
	            <p class=""nhsuk-u-margin-left-8"">9.3.1	PDFs and non-HTML documents</p>
	            <p class=""policy-text-justify"">Many documents are not accessible in a number of ways including missing text alternatives and missing document structure.</p>
	            <p class=""policy-text-justify"">The Accessibility Regulations do not require us to fix PDFs or other documents published before 23 September 2018 if they’re not essential to providing our services.  Please refer here: <a class=""policy-word-break"" href=""http://www.legislation.gov.uk/uksi/2018/952/regulation/4/made"">http://www.legislation.gov.uk/uksi/2018/952/regulation/4/made</a>.</p>
	            <h2 class=""nhsuk-heading-l"">10.	PREPARATION OF THIS ACCESSIBILITY STATEMENT</h2>
	            <p class=""policy-text-justify"">This statement was prepared on 14 November 2022.  It was last updated on 17 March 2023.</p>
	            <p class=""policy-text-justify"">This website was last tested on 10 November 2022. The test was carried out by Cyber-Duck. The sessions chosen to be tested were the most accessed pages and the pages that the learners interact with the most.  DLS was tested in relation to the following accessibility requirements:<p/>
	            <ul class=""nhsuk-u-padding-left-6 policy-text-justify"">			  
	              <li>Robust - meaning that the content and functionality is compatible with assistive technology</li>
	              <li>Operable - meaning that a user can successfully use controls, buttons, navigation, and other necessary elements</li>
	              <li>Understandable - meaning that content is consistent and appropriate in its presentation, format, and design</li>
	              <li>Perceivable - meaning that the user can easily identify content and interface elements</li>
	            </ul>
            </section>";


            Execute.Sql(@"UPDATE Config SET UpdatedDate = GETDATE() ,ConfigText =N'" + accessibilityStatement + "' " +
                         "where ConfigName='AccessibilityNotice' AND IsHtml = 1");
        }
        public override void Down()
        {
            var accessibilityStatementOld = @"<p>Our site aims to comply with the World Wide Web Consortium''s (W3C''s) Web Accessibility Guidelines to Level AA and we are committed to further improving accessibility.</p><h1>Text size and colour</h1><h2>Changing font sizes and font colours</h2><p>Changing fonts can be useful for you if you have low vision, and need larger fonts or high contrast colours. You can change the font size, style and colour, and choose an alternative colour for links. You can also change background and foreground colours.</p><hr /><h2>Changing fonts in Internet Explorer</h2><ul><li>If you are using Internet Explorer on a PC, select the View menu at the top of your window</li><li>To change font size, scroll down and select the Text size option</li><li>Alternatively, if you have a wheel mouse, hold down the CTRL key and use the wheel to interactively scale the font size</li><li>To ignore font and background colours choose the Internet options from the Tools menu at the top of the window</li><li>On the general tab of the window that appears, click the Accessibility button</li><li>This takes you to a menu where you can choose to ignore the way the page is formatted</li><li>Then return to the Internet options menu, and use the Colours and Fonts buttons to set your preferences</li></ul><hr /><h2>Changing fonts in Firefox</h2><ul><li>Click on ''View'' on the menubar</li><li>Then select ''Zoom''</li><li>Select ''Zoom In'' or ''Zoom Out''</li><li>Alternatively, if you have a wheel mouse, hold down the CTRL key and use the wheel to interactively scale the font size</li><li>Keyboard shortcuts of CTRL plus - and CTRL plus + are also available</li><li>To change the font style, size or colour, choose ''Tools'', ''Options'' and then the ''Content'' tab</li></ul><hr /><h2>Changing fonts in Chrome</h2><ul><li>From the browser, select Preferences from the Edit menu at the top of the window</li><li>Click on Web content and uncheck the Show style sheets option</li><li>Return to the list of preferences and choose Web browser</li><li>Click on Language/Fonts and choose the size you need</li></ul><hr /><h1>Keyboard navigation</h1><p>Arrow keys can be used to scroll up or down the page. You can use your Tab key to move between links, and press Return or Enter to select one. To go back to the previous page, use the Backspace key.</p><hr /><h1>PDF accessibility</h1><p>Useful information about services to make Acrobat documents more accessible is provided on Adobe''s website.</p><ul><li>&nbsp;<a href=""https://www.adobe.com/support/products/acrobat.html"" rel=""external"">Adobe Acrobat support (external)</a></li><li>&nbsp;<a href=""https://www.adobe.com/accessibility/index.html"" rel=""external"">Adobe accessibility (external)</a></li></ul><hr /><h1>Downloading documents</h1><p>Downloadable documents on this site are provided in a variety of formats. The most common are PDF, Word, Excel and Zip.</p><h2>Software for document reading</h2><p>Most computers already have the software to open these document formats. If you do not have Adobe Acrobat Reader (for reading PDFs), it is available from the&nbsp;<a href=""https://www.adobe.com/"" rel=""external"">Adobe website (external)</a>.&nbsp;If you do not have Winzip (for opening zip files), you can download a free trial from the&nbsp;<a href=""https://www.winzip.com/index.htm"" rel=""external"">Winzip website (external).</a></p><h2>Saving documents to your computer</h2><p>If you have a PC, right-click on the link to the document. If you use an Apple Mac, hold down the mouse button over the link. In both cases, a popup menu will then appear. Scroll down the menu and click on ''Save target as''. You will then be asked to choose a folder on your computer where you can save the document.</p><p>Assistive technologies such as screen readers may have their own specific way to save documents, please refer to your preferred software''s Help section.</p>";

            Execute.Sql(@"UPDATE Config SET ConfigText =N'" + accessibilityStatementOld + "' " +
                         "where ConfigName='AccessibilityNotice' AND IsHtml = 1");
        }
    }
}
