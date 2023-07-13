using FluentMigrator;
namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202307101239)]
    public class AddAcceptableUsePolicyRecord : Migration
    {
        public override void Up()
        {
            var acceptableUsePolicy = @"<h1 class=""policy-text-center"">ACCEPTABLE USE POLICY</h1>
                                <ol class=""custom-ordered-list nhsuk-u-padding-left-0"">
                                    <li class=""h2 nhsuk-heading-l nhsuk-u-margin-0"">
                                        General
                                        <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-4 nhsuk-u-padding-left-0"">
                                            <li>This Acceptable Use Policy sets out how we permit you to use any of our Platforms. Your compliance with this Acceptable Use Policy is a condition of your use of the Platform.</li>
                                            <li>Capitalised terms have the meaning given to them in the terms of use for the Platform which are available at <a class=""policy-word-break"" href=""https://www.dls.nhs.uk/v2/LearningSolutions/Terms"">https://www.dls.nhs.uk/v2/LearningSolutions/Terms</a>.</li>
                                        </ol>
                                    </li>
                                    <li class=""h2 nhsuk-heading-l nhsuk-u-margin-0"">
                                        Acceptable use
                                        <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-4 nhsuk-u-padding-left-0"">
                                            <li>You are permitted to use the Platform as set out in the Terms and for the purpose of personal study.</li>
                                            <li>You must not use any part of the Content on the Platform for commercial purposes without obtaining a licence to do so from us or our licensors.</li>
                                            <li>If you print off, copy, download, share or repost any part of the Platform in breach of this Acceptable Use Policy, your right to use the Platform will cease immediately and you must, at our option, return or destroy any copies of the materials you have made.</li>
                                            <li>Our status (and that of any identified contributors) as the authors of Content on the Platform must always be acknowledged (except in respect of Third-Party Content).</li>
                                        </ol>
                                    </li>
                                    <li class=""h2 nhsuk-heading-l nhsuk-u-margin-0 nhsuk-u-padding-bottom-2"">
                                        Prohibited uses
                                        <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-4 nhsuk-u-padding-left-0"">
                                            <li>
                                                You may not use the Platform:
                                                <ol class=""nhsuk-u-padding-top-2"">
                                                    <li>in any way that breaches any applicable local, national or international law or regulation;</li>
                                                    <li>in any way that is unlawful or fraudulent or has any unlawful or fraudulent purpose or effect;</li>
                                                    <li>in any way that infringes the rights of, or restricts or inhibits the use and enjoyment of this site by any third party;</li>
                                                    <li>for the purpose of harming or attempting to harm minors in any way;</li>
                                                    <li>to bully, insult, intimidate or humiliate any person;</li>
                                                    <li>to send, knowingly receive, upload, download, use or re-use any material which does not comply with our Content Standards as set out in paragraph 4;</li>
                                                    <li>to transmit, or procure the sending of, any unsolicited or unauthorised advertising or promotional material or any other form of similar solicitation (spam), or any unwanted or repetitive content that may cause disruption to the Platform or diminish the user experience, of the Platform’s usefulness or relevant to others;</li>
                                                    <li>to do any act or thing with the intention of disrupting the Platform in any way, including uploading any malware or links to malware, or introduce any virus, trojan, worm, logic bomb or other material that is malicious or technologically harmful or other potentially damaging items into the Platform;</li>
                                                    <li>to knowingly transmit any data, send or upload any material that contains viruses, Trojan horses, worms, time-bombs, keystroke loggers, spyware, adware or any other harmful programs or similar computer code designed to adversely affect the operation of any computer software or hardware; or</li>
                                                    <li>to upload terrorist content.</li>
                                                </ol>
                                            </li>
                                            <li>
                                                You also agree:
                                                <ol class=""nhsuk-u-padding-top-2"">
                                                    <li>to follow any reasonable instructions given to you by us in connection with your use of the Platform;</li>
                                                    <li>to respect the rights and dignity of others, in order to maintain the ethos and good reputation of the NHS, the public good generally and the spirit of cooperation between those studying and working within the health and care sector. In particular, you must act in a professional manner with regard to all other users of the Platform at all times;</li>
                                                    <li>
                                                        not to modify or attempt to modify any of the Content, save:
                                                        <ol class=""nhsuk-u-padding-top-2"">
                                                            <li>in respect of Contributions;</li>
                                                            <li>where you are the editor of a catalogue within the Learning Hub, you may alter Content within that catalogue;</li>
                                                        </ol>
                                                    </li>
                                                    <li>not to download or copy any of the Content to electronic or photographic media;</li>
                                                    <li>not to reproduce any part of the Content by any means or under any format other than as a reasonable aid to your personal study;</li>
                                                    <li>not to reproduce, duplicate, copy or re-sell any Content in contravention of the provisions of this Acceptable Use Policy; and</li>
                                                    <li>not to use tools that automatically perform actions on your behalf;</li>
                                                    <li>not to upload any content that infringes the intellectual property rights, privacy rights or any other rights of any person or organisation; and</li>
                                                    <li>not to attempt to disguise your identity or that of your organisation;</li>
                                                    <li>
                                                        not to access without authority, interfere with, damage or disrupt:
                                                        <ol class=""nhsuk-u-padding-top-2"">
                                                            <li>any part of the Platform;</li>
                                                            <li>any equipment or network on which the Platform is stored;</li>
                                                            <li>any software used in the provision of the Platform;</li>
                                                            <li>the server on which the Platform is stored;</li>
                                                            <li>any computer or database connected to the Platform; or</li>
                                                            <li>any equipment or network or software owned or used by any third party.</li>
                                                        </ol>
                                                    </li>
                                                    <li>not to attack the Platform via a denial-of-service attack or a distributed denial-of-service attack.</li>
                                                </ol>
                                            </li>
                                        </ol>
                                    </li>
                                    <li class=""h2 nhsuk-heading-l nhsuk-u-margin-0"">
                                        Content standards
                                        <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-4 nhsuk-u-padding-left-0"">
                                            <li>The content standards set out in this paragraph 4 (Content Standards) apply to any and all Contributions. </li>
                                            <li>The Content Standards must be complied with in spirit as well as to the letter. The Content Standards apply to each part of any Contribution as well as to its whole. </li>
                                            <li>We will determine, in our discretion, whether a Contribution breaches the Content Standards.</li>
                                            <li>
                                                A Contribution must:
                                                <ol class=""nhsuk-u-padding-top-2"">
                                                    <li>be accurate (where it states facts);</li>
                                                    <li>be genuinely held (where it states opinions); and</li>
                                                    <li>comply with the law applicable in England and Wales and in any country from which it is posted.</li>
                                                </ol>
                                            </li>
                                            <li>
                                                A Contribution must not:
                                                <ol class=""nhsuk-u-padding-top-2"">
                                                    <li>
                                                        contain misinformation that is likely to harm users, patients/service users, health and care workers, or the general public’s wellbeing, safety, trust and reputation, including the reputation of the NHS or any part of it. This could include false and misleading information relating to disease prevention and treatment, conspiracy theories, content that encourages discrimination, harassment or physical violence, content originating from misinformation campaigns, and content edited or manipulated in such a way as to constitute misinformation;
                                                    </li>
                                                    <li>
                                                        contain any content or link to any content:
                                                        <ol class=""nhsuk-u-padding-top-2"">
                                                            <li>which is created for advertising, promotional or other commercial purposes, including links, logos and business names;</li>
                                                            <li>which requires a subscription or payment to gain access to such content;</li>
                                                            <li>in which the user has a commercial interest;</li>
                                                            <li>which promotes a business name and/or logo;</li>
                                                            <li>which contains a link to an app via iOS or Google Play; or</li>
                                                            <li>which has as its purpose or effect the collection and sharing of personal data;</li>
                                                        </ol>
                                                    </li>
                                                    <li>
                                                        be irrelevant to the purpose or aims of the Platform or while addressing relevant subject matter, contain an irrelevant, unsuitable or inappropriate slant (for example relating to potentially controversial opinions or beliefs of any kind intended to influence others);
                                                    </li>
                                                    <li> be defamatory of any person;</li>
                                                    <li> be obscene, offensive, hateful or inflammatory, or contain any profanity;</li>
                                                    <li> bully, insult, intimidate or humiliate;</li>
                                                    <li>
                                                        encourage suicide, substance abuse, eating disorders or other acts of self-harm.Content related to self - harm for the purposes of therapy, education and the promotion of general wellbeing may be uploaded, but we reserve the right to make changes to the way in which it is accessed in order that users do not view it accidentally;
                                                    </li>
                                                    <li>
                                                        feature sexual imagery purely intended to stimulate sexual arousal. Non - pornographic content relating to sexual health and related issues, surgical procedures and the results of surgical procedures, breastfeeding, therapy, education and the promotion of general wellbeing may be uploaded, but we reserve the right to make changes to the way in which it is accessed in order that users do not view it accidentally;
                                                    </li>
                                                    <li>
                                                        include child sexual abuse material. Content relating to safeguarding which addresses the subject of child sexual abuse may be uploaded, but we reserve the right to make changes to the way in which it is accessed in order that users do not view it accidentally;
                                                    </li>
                                                    <li>
                                                        incite or glorify violence including content designed principally for the purposes of causing reactions of shock or disgust;
                                                    </li>
                                                    <li>
                                                        promote discrimination or discriminate in respect of the protected characteristics set out in the Equality Act 2010, being age, disability, gender reassignment, marriage and civil partnership, pregnancy and maternity, race, nationality, religion or belief, sex, and sexual orientation;
                                                    </li>
                                                    <li>infringe any copyright, database right or trade mark of any other person;</li>
                                                    <li>be likely to deceive any person;</li>
                                                    <li>breach any legal duty owed to a third party, such as a contractual duty or a duty of confidence;</li>
                                                    <li>
                                                        promote any illegal content or activity, including but not limited to the encouragement, promotion, justification, praise or provision of aid to dangerous persons or organisations, including extremists, terrorists and terrorist organisations and those engaged in any form of criminal activity;
                                                    </li>
                                                    <li>be in contempt of court;</li>
                                                    <li>
                                                        be threatening, abuse or invade another''s privacy, or cause annoyance, inconvenience or needless anxiety;
                                                    </li>
                                                    <li>be likely to harass, bully, shame, degrade, upset, embarrass, alarm or annoy any other person;</li>
                                                    <li>impersonate any person or misrepresent your identity or affiliation with any person;</li>
                                                    <li>
                                                        advocate, promote, incite any party to commit, or assist any unlawful or criminal act such as (by way of example only) copyright infringement or computer misuse;
                                                    </li>
                                                    <li>
                                                        contain a statement which you know or believe, or have reasonable grounds for believing, that members of the public to whom the statement is, or is to be, published are likely to understand as a direct or indirect encouragement or other inducement to the commission, preparation or instigation of acts of terrorism;
                                                    </li>
                                                    <li>contain harmful material;</li>
                                                    <li>give the impression that the Contribution emanates from us, if this is not the case; or</li>
                                                    <li>disclose any third party’s confidential information, identity, personally identifiable information or personal data (including data concerning health).</li>
                                                </ol>
                                            </li>
                                            <li>You acknowledge and accept that, when using the Platform and accessing the Content, some Content that has been uploaded by third parties may be factually inaccurate, or the topics addressed by the Content may be offensive, indecent, or objectionable in nature. We are not responsible (legally or otherwise) for any claim you may have in relation to the Content.</li>
                                            <li>When producing Content to upload to the Platform, we encourage you to implement NICE guideline recommendations and ensure that sources of evidence are valid (for example, by peer review).</li>
                                        </ol>
                                    </li>
                                    <li class=""h2 nhsuk-heading-l"">
                                        Metadata
                                        <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-4 nhsuk-u-padding-left-0"">
                                            When making any Contribution, you must where prompted include a sufficient description of the Content so that other users can understand the description, source, and age of the Content. For example, if Content has been quality assured, then the relevant information should be posted in the appropriate field. All metadata fields on the Platform must be completed appropriately before initiating upload. Including the correct information is important in order to help other users locate the Content (otherwise the Content may not appear in search results for others to select).
                                        </ol>
                                    </li>
                                    <li class=""h2 nhsuk-heading-l nhsuk-u-padding-bottom-2 nhsuk-u-margin-0"">
                                        Updates
                                        <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-4 nhsuk-u-padding-left-0"">
                                            You must update each Contribution at least once every 3 (three) years, or update or remove it should it cease to be relevant or become outdated or revealed or generally perceived to be unsafe or otherwise unsuitable for inclusion on the Platform.
                                        </ol>
                                    </li>
                                    <li class=""h2 nhsuk-heading-l nhsuk-u-padding-bottom-2 nhsuk-u-margin-0"">
                                        Accessibility
                                        <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-4 nhsuk-u-padding-left-0"">
                                            Where practicable, all Contributions should aim to meet the accessibility standards as described in our Accessibility Statement
                                            [<a class=""policy-word-break"" href=""https://www.dls.nhs.uk/v2/LearningSolutions/AccessibilityHelp"">https://www.dls.nhs.uk/v2/LearningSolutions/AccessibilityHelp</a>] and as set out in the AA Standard Web Content Accessibility Guidelines v2.1 found here:
                                            <a class=""policy-word-break"" href=""https://www.w3.org/TR/WCAG21/"">https://www.w3.org/TR/WCAG21/</a>.
                                        </ol>
                                    </li>
                                    <li class=""h2 nhsuk-heading-l nhsuk-u-padding-bottom-2 nhsuk-u-margin-0"">
                                        Rules about linking to the Platform
                                        <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-4 nhsuk-u-padding-left-0"">
                                            <li>The Platform must not be framed on any other site. </li>
                                            <li>You may directly link to any Content that is hosted on the Platform, however, please be aware that not all links will continue to be available indefinitely. We will use our best efforts to ensure that all links are valid at the time of creating the related Content but cannot be held responsible for any subsequent changes to the link address or related Content.</li>
                                        </ol>
                                    </li>
                                    <li class=""h2 nhsuk-heading-l nhsuk-u-padding-bottom-2 nhsuk-u-margin-0"">
                                        No text or data mining, or web scraping
                                        <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-4 nhsuk-u-padding-left-0"">
                                            <li>
                                                You shall not conduct, facilitate, authorize or permit any text or data mining or web scraping in relation to the Platform or any services provided via, or in relation to, the Platform. This includes using (or permitting, authorizing or attempting the use of):
                                                <ol class=""nhsuk-u-padding-top-2"">
                                                    <li>any ""robot"", ""bot"", ""spider"", ""scraper"" or other automated device, program, tool, algorithm, code, process or methodology to access, obtain, copy, monitor or republish any portion of the Platform or any data, Content, information or services accessed via the same; and/or</li>
                                                    <li>any automated analytical technique aimed at analyzing text and data in digital form to generate information which includes but is not limited to patterns, trends, and correlations.</li>
                                                </ol>
                                            </li>
                                            <li>The provisions in this paragraph should be treated as an express reservation of our rights in this regard, including for the purposes of Article 4(3) of Digital Copyright Directive ((EU) 2019/790).</li>
                                            <li>This paragraph shall not apply insofar as (but only to the extent that) we are unable to exclude or limit text or data mining or web scraping activity by contract under the laws which are applicable to us.</li>
                                        </ol>
                                    </li>
                                    <li class=""h2 nhsuk-heading-l nhsuk-u-padding-bottom-2 nhsuk-u-margin-0"">
                                        Breach of this Acceptable Use Policy
                                        <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-4 nhsuk-u-padding-bottom-2 nhsuk-u-padding-left-0"">
                                            Failure to comply with this Acceptable Use Policy constitutes a material breach of this Acceptable Use Policy upon which you are permitted to use the Platform and may result in our taking all or any of the following actions:
                                            <li class=""nhsuk-u-padding-top-2 nhsuk-u-padding-bottom-2"">immediate, temporary, or permanent withdrawal of your right to use the Platform;</li>
                                            <li>immediate, temporary, or permanent removal of any Contribution uploaded by you to the Platform;</li>
                                            <li>issue of a warning to you;</li>
                                            <li>legal proceedings against you for reimbursement of all costs on an indemnity basis (including, but not limited to, reasonable administrative and legal costs) resulting from the breach, and/or further legal action against you;</li>
                                            <li>disclosure of such information to law enforcement authorities as we reasonably feel is necessary or as required by law; and/or</li>
                                            <li>any other action we reasonably deem appropriate.</li>
                                        </ol>
                                    </li>
                                </ol>";

            Execute.Sql(@"INSERT INTO [dbo].[Config] ([ConfigName], [ConfigText], [IsHtml],[CreatedDate],[UpdatedDate])
                            VALUES (N'AcceptableUse',N'" + acceptableUsePolicy + "',1, GETDATE(), GETDATE())");
        }
        public override void Down()
        {
            Execute.Sql(@"DELETE FROM Config
`                           WHERE ConfigName = N'AcceptableUse'");
        }
    }
}
