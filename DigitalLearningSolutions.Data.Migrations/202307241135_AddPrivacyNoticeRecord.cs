using FluentMigrator;
namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202307241135)]
    public class AddPrivacyNoticeRecord:Migration
    {
        public override void Up()
        {
            var PrivacyPolicy = @"<h1 class=""policy-text-center"">PRIVACY NOTICE</h1>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                            This page explains our privacy policy and how we will use and protect any information about you that you give to us or that we collate when you visit this website, or undertake employment with NHS England (<b>NHSE</b> or <b>we/us/our</b>), or participate in any NHSE sponsored training, education and development including via any of our training platform websites (<b>Training</b>).
                                           </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                            This privacy notice is intended to provide transparency regarding what personal data NHSE may hold about you, how it will be processed and stored, how long it will be retained and who may have access to your data.
                                           </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                            Personal data is any information relating to an identified or identifiable living person (known as the data subject). An identifiable person is one who can be identified, directly or indirectly, in particular by reference to an identifier such as a name, an identification number or factors specific to the physical, genetic or mental identity of that person, for example.
                                           </ol>
                                     <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">1	OUR ROLE IN THE NHS</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                            We are here to improve the quality of healthcare for the people and patients of England through education, training and lifelong development of staff and appropriate planning of the workforce required to deliver healthcare services in England.
                                           </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                            We aim to enable high quality, effective, compassionate care and to identify the right people with the right skills and the right values. All the information we collect is to support these objectives.
                                           </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">2	IMPORTANT INFORMATION </h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                            NHSE is the data controller in respect of any personal data it holds concerning trainees in Training, individuals employed by NHSE and individuals accessing NHSE’s website.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                            We have appointed a data protection officer (<b>DPO</b>) who is responsible for overseeing questions in relation to this privacy policy.  If you have any questions about this privacy policy or our privacy practices, want to know more about how your information will be used, or make a request to exercise your legal rights, please contact our DPO in the following ways:
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                            <b>Name</b>: Andrew Todd
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                            <b>Email address</b>: <a class=""policy-word-break"" href=""mailto:gdpr@hee.nhs.uk"">gdpr@hee.nhs.uk</a>
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                            <b>Postal address</b>: NHS England of Skipton House, 80 London Road, London SE1 6LH
                                            </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">3	WHAT THIS PRIVACY STATEMENT COVERS</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                            This privacy statement only covers the processing of personal data by NHSE that NHSE has obtained from data subjects accessing any of NHSE’s websites and from its provision of services and exercise of functions. It does not cover the processing of data by any sites that can be linked to or from NHSE’s websites, so you should always be aware when you are moving to another site and read the privacy statement on that website.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           When providing NHSE with any of your personal data for the first time, for example, when you take up an appointment with NHSE or when you register for any Training, you will be asked to confirm that you have read and accepted the terms of this privacy statement. A copy of your acknowledgement will be retained for reference.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           If our privacy policy changes in any way, we will place an updated version on this page. Regularly reviewing the page ensures you are always aware of what information we collect, how we use it and under what circumstances, if any, we will share it with other parties.
                                            </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">4	WHY NHSE COLLECTS YOUR PERSONAL DATA</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                            Personal data may be collected from you via the recruitment process, when you register and/or create an account for any Training, during your Annual Review of Competence Progression or via NHSE’s appraisal process. Personal data may also be obtained from Local Education Providers or employing organisations in connection with the functions of NHSE.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           Your personal data is collected and processed for the purposes of and in connection with the functions that NHSE performs with regard to Training and planning. The collection and processing of such data is necessary for the purposes of those functions.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           A full copy of our notification to the Information Commissioner’s Office (<b>ICO</b>) (the UK regulator for data protection issues), can be found on the ICO website here: <a class=""policy-word-break"" href=""http://www.ico.org.uk"">www.ico.org.uk</a> by searching NHSE’s ICO registration number, which is Z2950066.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           In connection with Training, NHSE collects and uses your personal information for the following purposes:
                                            </ol>
                    
                                    <ol class=""nhsuk-u-padding-top-2 nhsuk-u-padding-left-8"">
                                            <li>to manage your Training and programme, including allowing you to access your own learning history;</li>
                                            <li>to quality assure Training programmes and ensure that standards are maintained, including gathering feedback or input on the service, content, or layout of the Training and customising the content and/or layout of the Training;</li>
                                            <li>to identify workforce planning targets;</li>
                                            <li>to maintain patient safety through the management of performance concerns;</li>
                                            <li>to comply with legal and regulatory responsibilities including revalidation;</li>
                                            <li>to contact you about Training updates, opportunities, events, surveys and information that may be of interest to you;</li>
                                            <li>transferring your Training activity records for programmes to other organisations involved in medical training in the healthcare sector. These organisations include professional bodies that you may be a member of, such as a medical royal college or foundation school; or employing organisations, such as trusts;</li>
                                            <li>making your Training activity records visible to specific named individuals, such as tutors, to allow tutors to view their trainees’ activity. We would seek your explicit consent before authorising anyone else to view your records;</li>
                                            <li>providing anonymous, summarised data to partner organisations, such as professional bodies; or local organisations, such as strategic health authorities or trusts;</li>
                                            <li>for NHSE internal review;</li>
                                            <li>to provide HR related support services and Training to you, for clinical professional learner recruitment;</li>
                                            <li>to promote our services;</li>
                                            <li>to monitor our own accounts and records;</li>
                                            <li>to monitor our work, to report on progress made; and</li>
                                            <li>to let us fulfil our statutory obligations and statutory returns as set by the Department of Health and the law (for example complying with NHSE’s legal obligations and regulatory responsibilities under employment law).</li>
                                        </ol>
                                     <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           Further information about our use of your personal data in connection with Training can be found in ’<i>A Reference Guide for Postgraduate Foundation and Specialty Training in the UK</i>’, published by the Conference of Postgraduate Medical Deans of the United Kingdom and known as the ‘<i>Gold Guide</i>’, which can be found here: <a class=""policy-word-break"" href=""https://www.copmed.org.uk/gold-guide"">https://www.copmed.org.uk/gold-guide</a>.
                                            </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">5	TYPES OF PERSONAL DATA COLLECTED BY NHSE</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           The personal data that NHSE collects when you register for Training enables the creation of an accurate user profile/account, which is necessary for reporting purposes and to offer Training that is relevant to your needs.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                         The personal data that is stored by NHSE is limited to information relating to your work, such as your job role, place of work, and membership number for a professional body (e.g. your General Medical Council number). NHSE will never ask for your home address or any other domestic information.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           When accessing Training, you will be asked to set up some security questions, which may contain personal information. These questions enable you to log in if you forget your password and will never be used for any other purpose. The answers that you submit when setting up these security questions are encrypted in the database so no one can view what has been entered, not even NHSE administrators.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          NHSE also store a record of some Training activity, including upload and download of Training content, posts on forums or other communication media, and all enquires to the service desks that support the Training.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           If you do not provide personal data that we need from you when requested, we may not be able to provide services (such as Training) to you. In this case, we may have to cancel such service, but we will notify you at the time if this happens.
                                            </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">6	COOKIES</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           When you access NHSE’s website and Training, we want to make them easy, useful and reliable.  This sometimes involves placing small amounts of limited information on your device (such as your computer or mobile phone).  These small files are known as cookies, and we ask you to agree to their usage in accordance with ICO guidance.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          These cookies are used to improve the services (including the Training) we provide you through, for example:
                                            </ol>
                                    <ol class=""nhsuk-u-padding-top-2 nhsuk-u-padding-left-8"">
                                            <li>enabling a service to recognise your device, so you do not have to give the same information several times during one task (e.g. we use a cookie to remember your username if you check the ’<i>Remember Me</i>’ box on a log in page);</li>
                                            <li>recognising that you may already have given a username and password, so you do not need to do it for every web page requested;</li>
                                            <li>measuring how many people are using services, so they can be made easier to use and there is enough capacity to ensure they are fast; and</li>
                                            <li>analysing anonymised data to help us understand how people interact with services so we can make them better.</li>
                                           </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          We use a series of cookies to monitor website speed and usage, as well as to ensure that any preferences you have selected previously are the same when you return to our website.  Please visit our cookie policies page to understand the cookies that we use: <a class=""policy-word-break"" href=""https://www.dls.nhs.uk/v2/CookieConsent/CookiePolicy"">https://www.dls.nhs.uk/v2/CookieConsent/CookiePolicy</a>
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          Most cookies applied when accessing Training are used to keep track of your input when filling in online forms, known as session-ID cookies, which are exempt from needing consent as they are deemed essential for using the website or Training they apply to.  Some cookies, like those used to measure how you use the Training, are not needed for our website to work.  These cookies can help us improve the Training, but we’ll only use them if you say it’s OK. We’ll use a cookie to save your settings.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          On a number of pages on our website or Training, we use ’plug-ins’ or embedded media. For example, we might embed YouTube videos. Where we have used this type of content, the suppliers of these services may also set cookies on your device when you visit their pages. These are known as ’third-party’ cookies. To opt-out of third-parties collecting any data regarding your interaction on our website, please refer to their websites for further information.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          We will not use cookies to collect personal data about you. However, if you wish to restrict or block the cookies which are set by our websites or Training, or indeed any other website, you can do this through your browser settings. The ’Help’ function within your browser should tell you how. Alternatively, you may wish to visit <a class=""policy-word-break"" href=""http://www.aboutcookies.org"">www.aboutcookies.org</a> which contains comprehensive information on how to do this on a wide variety of browsers. You will also find details on how to delete cookies from your machine as well as more general information about cookies. Please be aware that restricting cookies may impact on the functionality of our website.
                                            </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">7           LEGAL BASIS FOR PROCESSING</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           The retained EU law version of the General Data Protection Regulation ((EU) 2016/679) (<b>UK GDPR</b>) requires that data controllers and organisations that process personal data demonstrate compliance with its provisions. This involves publishing our basis for lawful processing of personal data.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          As personal data is processed for the purposes of NHSE’s statutory functions, NHSE’s legal bases for the processing of personal data as listed in Article 6 of the UK GDPR are as follows:
                                            </ol>
                                    <ul class=""nhsuk-u-padding-left-8"">
	                                      <li>6(1)(a) – Consent of the data subject</li>
	                                      <li>6(1)(b) – Processing is necessary for the performance of a contract to which the data subject is party or in order to take steps at the request of the data subject prior to entering into a contract</li>
	                                      <li>6(1)(c) – Processing is necessary for compliance with a legal obligation</li>
	                                      <li>6(1)(e) – Processing is necessary for the performance of a task carried out in the public interest or in the exercise of official authority vested in the controller</li>
	                                    </ul>
                                     <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          Where NHSE processes special categories of personal data, its additional legal bases for processing such data as listed in Article 9 of the UK GDPR are as follows:
                                            </ol>
                                    <ul class=""nhsuk-u-padding-left-8"">
	                                      <li>9(2)(a) – Explicit consent of the data subject</li>
	                                      <li>9(2)(b) – Processing is necessary for the purposes of carrying out the obligations and exercising specific rights of the controller or of the data subject in the field of employment and social security and social protection law</li>
	                                      <li>9(2)(f) – Processing is necessary for the establishment, exercise or defence of legal claims or whenever courts are acting in their judicial capacity</li>
	                                      <li>9(2)(g) – Processing is necessary for reasons of substantial public interest</li>
                                          <li>9(2)(h) – Processing is necessary for the purposes of occupational medicine, for the assessment of the working capacity of the employee, or the management of health and social care systems and services</li>
                                          <li>9(2)(j) – Processing is necessary for archiving purposes in the public interest, scientific or historical research purposes or statistical purposes</li>
	                                    </ul>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          Special categories of personal data include data relating to racial or ethnic origin, political opinions, religious beliefs, sexual orientation and data concerning health.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          Please note that not all of the above legal bases will apply for each type of processing activity that NHSE may undertake. However, when processing any personal data for any particular purpose, one or more of the above legal bases will apply.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          We may seek your consent for some processing activities, for example for sending out invitations to you to Training events and sending out material from other government agencies. If you do not give consent for us to use your data for these purposes, we will not use your data for these purposes, but your data may still be retained by us and used by us for other processing activities based on the above lawful conditions for processing set out above.
                                            </ol>
                                     <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">8	INFORMATION THAT WE MAY NEED TO SEND YOU</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           We may occasionally have to send you information from NHSE, the Department of Health, other public authorities and government agencies about matters of policy where those policy issues impact on Training, workforce planning, or other matters related to NHSE. This is because NHSE is required by statute to exercise functions of the Secretary of State in respect of Training and workforce planning. If you prefer, you can opt out of receiving information about general matters of policy impacting on Training and workforce planning by contacting your Local Office recruitment lead or <a class=""policy-word-break"" href=""mailto:tel@hee.nhs.uk"">tel@hee.nhs.uk</a>. The relevant Local Office or a representative from the relevant training platform website will provide you with further advice and guidance regarding any consequences of your request.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          NHSE will not send you generic information from other public authorities and government agencies on issues of government policy.
                                            </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">9	TRANSFERS ABROAD</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           The UK GDPR imposes restrictions on the transfer of personal data outside the European Union, to third countries or international organisations, in order to ensure that the level of protection of individuals afforded by the UK GDPR is not undermined.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          Your data may only be transferred abroad where NHSE is assured that a third country, a territory or one or more specific sectors in the third country, or an international organisation ensures an adequate level of protection.
                                            </ol>
                                     <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">10	HOW WE PROTECT YOUR PERSONAL DATA</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           Our processing of all personal data complies with the UK GDPR principles.  We have put in place appropriate security measures to prevent your personal data from being accidentally lost, used or accessed in an unauthorised way, altered or disclosed.  The security of the data is assured through the implementation of NHSE’s policies on information governance management.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          The personal data we hold may be held as an electronic record on data systems managed by NHSE, or as a paper record. These records are only accessed, seen and used in the following circumstances:
                                            </ol>
                                     <ol class=""nhsuk-u-padding-top-2 nhsuk-u-padding-left-8"">
                                            <li>if required and/or permitted by law; or </li>
                                            <li>by NHSE staff who need access to them so they can do their jobs and who are subject to a duty of confidentiality; or </li>
                                            <li>by other partner organisations, including our suppliers, who have been asked to sign appropriate non-disclosure or data sharing agreements and will never be allowed to use the information for commercial purposes.</li>
                                           </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          We make every effort to keep your personal information accurate and up to date, but in some cases we are reliant on you as the data subject to notify us of any necessary changes to your personal data. If you tell us of any changes in your circumstances, we can update the records with personal data you choose to share with us.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           Information collected by NHSE will never be sold for financial gain or shared with other organisations for commercial purposes.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           We have put in place procedures to deal with any suspected personal data breach and will notify you and any applicable regulator of a breach where we are legally required to do so.
                                            </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">11	SHARING PERSONAL DATA</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           So we can provide the right services at the right level, we may share your personal data within services across NHSE and with other third party organisations such as the Department of Health, higher education institutions, clinical placement providers, colleges, faculties, other NHSE Local Offices, the General Medical Council, NHS Trusts/Health Boards/Health and Social Care Trusts, approved academic researchers and other NHS and government agencies where necessary, to provide the best possible Training and to ensure that we discharge NHSEs responsibilities for employment and workforce planning for the NHS. This will be on a legitimate need to know basis only.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          We may also share information, where necessary, to prevent, detect or assist in the investigation of fraud or criminal activity, to assist in the administration of justice, for the purposes of seeking legal advice or exercising or defending legal rights or as otherwise required by the law.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          Where the data is used for analysis and publication by a recipient or third party, any publication will be on an anonymous basis, and will not make it possible to identify any individual. This will mean that the data ceases to become personal data.
                                            </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">12	HOW LONG WE RETAIN YOUR PERSONAL DATA</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           We will keep personal data for no longer than necessary to fulfil the purposes we collected it for, in accordance with our records management policy and the NHS records retention schedule within the NHS Records Management Code of Practice at: <a class=""policy-word-break"" href=""https://transform.england.nhs.uk/information-governance/guidance/records-management-code/records-management-code-of-practice-2021"">https://transform.england.nhs.uk/information-governance/guidance/records-management-code/records-management-code-of-practice-2021</a> (as may be amended from time to time).
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          In some circumstances you can ask us to delete your data.  Please see the “Your rights” section below for further information.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          In some circumstances we will anonymise your personal data (so that it can no longer be associated with you) for research or statistical purposes, in which case we may use this information indefinitely without further notice to you.
                                            </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">13	OPEN DATA</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                           Open data is data that is published by central government, local authorities and public bodies to help you build products and services.  NHSE policy is to observe the Cabinet Office transparency and accountability commitments towards more open use of public data in accordance with relevant and applicable UK legislation. 
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          NHSE would never share personal data through the open data facility. To this end, NHSE will implement information governance protocols that reflect the ICO’s recommended best practice for record anonymisation, and Office of National Statistics guidance on publication of statistical information.
                                            </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">14	YOUR RIGHTS</h3>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">14.1	Right to rectification and erasure</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          Under the UK GDPR you have the right to rectification of inaccurate personal data and the right to request the erasure of your personal data. However, the right to erasure is not an absolute right and it may be that it is necessary for NHSE to continue to process your personal data for a number of lawful and legitimate reasons.
                                            </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">14.2	Right to object and withdraw your consent</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          You have the right in certain circumstances to ask NHSE to stop processing your personal data in relation to any NHSE service. As set out above, you can decide that you do not wish to receive information from NHSE about matters of policy affecting Training and workforce. However, the right to object is not an absolute right and it may be that it is necessary in certain circumstances for NHSE to continue to process your personal data for a number of lawful and legitimate reasons.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          If you object to the way in which NHSE is processing your personal information or if you wish to ask NHSE to stop processing your personal data, please contact your relevant Local Office.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          Please note, if we do stop processing personal data about you, this may prevent NHSE from providing the best possible service to you.  Withdrawing your consent will result in your Training account being anonymised and access to the Training removed.
                                            </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">14.3	Right to request access</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          You can access a copy of the information NHSE holds about you by writing to NHSE’s Public and Parliamentary Accountability Team. This information is generally available to you free of charge subject to the receipt of appropriate identification.  More information about subject access requests can be found here: <a class=""policy-word-break"" href=""https://www.hee.nhs.uk/about/contact-us/subject-access-request"">https://www.hee.nhs.uk/about/contact-us/subject-access-request</a>.
                                            </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">14.4	Right to request a transfer</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          The UK GDPR sets out the right for a data subject to have their personal data ported from one controller to another on request in certain circumstances. You should discuss any request for this with your Local Office.  This right only applies to automated information which you initially provided consent for us to use or where we used the information to perform a contract with you.
                                            </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">14.5	Right to restrict processing</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          You can ask us to suspend the processing of your personal data if you want us to establish the data’s accuracy, where our use of the data is unlawful but you do not want us to erase it, where you need us to hold the data even if we no longer require it as you need it to establish, exercise or defend legal claims or where you have objected to our use of your data but we need to verify whether we have overriding legitimate grounds to use it.
                                            </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">14.6	Complaints</h3>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          You have the right to make a complaint at any time to the ICO. We would, however, appreciate the chance to deal with your concerns before you approach the ICO so please contact your Local Office or the DPO in the first instance, using the contact details above.
                                            </ol>
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          You can contact the ICO at the following address:
                                            </ol>
                                    
                                    <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                        <p>The Office of the Information Commissioner<br>Wycliffe House<br>Water Lane<br>Wilmslow<br>Cheshire<br>SK9 5AF</p>
                                           </ol>
                                    <h3 class=""nhsuk-heading-m nhsuk-u-margin-left-0"">14.7	Your responsibilities</h3>
                                     <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          It is important that you work with us to ensure that the information we hold about you is accurate and up to date so please inform NHSE if any of your personal data needs to be updated or corrected.
                                            </ol>
                                     <ol class=""h4 nhsuk-body-m nhsuk-u-padding-top-1 nhsuk-u-padding-left-0"">
                                          All communications from NHSE will normally be by email. It is therefore essential for you to maintain an effective and secure email address, or you may not receive information or other important news and information about your employment or Training.
                                            </ol>
                                   ";
            Execute.Sql(@"INSERT INTO [dbo].[Config] ([ConfigName], [ConfigText], [IsHtml],[CreatedDate],[UpdatedDate])
                            VALUES (N'PrivacyPolicy',N'" + PrivacyPolicy + "',1, GETDATE(), GETDATE())");
        }
        public override void Down()
        {
            Execute.Sql(@"DELETE FROM Config
`                           WHERE ConfigName = N'PrivacyPolicy'");
        }
    }
}
