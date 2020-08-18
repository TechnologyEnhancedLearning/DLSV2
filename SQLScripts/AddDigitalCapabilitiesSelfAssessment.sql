BEGIN TRANSACTION

SET IDENTITY_INSERT SelfAssessments ON
INSERT INTO SelfAssessments (Id, Name, Description)
    VALUES (1, 'Digital Capability Self Assessment', 'When thinking about your current role, for each of the following statements rate your current confidence level (Where are you now) and where your confidence leve ought to be to undertake your role successfully (Where do you need to be). Once you have submitted your ratings they will be used to recommend useful learning resources. We will also collect data anonymously to build up a picture of digital capability across the workforce to help with service design and learning provision.')
SET IDENTITY_INSERT SelfAssessments OFF

SET IDENTITY_INSERT CompetencyGroups ON
INSERT INTO CompetencyGroups (Id, Name) VALUES
    (1, 'Data, information and content'),
    (2, 'Teaching, learning and self-development'),
    (3, 'Communication, collaboration and participation'),
    (4, 'Technical proficiency'),
    (5, 'Creation, innovation and research'),
    (6, 'Digital identity, wellbeing, safety and security'),
    (7, 'General questions')
SET IDENTITY_INSERT CompetencyGroups OFF

SET IDENTITY_INSERT AssessmentQuestions ON
INSERT INTO AssessmentQuestions (Id, Question, MaxValueDescription, MinValueDescription) VALUES
    (1, 'Where are you now', 'Very confident', 'Beginner'),
    (2, 'Where do you need to be', 'Very confident', 'Beginner'),
    (3, 'To what extent to you agree', 'Strongly agree', 'Strongly disagree')
SET IDENTITY_INSERT AssessmentQuestions OFF
    
SET IDENTITY_INSERT Competencies ON
INSERT INTO Competencies (Id, Description, CompetencyGroupId) VALUES
    (1, 'I can find, use and store information that exists in different digital locations e.g. on a PC, shared drives, via the internet', 1),
    (2, 'I understand and stick to guidelines and regulations when using data and information to make sure of security and confidentiality requirements', 1),
    (3, 'I’m able to judge how credible and trustworthy sources of data and information are', 1),
    (4, 'I champion and lead on the use of data and information, by myself and others, to inform everything we do', 1),
    (5, 'I can collect useful resources and store or share them online (e.g. bookmarks, pins, tagging, posting to forums etc)', 1),

    (6, 'I can use a wide range of digital technologies to help me learn (e.g. e-learning, YouTube, podcasts, online courses, MOOCs)', 2),
    (7, 'I’m good at looking for new ways to learn using digital means and I encourage and support others to do the same', 2),
    (8, 'I can create lots of different online/digital resources/tools that support other people’s learning', 2),
    (9, 'I know how to record my digital or online learning so that I can review, reflect, track my progress and make plans', 2),
    (10, 'I can participate in or deliver online learning sessions (e.g. forums, podcasts, webinars, video tutorials)', 2),

    (11, 'I can connect online regularly with people I wouldn’t know otherwise', 3),
    (12, 'I can use a wide range of digital tools (e.g. Webex, Skype, Slack, email, WhatsApp) to communicate appropriately, always bearing in mind who I’m communicating with and what the purpose is', 3),
    (13, 'I am able to support others in building and maintain online/digital collaboratives spaces where we can work on shared goals', 3),
    (14, 'I post to my own or a professional/shared blog', 3),
    (15, 'I’m always careful and encourage others to be careful in communicating in ways that are respectful of different needs, expectations, cultures and experiences', 3),

    (16, 'I am able to learn, use and update a wide range of devices and applications (e.g. smartphones, PCs, tablets and a range of software like Word, Microsoft 365, Excel, Trello, Evernote)', 4),
    (17, 'I can usually resolve a range of day-to-day technical challenges and issues with my devices, software and applications on my own without help', 4),
    (18, 'I have a wide range of technical skills and I like to keep up to date with digital technology and innovation', 4),
    (19, 'I can carry out regular technical tasks like changing passwords, updating or installing applications (apps) etc', 4),
    (20, 'I can help others with technical issues', 4),

    (21, 'I use a range of digital devices, technologies and software to create or edit new online or offline content (e.g. photo editing, film creation, social media messages, blogging)', 5),
    (22, 'I’m happy to try new applications (apps) and see new opportunities for digital solutions', 5),
    (23, 'I lead on and support others in the appropriate and creative use of digital in research, audit, quality improvement and/or research activities', 5),
    (24, 'I see digital and other technologies as an opportunity to challenge and improve the current way of doing things so that we can deliver better outcomes and I see myself as part of that', 5),
    (25, 'I protect and promote my own and my organisation’s digital content (e.g. website content, written reports, presentations etc) and respect the intellectual property of others', 5),

    (26, 'I know how to create and protect different digital identities online (i.e. I know that I might have different identity on LinkedIn and Facebook, I know how to protect my own and/or my organisation’s reputation online; I can present myself and my organisation in appropriate ways online)', 6),
    (27, 'I use digital technologies in ways that support my own good health and wellbeing and that of others (i.e. I know when to ‘switch off’; I tackle any bullying or undermining behaviour online; I use apps to support wellbeing e.g. MyFitnessPal, mindfulness apps etc)', 6),
    (28, 'I recognise and act on any breaches of safety or security rules and guidelines', 6),
    (29, 'I use digital technology to manage and reduce my impact on the environment (e.g. not printing unnecessarily; avoiding travel when virtual collaboration is possible)', 6),
    (30, 'I act on any behaviours that compromise respectful, appropriate, professional interactions and support others to do the same', 6),

    (31, 'I am the person responsible for taking care of my own digital literacy learning', 7),
    (32, 'Taking an active role in my own learning is the most important thing that affects my digital literacy skills development', 7)
