BEGIN TRANSACTION

SET IDENTITY_INSERT SelfAssessments ON
INSERT INTO SelfAssessments (Id, Name, Description)
    VALUES (3, 'Utilising External Evidence and Organisational Knowledge – A Self-Assessment', 'Review the arrangements you have in place to use the expertise of knowledge specialists, access high quality evidence and the capacity you have available to undertake background research.')
SET IDENTITY_INSERT SelfAssessments OFF

SET IDENTITY_INSERT CompetencyGroups ON
INSERT INTO CompetencyGroups (Id, Name) VALUES
    (9, 'Leadership'),
    (10, 'Behaviours'),
    (11, 'Capabilities and Working Practices'),
    (12, 'Knowledge Services')
SET IDENTITY_INSERT CompetencyGroups OFF

SET IDENTITY_INSERT AssessmentQuestions ON
INSERT INTO AssessmentQuestions (Id, Question, MaxValueDescription, MinValueDescription) VALUES
    (4, 'Rate your organisational maturity', 'Business as usual', 'Nothing in place yet')
SET IDENTITY_INSERT AssessmentQuestions OFF
    
SET IDENTITY_INSERT Competencies ON
INSERT INTO Competencies (Id, Description, CompetencyGroupId) VALUES
    (35, 'Leaders and their teams use of externally  generated evidence', 9),
    (36, 'Leaders taking a strategic view of using external evidence and organisational knowledge', 9),
    (37, 'Leadership to support the use of external evidence and organisational knowledge', 9),
    (38, 'Leaders building a learning organisation', 9),
    (39, 'Leaders advocate and model the benefits of using evidence and sharing knowledge', 9),

    (40, 'Capacity to use  evidence from research', 10),
    (41, 'Productivity and efficiency', 10),
    (42, 'Approach to innovation', 10),
    (43, 'Approach to keeping up to date', 10),
    (44, 'Capacity to use organisational knowledge', 10),
    (45, 'Cross-team working and networking', 10),
    (46, 'Technology for collaboration', 10),
    (47, 'Access to national guidance and policies', 10),
    (48, 'Access to Standard Operating Procedures, Policies and Guidance', 10),

    (49, 'Using organisational knowledge, developing skills of healthcare workforce', 11),
    (50, 'Skills to mobilise knowledge throughout the organisation', 11),
    (51, 'Using evidence from research developing skills of healthcare workforce', 11),
    (52, 'Skills to access evidence from research ', 11),

    (53, 'Access to a library and knowledge service - whether via an SLA or developed as an in-house service', 12),
    (54, 'Use of library and knowledge services', 12),
    (55, 'Library and Knowledge Service alignment to organisational priorities ', 12),
    (56, 'Capacity to use evidence form research', 12),
    (57, 'Capacity of Library and Knowledge services to mobilise knowledge throughout the organisation (if there is a Library and Knowledge Service', 12)
SET IDENTITY_INSERT Competencies OFF

INSERT INTO CompetencyAssessmentQuestions (CompetencyID, AssessmentQuestionID) VALUES
    (35, 4), 
    (36, 4), 
    (37, 4), 
    (38, 4), 
    (39, 4), 
    (40, 4), 
    (41, 4), 
    (42, 4), 
    (43, 4), 
    (44, 4), 
    (45, 4), 
    (46, 4), 
    (47, 4), 
    (48, 4), 
    (49, 4), 
    (50, 4), 
    (51, 4), 
    (52, 4), 
    (53, 4), 
    (54, 4), 
    (55, 4), 
    (56, 4),
    (57, 4)

INSERT INTO SelfAssessmentStructure (SelfAssessmentID, CompetencyID) VALUES
    (3, 35), (3, 36), (3, 37), (3, 38), (3, 39), (3, 40), (3, 41), (3, 42), (3, 43), (3, 44),
    (3, 45), (3, 46), (3, 47), (3, 48), (3, 49), (3, 50), (3, 51), (3, 52), (3, 53), (3, 54),
    (3, 55), (3, 56), (3, 57)

COMMIT
