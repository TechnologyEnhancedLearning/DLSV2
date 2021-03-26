SET IDENTITY_INSERT [dbo].[Brands] ON 
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName], [BrandDescription], [BrandImage], [ImageFileType], [IncludeOnLanding], [ContactEmail], [OwnerOrganisationID], [Active], [OrderByNumber], [BrandLogo], [PopularityHigh]) VALUES (10, N'HEE Learning Disabilities and Autism', NULL, NULL, NULL, 0, NULL, 101, 1, 0, NULL, 100)
GO
SET IDENTITY_INSERT [dbo].[Brands] OFF
GO
SET IDENTITY_INSERT [dbo].[Frameworks] ON 
GO
INSERT [dbo].[Frameworks] ([ID], [BrandID], [CategoryID], [TopicID], [FrameworkName], [Description], [FrameworkConfig], [OwnerAdminID], [CreatedDate], [PublishStatusID], [UpdatedByAdminID]) VALUES (1050, 10, 1, 1, N'Core Capabilities Framework for Supporting Autistic People (Tier 1)', NULL, NULL, 1, CAST(N'2021-01-11T15:49:36.037' AS DateTime), 1, 1)
GO
INSERT [dbo].[Frameworks] ([ID], [BrandID], [CategoryID], [TopicID], [FrameworkName], [Description], [FrameworkConfig], [OwnerAdminID], [CreatedDate], [PublishStatusID], [UpdatedByAdminID]) VALUES (1051, 10, 1, 1, N'Core Capabilities Framework for Supporting Autistic People (Tier 2)', NULL, NULL, 1, CAST(N'2021-01-11T16:01:06.437' AS DateTime), 1, 1)
GO
SET IDENTITY_INSERT [dbo].[Frameworks] OFF
GO
SET IDENTITY_INSERT [dbo].[CompetencyGroups] ON 
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (101, N'Understanding autism', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (102, N'Personalised support', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (103, N'Physical and mental health', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (104, N'Risk, legislation and safeguarding', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (105, N'Leadership and management, education and research', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (106, N'Identification, assessment and diagnosis of autism', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (107, N'Person-centred care and support', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (108, N'Communication and interaction', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (109, N'Sensory processing and the environment', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (110, N'Families and carers as partners in care and support', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (111, N'Supporting changes throughout life', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (112, N'Supporting autistic people where behaviour may challenge', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (113, N'Forensic support', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (114, N'Relationships, sexuality and sexual health', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (115, N'Meaningful activity and independence', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (116, N'Physical health', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (117, N'Mental health', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (118, N'Health equality and reasonable adjustments', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (119, N'Law, ethics and safeguarding', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (120, N'Equality, diversity and inclusion', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (121, N'Leadership and management', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (122, N'Education and personal development', 1)
GO
INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (123, N'Research and evidence-based practice', 1)
GO
SET IDENTITY_INSERT [dbo].[CompetencyGroups] OFF
GO
SET IDENTITY_INSERT [dbo].[FrameworkCompetencyGroups] ON 
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (100, 101, 1, 1, 1050)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (101, 101, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (102, 102, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (103, 103, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (104, 104, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (105, 105, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (106, 106, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (107, 107, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (108, 108, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (109, 109, 9, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (110, 110, 10, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (111, 111, 11, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (112, 112, 12, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (113, 113, 13, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (114, 114, 14, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (115, 115, 15, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (116, 116, 16, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (117, 117, 17, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (118, 118, 18, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (119, 119, 19, 1, 1051)
GO
SET IDENTITY_INSERT [dbo].[FrameworkCompetencyGroups] OFF
GO
SET IDENTITY_INSERT [dbo].[Competencies] ON 
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (58, N'<ul><li>how common it is</li><li>that autism is neurodevelopmental and life long</li><li>that every autistic person has a different combination of traits and sensitivities and is unique.</li></ul>', 1, N'Know basic facts about autism')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (59, NULL, 1, N'Be able to use respectful terminology')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (60, N'<p>For example:</p><ul><li>at home</li><li>in the classroom</li><li>in care settings</li><li>the community.</li></ul>', 1, N'Understand what common autistic characteristics may look like in real life situations')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (61, N'<ul><li>understand that communication includes both giving and receiving information and the importance of actively including autistic people, regardless of their ability to communicate verbally</li><li>be aware how autistic people may become overwhelmed and need time and quiet space to process and understand information. For example, when overwhelmed some people may ‘shutdown’ or simply acquiesce to anything said in order to bring the experience to a close; therefore, you will not get accurate information and they will not retain what you have said</li><li>be aware of (e.g. through reading their communication passport) and respect the different methods of communication that an autistic person may use</li><li>actively listen to what a person is ‘saying’ and be prepared to use patience and perseverance in communication – including being silent to allow thinking time</li><li>be aware that processing and understanding spoken language is a challenge for many autistic people, especially when anxious or in difficult sensory environments</li><li>be aware that autistic people often take language literally so it is important to use clear, unambiguous language, responding positively when autistic people use direct language and give direct feedback</li><li>be aware of difficulties and differences in non-verbal communication e.g. facial expression; eye contact; and personal distance</li></ul>', 1, N'Take responsibility for meeting an autistic person’s unique communication and information needs')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (62, NULL, 1, N'Be aware that behaviour seen as challenging may be a form of communication or an indication of distress')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (63, N'<p>For example:</p><ul><li>processing time</li><li>difficulties with small talk</li><li>social rules</li><li>understanding and interpreting emotions.</li></ul>', 1, N'Recognise some key differences in social interaction')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (64, NULL, 1, N'Be aware that autistic people may live with other physical or mental health conditions or impairments that will also impact on their lives')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (65, NULL, 1, N'Understand the role of trauma in the lives of autistic people leading to a wide range of mental health problems in later life and the importance of building trust and making choices for recovery')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (66, N'<p>For example:</p><ul><li>oversensitivity or under-sensitivity to lighting, sound, temperature, touch, smell</li><li>how anxiety and stress can contribute to sensory tolerance.</li></ul>', 1, N'Understand how sensory issues can impact on autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (67, N'<p>For example:</p><ul><li>turning off unnecessary lights, TV / radio</li><li>offering quiet space</li><li>enabling the use of sensory protection such as noise-cancelling headphones.</li></ul>', 1, N'Be able to make simple changes to ensure an environment is accessible to autistic people, including opportunities to avoid sensory overload and consider the use of an alternative location')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (68, NULL, 1, N'Plan changes in advance whenever possible and provide preparation and information about upcoming events using a variety of communication methods')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (69, NULL, 1, N'Recognise the importance of passionate interests and hobbies')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (70, N'<p>For example:</p><ul><li>Don’t spring surprises!</li><li>Don’t touch without consent</li><li>Slow down and pause</li><li>Create or find a calm, quiet environment</li><li>Explain FIRST, THEN do.</li></ul>', 1, N'Be able to consistently put key adaptations into practice')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (71, NULL, 1, N'Be aware of the Equality Act 2010, Human Rights Act 1998 and Mental Capacity Act 2005')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (72, NULL, 1, N'Know where to access resources and further information about autism')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (73, NULL, 1, N'Understand that the spectrum of autism consists of a range of both abilities and disabilities, many of which may not be obvious')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (74, NULL, 1, N'Understand that the spectrum of autism consists of a range of both abilities and disabilities, many of which may not be obvious')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (75, NULL, 1, N'Know the importance of equal, timely access to autism assessment and diagnosis and some of the barriers to diagnosis')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (76, NULL, 1, N'Be able to identify practical strategies to offer person-centred support to autistic individuals in a range of day to day situations')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (77, N'<p>Including:</p><ul><li> using visual information (photos, diagrams, symbols)</li><li>use of IT</li><li>autism alert cards</li><li>written information (e.g. text or email) </li></ul><p>when this works for the individual.</p>', 1, N'Be able to identify simple adjustments which can be made to meet the communication needs and preferences of autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (78, NULL, 1, N'Avoid the tendency to underestimate the capabilities of less verbal individuals and overestimate the capabilities of those who are more verbal')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (79, NULL, 1, N'Understand how behaviour may indicate stress and avoid assumptions about what a person’s behaviour may be trying to communicate')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (80, NULL, 1, N'Understand that any change in a person’s presentation or behaviour may be a sign of a health or emotional problem, distress or sensory overload. Do not assume it is simply an inevitable part of autism even if it presents differently than in other people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (81, NULL, 1, N'Understand stimming, including why it can be helpful as a form of expression and where to seek support if it seems to be becoming harmful')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (82, NULL, 1, N'Understand activities that people adopt to ‘self soothe’ or ‘self-regulate’ or just ‘calm down’ and make sure people can do these things when they need to')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (83, NULL, 1, N'Be able to execute practical strategies to support autistic people with changes, such as preparation and providing clear information - and support the autistic person through change to understand the situation through appropriate means')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (84, NULL, 1, N'Understand the importance of working with others, including the role that family carers and supporters may play in the lives of autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (85, NULL, 1, N'Know how to access further support within one’s own organisation to ensure the needs of autistic people are met')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (86, NULL, 1, N'Understand how health inequality affects autistic people and be aware of the main causes of health inequality for autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (87, NULL, 1, N'Be able to identify and put in place ‘reasonable adjustments’ in access to health care and other services')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (88, NULL, 1, N'Be aware of the role of health action plans/health passports in signposting important adjustments')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (89, NULL, 1, N'Be aware of everyday issues commonly faced by autistic people such as anxiety, fear, depression, stress, low self-esteem etc and how these may be reduced')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (90, NULL, 1, N'Understand that autistic people can develop mental health conditions for the same reasons as people without autism, but that the prevalence of mental health conditions in autistic people is higher due to the impact of factors such as social inequality, isolation, stigma and discrimination')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (91, NULL, 1, N'Understand that autistic people have a right to be supported to make their own decisions and must be given all practicable help before anyone concludes that they cannot make a decision')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (92, N'<p>For example, through:</p><ul><li> social isolation</li><li>bullying</li><li>social misunderstandings</li></ul>', 1, N'Be aware of how views and attitudes of others can impact on the lives of autistic people and their families and carers')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (93, NULL, 1, N'Be aware of types of abuse which may be especially relevant for autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (94, NULL, 1, N'Be aware of one’s own responsibilities under the Equality Act, Human Rights Act and Mental Capacity Act')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (95, NULL, 1, N'Recognise the indicators of autism that would signal the need for further assessment, and conditions which may co-occur with autism')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (96, NULL, 1, N'Be aware of some of the key differences between learning disability, autism, mental health conditions and learning difficulties – and understand that individuals may experience more than one of these, or other neurodevelopmental conditions at the same time')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (97, NULL, 1, N'Promote equal access to autism diagnostic assessment, recognising that there may be some girls and women who present with less traditionally obvious characteristics, leading to them historically being excluded from assessment and diagnosis')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (98, NULL, 1, N'Recognise that autistic people can be misdiagnosed and those who display less traditionally obvious characteristics may be masking their difficulties, which can be a barrier to diagnosis and that this may occur in relation to women and girls, men and boys, as well as those who are gender fluid or non-binary')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (99, NULL, 1, N'Know why timely identification of autism is important and the likely outcomes if assessment for diagnosis is delayed')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (100, NULL, 1, N'Explain the benefits of an assessment for diagnosis of autism with sensitivity and in a way that is appropriate to the autistic person and their families and carers')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (101, NULL, 1, N'Avoid the tendency to underestimate the capabilities of less verbal individuals and overestimate the capabilities of those who are more verbal, recognising that receptive and expressive language may affect a person''s ability to engage in conversation/interaction')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (102, NULL, 1, N'Be aware of relevant specialist services and support networks locally and nationally and appropriately refer autistic people to them')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (103, NULL, 1, N'Effectively engage with both the autistic person themselves and with families and carers providing care and support')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (104, NULL, 1, N'Understand how to use and adapt care and support approaches in a person-centred way to meet the needs of autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (105, NULL, 1, N'Facilitate and seek to increase an autistic person’s choice and control over major life decisions in addition to everyday choices, whenever possible, and recognising difficulties autistic people may have with choice')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (106, NULL, 1, N'Understand the important role of family and carers in person-centred thinking and planning')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (107, NULL, 1, N'Assess and plan for the needs of families and carers providing care and support for an autistic person')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (108, NULL, 1, N'Be aware of statutory rights to independent advocacy and make or support referrals as appropriate')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (109, NULL, 1, N'Communicate clearly and straightforwardly about the care and support needs of autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (110, NULL, 1, N'Contribute to gathering information about a person’s strengths, needs and preferences for their person-centred plan')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (111, NULL, 1, N'Schedule and measure progress towards goals important to the autistic person')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (112, NULL, 1, N'Evaluate the extent to which each autistic adult wants and benefits from activities such as social interaction; constant activity; community participation; and other outcomes which may prioritise ’a normal life’ over wellbeing and reflect this in person-centred care plans')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (113, N'<p>This may include providing autistic children and young people with a range of experiences to develop their interests and skills.</p>', 1, N'Recognise that whilst encouraging new experiences that an autistic person might like based on their preferences is advocated, they may prefer repetition and routine rather than unpredictability')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (114, NULL, 1, N'Provide care and support to an autistic person in the ways identified in their person-centred plan')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (115, NULL, 1, N'Understand the principles and follow the practice of co-production')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (116, NULL, 1, N'Understand the importance of an autistic person getting the support they need to make choices and decisions and to increase their skills and experience of doing so, accounting for their age and ability')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (117, NULL, 1, N'Support and facilitate the development of a person’s autistic identity, including gender identity and access to autistic culture and autistic space')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (118, NULL, 1, N'Understand the role of positive risk taking in enabling a person-centred approach and enabling new experiences based on the person’s preferences')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (125, NULL, 1, N'Understand that effective communication, in all areas of life, is critical for supporting the autonomy, wellbeing and quality of life of autistic people and continue to support the development of functional communication throughout the lifespan')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (126, NULL, 1, N'Advocate for communication adjustments for autistic people when accessing non-specialist and community services')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (127, N'<p>For example:</p><ul><li>written information (including text and email)</li><li> signing</li><li>symbol-based communication</li><li>assistive technology</li><li>the appropriate (and inappropriate) use of touch</li></ul>', 1, N'Understand and promote the role of non-verbal communication and provide access to non-verbal means of communicating whenever appropriate')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (128, NULL, 1, N'Ensure provision of information is specific and clear – avoiding ambiguities')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (129, NULL, 1, N'Understand the importance of providing time and space for autistic people to process and understand information and to make and communicate decisions')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (130, NULL, 1, N'Recognise behaviour can be a form of communication and avoid assumptions about the meanings that can be attached to behaviour')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (131, NULL, 1, N'Understand the importance of being able to communicate basic needs to reduce frustration')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (132, NULL, 1, N'Use a range of communication techniques26 to convey information, according to the different abilities and preferences of autistic people, recognising that each autistic person may have a unique way of communicating')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (133, NULL, 1, N'Understand why individualised communication plans should be developed, implemented and reviewed with the autistic person, avoiding assumptions which may over or under-estimate an autistic person’s ability to communicate')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (134, NULL, 1, N'Recognise the impact of the environment and sensory needs on communication – knowing how to find the right time, place and situation for important communications')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (135, NULL, 1, N'Understand the importance of and promote effective communication with families and carers. Recognise the expertise that families and carers may be able to offer to support effective communication with the autistic person')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (136, NULL, 1, N'Communicate effectively with colleagues using a variety of media formats (e.g. verbal, written and digital) and in accordance with legal requirements')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (137, NULL, 1, N'Signpost to appropriate specialist speech and language therapy advice and intervention and make timely referrals where appropriate')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (138, NULL, 1, N'Be aware of and support the legal frameworks (Equality Act, Mental Capacity Act and Accessible Information Standard) to make adjustments to all forms of care, treatment, communication and information')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (139, NULL, 1, N'Understand that not every autistic person will have the same level of over-or under-sensory sensitivity, or indeed any sensory sensitivity in some areas and that each person’s tolerance of sensory stimuli will vary according to other factors and over time')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (140, NULL, 1, N'Create environments to support autistic people and understand how to adjust environments to enable areas to be inclusive and welcoming to everyone')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (141, N'<p>&nbsp;Such as:</p><ul><li> noise cancelling headphones</li><li>sunglasses</li><li>dimmable lights etc</li></ul>', 1, N'Recognise the importance of accessible quiet spaces and the autistic person’s right to take a break if that is their choice - and acceptance/provision of adaptations')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (142, N'<p>For example:</p><ul><li>making sure that assessments, meetings, interviews, appointments are planned for</li><li>asking the question about any adjustments that might be required to the environment</li></ul>', 1, N'Utilise the expertise of autistic people to identify sensory issues and in developing proactive approaches to the environment')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (143, NULL, 1, N'Be able to recognise sensory overload and know how to respond')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (144, NULL, 1, N'Acknowledge, understand and encourage sensory stimulation behaviours (e.g. ‘stimming’), intervening only on the basis of an agreed plan led by the autistic person if they are suffering distress or harm, or where intervention may be legally required (e.g. as a duty of care)')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (145, NULL, 1, N'Seek to provide access to sensory stimulation opportunities appropriate to individual needs and include these in care plans where appropriate')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (146, NULL, 1, N'Understand that some autistic people have a high pain threshold and may not report injuries or illness, even when severe')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (147, NULL, 1, N'Understand and promote the significance and value of families, carers and social networks (where an autistic person has and wishes to involve these) in planning and providing care and support for autistic people, including in Best Interests decision making')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (148, NULL, 1, N'Establish and maintain positive relationships with families and carers and understand the importance of discussing when and how they would like to be involved in the person’s care or support and when and how the autistic person would like them to be involved in their care or support')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (149, NULL, 1, N'Recognise the positive and negative impact that caring for an autistic person in the family may have on relationships and family members’ own wellbeing, including the possibility of parents, carers, and/or family members being autistic themselves and be able to signpost to carer assessments as appropriate')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (150, NULL, 1, N'Understand the importance of providing information and advice and where appropriate, training, to families and carers. Utilise their expertise (in addition to that of autistic people) in developing training')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (151, N'<p>This may include:</p><ul><li> providing sensitive and balanced support for parents coming to terms with their child’s differences or who are under great stress</li><li>parents who struggle to accept an autistic person’s adulthood and those whose focus is on curing or eliminating autistic traits</li></ul>', 1, N'Work with families to enable them to access support in their role as carers')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (152, NULL, 1, N'Recognise that the needs and wishes of family members and/or carers may not be the same as the needs and wishes of the autistic person')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (153, NULL, 1, N'Be aware of and responsive to each families’ own culture, traditions and style of interaction')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (154, NULL, 1, N'Contribute to the development of practices and services that actively reflect the culture, wishes and needs of families and carers')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (155, NULL, 1, N'Support people to express their personal preferences and anxieties when going through change and adapt support methods to take account of their views, using a person-centred approach')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (156, NULL, 1, N'Support autistic people to develop and maintain routines, structure and systems to create order in ways that are helpful to them')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (157, NULL, 1, N'Recognise that uncertainty and unpredictability cause stress and provide clear information that reduces stress')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (158, NULL, 1, N'Be honest with people about transitions, recognising that withholding potentially upsetting information may worsen rather than alleviate distress')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (159, NULL, 1, N'Recognise and provide support for the challenges an autistic person may face with moving from childhood into adulthood, including carrying out timely transition assessments')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (160, N'<p>For example, practical and emotional support may relate to:</p><ul><li> finances</li><li>education</li><li>employment</li><li>accommodation</li><li>retirement</li><li>bereavement</li><li>palliative or end of life care</li></ul>', 1, N'Provide information, advice and support for autistic people, their families and carers to enable involvement, choice and control at times of change')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (161, NULL, 1, N'Be aware that timely counselling and coaching can prevent escalation of the difficulties people may face')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (162, NULL, 1, N'Identify recent or imminent changes affecting autistic people and support them to assess the implications and likely impacts of the change identified, recognising that even "minor" transitions can be significant')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (163, NULL, 1, N'Evaluate the impact of age-related changes on older autistic people, including dementia and frailty, and how services can best meet their needs')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (164, NULL, 1, N'Recognise the importance of identifying and assessing the changing health and social care needs of autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (165, NULL, 1, N'Provide accessible information, advice and support which is tailored to an individual''s communication needs')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (166, NULL, 1, N'Identify how and when to seek additional expertise and advice when supporting a person through change')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (167, NULL, 1, N'Understand and support the important role families and carers have in supporting autistic people where behaviour may challenge, in addition to the full involvement of the person themselves')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (168, NULL, 1, N'Recognise that behaviour perceived as challenging is not an inevitable part of autism but may be a possible indication of distress and focus on the removal of communication barriers, environmental and other stressors as a priority over modification of the behaviour')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (169, NULL, 1, N'Recognise that the actions of staff and carers can increase or reduce the likelihood of behaviour which challenges')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (170, NULL, 1, N'Consult the autistic person and their family/support regarding what causes distress or anxiety in order to understand and respect an autistic person’s perspective')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (171, NULL, 1, N'Support autistic people to have a ‘plan B’ or a range of responses should things become difficult, including contingency planning which seeks the person’s advance wishes')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (172, NULL, 1, N'Be proactive in de-escalating situations, for example, preventing ‘meltdown’ or ‘shutdown’ by facilitating ways to alleviate stress (such as access to a quiet space or ending an interaction).')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (173, NULL, 1, N'Be able to accept, understand and accommodate behaviours that are unique to the autistic person and which do not infringe the rights or safety of others')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (174, NULL, 1, N'Understand stimming, including promoting it when it can be helpful and know where to seek support if it becomes harmful')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (175, NULL, 1, N'Be able to apply strategies to manage the risk of serious aggression and self-injurious behaviour')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (176, N'<p>Including:</p><ul><li>physical health problems</li><li>pain and exposure to sources of stress including:</li><ul><li>uncertainty</li><li>change</li><li>interaction</li></ul><li>communication</li><li>sensory overload and demands</li></ul>', 1, N'Understand the interactions between the quality of a person’s life and behaviours that may be interpreted as ‘behaviours that challenge’')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (177, NULL, 1, N'Understand that autistic people may be at increased risk of misuse of restrictive practices, including physical and chemical restraint and seek to provide care and support in ways which avoid and prevent any need for restrictive practices')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (178, NULL, 1, N'Be able to follow a behaviour support plan in the wider context of an overall care plan; according to specified responsibilities and timeframes')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (179, NULL, 1, N'Understand when the support of a ‘specialist’ might be needed and how to access them, avoiding assumptions and labels such as ‘unpredictable’, ‘complex’ or ‘challenging’')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (180, NULL, 1, N'Confidently identify, advocate for and implement reasonable adjustments for autistic people within the Criminal Justice System')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (181, NULL, 1, N'Be aware of the likelihood of past, present and future trauma and risk of victimisation of autistic people and recognise vulnerability and needs, alongside any risks of offending behaviour')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (182, NULL, 1, N'Carry out a risk assessment, relevant to the autistic person and integrate risk assessment into the planning and provision of care, clearly distinguishing between offending behaviour and autistic behaviour which does not infringe the rights of others')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (183, NULL, 1, N'Work to assess and manage risk in conjunction with the multi-disciplinary team, in a multi-agency environment, including helping to identify and monitor factors (both internal and external to the autistic person) which indicate increased or reduced risk')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (184, NULL, 1, N'Contribute to the formulation of crisis and emergency plans, including supporting the development of an autistic person’s own coping strategies')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (185, N'<p>For example:</p><ul><li>sex and relationships education</li><li>social understanding and skills including support to recognise exploitation</li><li>emotional regulation skills</li><li>positive ways to meet sensory needs</li></ul>', 1, N'Contribute to the identification of and provision to meet educational and support needs relevant to offending behaviour')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (186, NULL, 1, N'Be aware of good practice and know how to adapt approaches to working with autistic offenders')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (187, NULL, 1, N'Manage actual or potential aggression in line with current legal requirements and best practice')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (188, NULL, 1, N'Respond to an autistic person’s communication needs and recognise the roles of unmet communication needs and miscommunication in risky and challenging behaviour')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (189, NULL, 1, N'Contribute to the assessment and modifying of offender rehabilitation and treatment programmes to make sure that they meet the communication needs of autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (190, NULL, 1, N'Recognise the impact of the autistic person’s activities on family and friends')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (191, NULL, 1, N'Recognise the impact of any victim considerations and Ministry of Justice restrictions')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (192, NULL, 1, N'Identify early signs of relapse and crisis and how to articulate this in relation to the autistic person and their family')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (193, NULL, 1, N'Recognise own emotional response to the autistic person’s risk factors and actions and use techniques to minimise the impact on the service and the person themselves')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (194, NULL, 1, N'Support autistic people to maintain their relationships with family members and other people in their social network and to develop new friendships and relationships, if they wish to do so')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (195, NULL, 1, N'Support and facilitate the delivery of person-centred/age appropriate and autism specific sex and relationships education across the lifespan, including support in recognising healthy and unhealthy relationships and online risks and issues')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (196, NULL, 1, N'Understand how to support people to say no to unwanted relationships including when they may be at risk of ‘mate crime’ or ‘cuckooing’, including recognising the role of public services in taking appropriate actions directed towards perpetrators of abuse to prevent and intervene in criminal activity')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (197, NULL, 1, N'Be able to support the sexual expression of an autistic adult, including LGBTQ+ issues, using appropriate person-centred (including age-appropriate) approaches')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (198, NULL, 1, N'Know how to support autistic young people and adults to access and use help, advice or services to meet their sexual health needs as appropriate, including the importance of preventive education and healthcare')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (199, NULL, 1, N'Be aware of how views and attitudes of others can impact on the lives of autistic people and their families and carers')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (200, NULL, 1, N'Know how to support autistic people to develop and continue their interests, social life and community involvement and know why this is important')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (201, NULL, 1, N'Recognise and encourage the importance of passionate interests and hobbies')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (202, NULL, 1, N'Support autistic people to develop and retain skills for everyday life, including practical tasks, decision making and positive risk taking, accounting for age')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (203, NULL, 1, N'Provide support for autistic people to manage their finances and maximise their capacity to make their own financial decisions')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (204, NULL, 1, N'Be able to support autistic people to maximise their control over their own support, including through the use of direct payments')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (205, N'<p>For example: </p><ul><li>housing</li><li>transport </li><li>leisure services</li></ul>', 1, N'Support autistic people to choose and use professional services and facilities and decide how long to use them for')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (206, NULL, 1, N'Understand the rights of an autistic person in relation to reasonable adjustments when accessing and using services and support them to advocate for adjustments and adaptations as necessary')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (207, NULL, 1, N'Understand the value of engagement in education, training, employment or meaningful occupation for autistic people and their potential to contribute to society')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (208, NULL, 1, N'Be aware of the support available to autistic people to access education, training and employment')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (209, NULL, 1, N'Understand factors that impact on autistic people being able to navigate their physical and social environment, including discrimination, bullying and hate crime')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (210, N'<p>For example:</p><ul><li>Google Maps</li><li>Skype</li><li>Apps for task planning</li><li>Calendars</li><li>Online shopping etc.</li></ul>', 1, N'Be aware of how everyday technology can be used to enable autistic people to choose and use the full range of social interaction available in a way which works for them')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (211, NULL, 1, N'Recognise and respond to the cultural, religious and spiritual needs of autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (212, NULL, 1, N'Support autistic people to draw on their strengths to manage setbacks and personal difficulties')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (213, NULL, 1, N'Understand the importance of a positive, person-centred approach to risk and how this is supported by the legal framework')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (214, N'<p>For example:</p><ul><li>physical impairments</li><li>visual impairment</li><li>chromosome disorders</li><li>mental health conditions (including eating disorders)</li><li>epilepsy</li><li>other neurodevelopmental conditions such as ADHD</li></ul>', 1, N'Be aware that autistic people may live with other conditions or impairments that will also impact on their lives')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (215, NULL, 1, N'Recognise the signs, symptoms, prevalence and potential impact on the lives of autistic people of common health conditions')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (216, N'<p>For example:</p><ul><li>psychologists</li><li>speech and language therapists</li><li>optometrists</li><li>occupational therapists</li><li>dietitians and physiotherapists</li></ul>', 1, N'Know the function of different healthcare services that autistic people may need to access and the barriers autistic people may face in accessing them')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (217, NULL, 1, N'Suggest, encourage, support and promote healthy lifestyle options and make referral to services providing healthy lifestyle advice and options, including support to take up offers of general health screening, whilst also respecting the rights of autistic adults to make unwise choices on an equal basis with others in society')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (218, NULL, 1, N'Understand the benefits and risks of prescribed medication (including psychotropic medication) on the physical and mental health and the choices and rights of patients – including the potential for autistic people to have atypical reactions to medication')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (219, NULL, 1, N'Fulfil professional duties related to the safe administration of medication where appropriate, identifying and supporting with additional needs around taking medication (such as needing prompting)')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (220, NULL, 1, N'Understand the role of families and carers in supporting the health and wellbeing of autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (221, NULL, 1, N'Contribute to development of health action plans with autistic people, including identifying reasonable adjustments and unmet support needs')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (222, NULL, 1, N'Recognise the potential impact of sensory differences on the autistic person being able to recognise themselves when they are feeling unwell, and the potential for differences in interpreting pain sensations')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (223, NULL, 1, N'Refer autistic people to specialist healthcare services for assessment, diagnosis and support and ensure they are aware of any expected timeframes. Encourage them to persist in liaison with specialist services, providing support to do so when needed')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (224, NULL, 1, N'Support autistic people to make healthcare decisions including advocating for reasonable adjustments such as provision of accessible information and processing time')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (225, N'<p>For example:</p><ul><li>communication difficulties</li><li>anxiety</li><li>difficulties with initiative and/or their high pain threshold</li></ul>', 1, N'Understand that some autistic people may not report pain or seek help early due to a variety of factors')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (226, NULL, 1, N'Understand that mental health conditions are common and can be overlooked in autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (227, NULL, 1, N'Understand the role of trauma in the lives of autistic people which may lead to a wide range of mental health problems in later life and the importance of building trusting relationships and providing support to make choices to enable empowerment')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (228, NULL, 1, N'Recognise that mental health conditions may develop and present in different ways from non-autistic people and recognise signs or symptoms e.g. repeated self-harm or self-injurious behaviour')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (229, NULL, 1, N'Recognise that some forms of self-injurious behaviour (e.g. hitting oneself) may be an indication of distress in contrast to self-harm which may relate to a mental health condition')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (230, NULL, 1, N'Recognise when an autistic person may be experiencing mental distress, including suicidal thoughts and intentions')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (231, NULL, 1, N'Support autistic people to develop and maintain good mental health including lifestyle choices such as exercise, pets and social interaction of the person’s choice and through developing positive attitudes to autism')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (232, NULL, 1, N'Recognise that autistic people have a right to equitable access to treatment, including appropriate medication, whilst also recognising the issue of over-medication of autistic people and know how to address this')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (233, NULL, 1, N'Understand that autistic people with mental health needs may present with behaviour which may challenge, masking other difficulties such as with communication')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (234, NULL, 1, N'Create opportunities for autistic people to express their feelings, including feelings of loss, grief and bereavement, and anger and frustration, in ways which are meaningful to them')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (235, N'<p>Such as:</p><ul><li>occupational therapists</li><li>counsellors</li><li>speech and language therapists</li><li>psychologists</li><li>psychiatrists</li><li>mental health nurses</li></ul>', 1, N'Know the function of different mental health services that autistic people may need to access and where to refer an autistic person with a suspected mental health condition')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (236, NULL, 1, N'Coordinate and communicate with key people and services in the life of the autistic person and a mental health condition')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (237, NULL, 1, N'Understand the health inequalities commonly experienced by autistic people, including early mortality')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (238, NULL, 1, N'Understand the importance of access to appropriate healthcare for autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (239, N'<p>Including: </p><ul><li>diagnostic overshadowing</li><li>failure to follow legal duties in the Mental Capacity Act</li><li>inappropriate decisions not to treat or withhold lifesaving care</li><li>inaccessible information</li><li>lack of co-ordination of care</li><li>failure to make reasonable adjustments</li><li>failure to provide necessary support to carry out healthcare activities or follow advice</li></ul>', 1, N'Understand the key barriers that may prevent autistic people accessing appropriate healthcare')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (240, NULL, 1, N'Be aware of current legislation, policies and guidance relevant to autistic people accessing healthcare')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (241, NULL, 1, N'Understand how annual health checks and health action plans can underpin long term health and wellbeing for autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (242, NULL, 1, N'Know the importance of health passports, communication passports, health action plans, hospital traffic lights or hospital passports / books and how these can provide important information about a person’s communication and care needs and any potential hazards such as a risk of choking, known allergies and epilepsy – and how to interpret and use the information within them')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (243, NULL, 1, N'Identify a number of methods by which a person''s support needs may be flagged to healthcare providers, including additional information on summary care records (SCRs) and, for those with a learning disability, the GP Learning Disability Register')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (244, NULL, 1, N'Understand the unique roles that both health and social care professionals may play in the care and support of an autistic person and be aware of the importance of care co-ordination and working together')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (245, NULL, 1, N'Understand how limited, or unconventional, communication and poor health literacy may reduce the ability of autistic people to convey health needs effectively to others and the adjustments to practice, and support available, to overcome this')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (246, N'<p>Adjustments may include:</p><ul><li> consideration around appointment times, duration and support required</li><li>adaptations to decision making (e.g. providing information about medication and other treatments in accessible formats and giving processing time)</li></ul>', 1, N'Identify the need for, and implement, reasonable adjustments to enable the health needs of autistic people to be met')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (247, NULL, 1, N'Understand that decision-making in healthcare is a demand which can be very difficult for autistic people to cope with if not supported appropriately')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (248, N'<p>Including:</p><ol style="list-style-type: lower-roman;"><li> know how to find out if people have any information or communication needs and how to meet their needs</li><li>understand how to seek out information around communication needs and respond to flags or additional information provided within a person’s records or correspondence</li><li>understand how to share information about people’s communication needs with other providers of NHS and adult social care, and the legal basis for doing so</li></ol>', 1, N'Understand how to identify the need for, and provide accessible information, as required by the Accessible Information Standard, tailored to the communication needs of autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (249, NULL, 1, N'Understand how legislation and policies promote and protect the rights of autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (250, NULL, 1, N'Be aware of key legislation relevant to mental capacity, liberty protection safeguards, equality and human rights and differences between the rights of children and of adults')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (251, NULL, 1, N'Understand that autistic adults have the right to make their own decisions and that, while capacity may be questioned and assessed, an adult does not have to prove they are able to make a decision themselves. If a person thinks an adult may lack capacity to make a decision, then they must demonstrate why that is the case')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (252, NULL, 1, N'Understand that autistic adults may make what might be seen as eccentric or unwise decisions and that these, by themselves, do not necessary mean that they lack capacity to make their own decisions')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (253, NULL, 1, N'Understand what practical steps can be taken to support an autistic person with making a decision')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (254, NULL, 1, N'Be able to support a person to get advocacy to help them make and communicate a decision')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (255, NULL, 1, N'Understand that if a person is not capable of making a decision then anything done on their behalf must be in their best interests, in which their wishes and feelings and those of family/friends must be considered, and must be the least restrictive of their rights, in accordance with the Mental Capacity Act')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (256, NULL, 1, N'Understand that lacking capacity to make a decision at the time it needs to be made doesn’t mean that the person is not capable of making that decision another time or of making other decisions')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (257, NULL, 1, N'Know where to get advice to resolve tensions between duty of care and an autistic person’s and/or family and carers wishes. Be aware of the importance of human rights in resolving such dilemmas')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (258, NULL, 1, N'Communicate effectively about proposed care and support to enable an autistic person to make informed choices')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (259, NULL, 1, N'Recognise a range of signs and factors which may indicate that an autistic person is experiencing neglect, abuse or exploitation')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (260, NULL, 1, N'Know what to do if neglect, abuse, unsafe practices or exploitation is suspected, including how to raise concerns within local safeguarding or whistle blowing procedures')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (261, NULL, 1, N'Understand the national and local context of safeguarding and protection from abuse for autistic people, including ‘making safeguarding personal’')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (262, NULL, 1, N'Effectively support autistic people to disclose harm or abuse')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (263, NULL, 1, N'Understand ways to reduce the likelihood of abuse for autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (264, NULL, 1, N'Understand the risks associated with the internet and online social networking and balance these with rights to equality and freedom of expression in accordance with the law')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (265, NULL, 1, N'Actively challenge others who are not behaving in an ethical way')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (266, NULL, 1, N'Be aware of one’s own values and beliefs, including unconscious bias')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (267, N'<ol style="list-style-type: lower-roman;"><li>diversity</li><li>equality</li><li>inclusion</li><li>discrimination</li><li>ethnicity and religion</li></ol>', 1, N'Know what is meant by the following terms')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (268, NULL, 1, N'Know ways in which discrimination may deliberately or inadvertently occur in an autistic person’s local community (including social networking)')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (269, NULL, 1, N'Understand how practices that support equality and inclusion reduce the likelihood of discrimination')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (270, NULL, 1, N'Interact with people in ways that respect their beliefs, culture, values, preferences and right to equality with others, including where this does not involve treating people the same')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (271, NULL, 1, N'Know how to challenge discrimination in a way that encourages positive change')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (272, NULL, 1, N'Know who to ask for advice and support about equality and inclusion')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (273, NULL, 1, N'Adapt assessment, support and care planning taking account of equality issues (e.g. cultural diversity, disabilities, age, gender and sexual orientation), including autism-specific needs')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (274, NULL, 1, N'Recognise diversity in family arrangements and the local community')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (275, NULL, 1, N'Actively challenge any discriminatory practice that may compromise the right of an autistic person to dignity, respect, safety and equality')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (276, N'<p>Including one’s own responsibilities under:</p><ul><li> the Mental Capacity Act 2005, Mental Capacity (Amendment) Act 2019</li><li>the Equality Act 2010</li><li>the Care Act 2014</li><li>the Human Rights Act 1998</li><li>the Accessible Information Standard</li></ul>', 1, N'Be aware of the key legislation, policy and guidelines relating to autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (277, NULL, 1, N'Understand the features of effective team performance within care and support for autistic people, including consistency and clear communication of information')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (278, NULL, 1, N'Advocate for and practice co-production with autistic people and their families and carers')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (279, NULL, 1, N'Support a positive culture and shared vision within the team and with autistic people for autism care and support')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (280, NULL, 1, N'Support individual team members to work towards agreed objectives in care and support for autistic people, ensuring that these objectives are consistent with promoting the wellbeing and quality of life of autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (281, NULL, 1, N'Describe strategies and tools that could be adopted to reduce staff stress levels, to build resilience and to maintain the wellbeing of staff within the team')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (282, NULL, 1, N'Know the appropriate type and level of resources required to deliver safe and effective services in care and support for autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (283, NULL, 1, N'Understand the importance of continuing professional development')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (284, NULL, 1, N'Understand the process for agreeing a personal development plan and who should be involved')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (285, NULL, 1, N'Know why feedback from others is important in helping to develop and improve working practice')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (286, NULL, 1, N'Understand the principles of reflective practice and why it is important')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (287, NULL, 1, N'Understand the purpose and benefits of supervision and appraisal or similar arrangements')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (288, NULL, 1, N'Be aware of a range of learning opportunities and how they can be used')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (289, NULL, 1, N'Be able to implement a personal development plan through accessing development opportunities')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (290, NULL, 1, N'Use opportunities with others to reflect on learning in order to continuously improve practice')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (291, NULL, 1, N'Disseminate information about knowledge, evidence-based and legally literate practice that will be useful to others')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (292, NULL, 1, N'Challenge poor practice in ways that promote the use of evidence-based, legally and ethically sound practice to safeguard individuals and enhance their wellbeing')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (293, NULL, 1, N'Appreciate and utilise the lived expertise of autistic people and their families and carers in one’s own personal development')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (294, NULL, 1, N'Undertake personal development based on the changing needs of the individuals using the service to ensure that the service provided meets those needs')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (295, NULL, 1, N'Recognise the importance of research, evidence-based and legally literate practice and support for autistic people and their families and carers')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (296, NULL, 1, N'Know where to find research and development evidence (including the rich qualitative data available from people with lived experience) and up-to-date legal knowledge and how to use it to underpin ways of working to benefit autistic people')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (297, NULL, 1, N'Understand the strengths and weaknesses of different types of evidence')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (298, NULL, 1, N'Understand the importance of collecting quality assurance information including feedback from autistic people and their families and carers')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (299, NULL, 1, N'Be able to obtain and act on the feedback and experiences of autistic people and their families and carers including outcome measures such as quality of life')
GO
INSERT [dbo].[Competencies] ([ID], [Description], [UpdatedByAdminID], [Name]) VALUES (300, NULL, 1, N'Understand the importance of a research-active workforce')
GO
SET IDENTITY_INSERT [dbo].[Competencies] OFF
GO
SET IDENTITY_INSERT [dbo].[FrameworkCompetencies] ON 
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (33, 58, 100, 1, 1, 1050)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (34, 59, 100, 2, 1, 1050)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (35, 60, 100, 3, 1, 1050)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (36, 61, 100, 4, 1, 1050)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (37, 62, 100, 5, 1, 1050)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (38, 63, 100, 6, 1, 1050)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (39, 64, 100, 7, 1, 1050)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (40, 65, 100, 8, 1, 1050)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (41, 66, 100, 9, 1, 1050)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (42, 67, 100, 10, 1, 1050)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (43, 68, 100, 11, 1, 1050)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (44, 69, 100, 12, 1, 1050)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (45, 70, 100, 13, 1, 1050)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (46, 71, 100, 14, 1, 1050)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (47, 72, 100, 15, 1, 1050)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (49, 73, 101, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (50, 75, 101, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (51, 76, 101, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (52, 77, 101, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (53, 78, 101, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (54, 79, 101, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (55, 80, 101, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (56, 81, 101, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (57, 82, 101, 9, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (58, 83, 101, 10, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (59, 84, 101, 11, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (60, 85, 101, 12, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (61, 86, 101, 13, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (62, 87, 101, 14, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (63, 88, 101, 15, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (64, 89, 101, 16, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (65, 90, 101, 17, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (66, 91, 101, 18, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (67, 92, 101, 19, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (68, 93, 101, 20, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (69, 94, 101, 21, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (70, 95, 102, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (71, 96, 102, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (72, 97, 102, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (73, 98, 102, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (74, 99, 102, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (75, 100, 102, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (76, 101, 102, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (77, 102, 102, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (78, 103, 102, 9, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (79, 104, 103, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (80, 105, 103, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (81, 106, 103, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (82, 107, 103, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (83, 108, 103, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (84, 109, 103, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (85, 110, 103, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (86, 111, 103, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (87, 112, 103, 9, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (88, 113, 103, 10, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (89, 114, 103, 11, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (90, 115, 103, 12, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (91, 116, 103, 13, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (92, 117, 103, 14, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (93, 118, 103, 15, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (103, 125, 104, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (104, 126, 104, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (105, 127, 104, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (106, 128, 104, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (107, 129, 104, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (108, 130, 104, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (109, 131, 104, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (110, 132, 104, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (111, 133, 104, 9, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (112, 134, 104, 10, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (113, 135, 104, 11, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (114, 136, 104, 12, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (115, 137, 104, 13, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (116, 138, 104, 14, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (117, 139, 105, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (118, 140, 105, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (119, 141, 105, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (120, 142, 105, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (121, 143, 105, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (122, 144, 105, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (123, 145, 105, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (124, 146, 105, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (125, 147, 106, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (126, 148, 106, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1106, 149, 106, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (128, 150, 106, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (129, 151, 106, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (130, 152, 106, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (131, 153, 106, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (132, 154, 106, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (133, 155, 107, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (134, 156, 107, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (135, 157, 107, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (136, 158, 107, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (137, 159, 107, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (138, 160, 107, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (139, 161, 107, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (140, 162, 107, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (141, 163, 107, 9, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (142, 164, 107, 10, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (143, 165, 107, 11, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (144, 166, 107, 12, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (145, 167, 108, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (146, 168, 108, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (147, 169, 108, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (148, 170, 108, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (149, 171, 108, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (150, 172, 108, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (151, 173, 108, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (152, 174, 108, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (153, 175, 108, 9, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (154, 176, 108, 10, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (155, 177, 108, 11, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (156, 178, 108, 12, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (157, 179, 108, 13, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (158, 180, 109, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (159, 181, 109, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (160, 182, 109, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (161, 183, 109, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (162, 184, 109, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (163, 185, 109, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (164, 186, 109, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (165, 187, 109, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (166, 188, 109, 9, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (167, 189, 109, 10, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (168, 190, 109, 11, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (169, 191, 109, 12, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (170, 192, 109, 13, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (171, 193, 109, 14, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (172, 194, 110, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (173, 195, 110, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (174, 196, 110, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (175, 197, 110, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (176, 198, 110, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (177, 199, 111, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (178, 200, 111, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (179, 201, 111, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (180, 202, 111, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (181, 203, 111, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (182, 204, 111, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (183, 205, 111, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (184, 206, 111, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (185, 207, 111, 9, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (186, 208, 111, 10, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (187, 209, 111, 11, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (188, 210, 111, 12, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (189, 211, 111, 13, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (190, 212, 111, 14, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (191, 213, 111, 15, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (192, 214, 112, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (193, 215, 112, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (194, 216, 112, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (195, 217, 112, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (196, 218, 112, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (197, 219, 112, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (198, 220, 112, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (199, 221, 112, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (200, 222, 112, 9, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (201, 223, 112, 10, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (202, 224, 112, 11, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (203, 225, 112, 12, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (204, 226, 113, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (205, 227, 113, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (206, 228, 113, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (207, 229, 113, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (208, 230, 113, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (209, 231, 113, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (210, 232, 113, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (211, 233, 113, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (212, 234, 113, 9, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (213, 235, 113, 10, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (214, 236, 113, 11, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (215, 237, 114, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (216, 238, 114, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (217, 239, 114, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (218, 240, 114, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (219, 241, 114, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (220, 242, 114, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (221, 243, 114, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (222, 244, 114, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (223, 245, 114, 9, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (224, 246, 114, 10, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (225, 247, 114, 11, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (226, 248, 114, 12, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (227, 249, 115, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (228, 250, 115, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (229, 251, 115, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (230, 252, 115, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (231, 253, 115, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (232, 254, 115, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (233, 255, 115, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (234, 256, 115, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (235, 257, 115, 9, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (236, 258, 115, 10, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (237, 259, 115, 11, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (238, 260, 115, 12, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (239, 261, 115, 13, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (240, 262, 115, 14, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (241, 263, 115, 15, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (242, 264, 115, 16, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (243, 265, 115, 17, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (244, 266, 116, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (245, 267, 116, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (246, 268, 116, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (247, 269, 116, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (248, 270, 116, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (249, 271, 116, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (250, 272, 116, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (251, 273, 116, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (252, 274, 116, 9, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (253, 275, 116, 10, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (254, 276, 116, 11, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (255, 277, 117, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (256, 278, 117, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (257, 279, 117, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (258, 280, 117, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (259, 281, 117, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (260, 282, 117, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (261, 283, 118, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (262, 284, 118, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (263, 285, 118, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (264, 286, 118, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (265, 287, 118, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (266, 288, 118, 6, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (267, 289, 118, 7, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (268, 290, 118, 8, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (269, 291, 118, 9, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (270, 292, 118, 10, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (271, 293, 118, 11, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (272, 294, 118, 12, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (273, 295, 119, 1, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (274, 296, 119, 2, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (275, 297, 119, 3, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (276, 298, 119, 4, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (277, 299, 119, 5, 1, 1051)
GO
INSERT [dbo].[FrameworkCompetencies] ([ID], [CompetencyID], [FrameworkCompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (278, 300, 119, 6, 1, 1051)
GO
SET IDENTITY_INSERT [dbo].[FrameworkCompetencies] OFF
GO