SET IDENTITY_INSERT Competencies OFF

INSERT INTO CompetencyAssessmentQuestions (CompetencyID, AssessmentQuestionID) VALUES
    (1, 1), (1, 2),
    (2, 1), (2, 2),
    (3, 1), (3, 2),
    (4, 1), (4, 2),
    (5, 1), (5, 2),
    (6, 1), (6, 2),
    (7, 1), (7, 2),
    (8, 1), (8, 2),
    (9, 1), (9, 2),
    (10, 1), (10, 2),
    (11, 1), (11, 2),
    (12, 1), (12, 2),
    (13, 1), (13, 2),
    (14, 1), (14, 2),
    (15, 1), (15, 2),
    (16, 1), (16, 2),
    (17, 1), (17, 2),
    (18, 1), (18, 2),
    (19, 1), (19, 2),
    (20, 1), (20, 2),
    (21, 1), (21, 2),
    (22, 1), (22, 2),
    (23, 1), (23, 2),
    (24, 1), (24, 2),
    (25, 1), (25, 2),
    (26, 1), (26, 2),
    (27, 1), (27, 2),
    (28, 1), (28, 2),
    (29, 1), (29, 2),
    (30, 1), (30, 2),
    (31, 3), (32, 3)

INSERT INTO SelfAssessmentStructure (SelfAssessmentID, CompetencyID) VALUES
    (1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (1, 7), (1, 8), (1, 9), (1, 10),
    (1, 11), (1, 12), (1, 13), (1, 14), (1, 15), (1, 16), (1, 17), (1, 18), (1, 19), (1, 20),
    (1, 21), (1, 22), (1, 23), (1, 24), (1, 25), (1, 26), (1, 27), (1, 28), (1, 29), (1, 30),
    (1, 31), (1, 32)

INSERT INTO CandidateAssessments (CandidateID, SelfAssessmentID) VALUES (254480, 1)

COMMIT
