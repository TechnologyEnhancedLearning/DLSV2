SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_ApplicationGroups](
	[AppGroupID] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationGroup] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ApplicationGroups] PRIMARY KEY CLUSTERED 
(
	[AppGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_aspProgressLearningLogItems](
	[LinkLearningLogID] [int] IDENTITY(1,1) NOT NULL,
	[aspProgressID] [int] NOT NULL,
	[LearningLogItemID] [int] NOT NULL,
 CONSTRAINT [PK_aspProgressLearningLogItems] PRIMARY KEY CLUSTERED 
(
	[LinkLearningLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_aspSelfAssessLog](
	[SelfAssessLogID] [int] IDENTITY(1,1) NOT NULL,
	[aspProgressID] [int] NOT NULL,
	[AssessDescriptorID] [int] NOT NULL,
	[OutcomesEvidence] [nvarchar](max) NULL,
	[SupervisorVerifiedID] [int] NULL,
	[SupervisorVerifiedDate] [datetime] NULL,
	[SupervisorOutcome] [bit] NULL,
	[SupervisorVerifiedComments] [nvarchar](max) NULL,
	[LastReviewed] [datetime] NULL,
	[ReviewedByCandidateID] [int] NOT NULL,
 CONSTRAINT [PK_aspSelfAssessLog] PRIMARY KEY CLUSTERED 
(
	[SelfAssessLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_AssessmentTypeDescriptors](
	[AssessmentTypeDescriptorID] [int] IDENTITY(1,1) NOT NULL,
	[AssessmentTypeID] [int] NOT NULL,
	[DescriptorText] [nvarchar](100) NOT NULL,
	[DescriptorDetail] [nvarchar](max) NULL,
	[WeightingScore] [int] NOT NULL,
 CONSTRAINT [PK_AssessmentTypeDescriptors] PRIMARY KEY CLUSTERED 
(
	[AssessmentTypeDescriptorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_AssessmentTypes](
	[AssessmentTypeID] [int] IDENTITY(1,1) NOT NULL,
	[AssessmentType] [nvarchar](100) NOT NULL,
	[LayoutHZ] [bit] NOT NULL,
	[SelfAssessPrompt] [nvarchar](500) NULL,
	[IncludeComments] [bit] NOT NULL,
	[MandatoryComments] [bit] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_Browsers](
	[BrowserID] [int] IDENTITY(1,1) NOT NULL,
	[Browser] [nvarchar](50) NULL,
 CONSTRAINT [PK_Browsers] PRIMARY KEY CLUSTERED 
(
	[BrowserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_ConsolidationRatings](
	[ConsolidationRatingID] [int] IDENTITY(1,1) NOT NULL,
	[SectionID] [int] NOT NULL,
	[Rating] [int] NOT NULL,
	[RateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ConsolidationRatings] PRIMARY KEY CLUSTERED 
(
	[ConsolidationRatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_ContributorRoles](
	[ContributorRoleID] [int] IDENTITY(1,1) NOT NULL,
	[ContributorRole] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ContributorRoles] PRIMARY KEY CLUSTERED 
(
	[ContributorRoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_EmailDupExclude](
	[ExclusionID] [int] IDENTITY(1,1) NOT NULL,
	[ExclusionEmail] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_EmailDupExclude] PRIMARY KEY CLUSTERED 
(
	[ExclusionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_FilteredComptenencyMapping](
	[CompetencyID] [int] NOT NULL,
	[FilteredCompetencyID] [int] NOT NULL,
 CONSTRAINT [PK_FilteredComptenencyMapping] PRIMARY KEY CLUSTERED 
(
	[CompetencyID] ASC,
	[FilteredCompetencyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_FilteredSeniorityMapping](
	[CompetencyGroupID] [int] NOT NULL,
	[SeniorityID] [int] NOT NULL,
 CONSTRAINT [PK_FilteredSeniorityMapping] PRIMARY KEY CLUSTERED 
(
	[CompetencyGroupID] ASC,
	[SeniorityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_FollowUpFeedback](
	[FUEvaluationID] [int] IDENTITY(1,1) NOT NULL,
	[JobGroupID] [int] NOT NULL,
	[CustomisationID] [int] NOT NULL,
	[Q1] [tinyint] NOT NULL,
	[Q1Comments] [nvarchar](max) NULL,
	[Q2] [tinyint] NOT NULL,
	[Q2Comments] [nvarchar](max) NULL,
	[Q3] [tinyint] NOT NULL,
	[Q3Comments] [nvarchar](max) NULL,
	[Q4] [tinyint] NOT NULL,
	[Q4Comments] [nvarchar](max) NULL,
	[Q5] [tinyint] NOT NULL,
	[Q5Comments] [nvarchar](max) NULL,
	[Q6] [tinyint] NOT NULL,
	[Q6Comments] [nvarchar](max) NULL,
	[EvaluatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_FollowUpFeedback] PRIMARY KEY CLUSTERED 
(
	[FUEvaluationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_KBCentreBrandsExcludes](
	[KBCentreBrandExcludeID] [int] IDENTITY(1,1) NOT NULL,
	[CentreID] [int] NOT NULL,
	[BrandID] [int] NOT NULL,
 CONSTRAINT [PK_KBCentreBrandsExcludes] PRIMARY KEY CLUSTERED 
(
	[KBCentreBrandExcludeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_KBCentreCategoryExcludes](
	[KBCentreCategoryExcludeID] [int] IDENTITY(1,1) NOT NULL,
	[CentreID] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
 CONSTRAINT [PK_KBCentreCategoryExcludes] PRIMARY KEY CLUSTERED 
(
	[KBCentreCategoryExcludeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_kbLearnTrack](
	[kbLearnTrackID] [int] IDENTITY(1,1) NOT NULL,
	[TutorialID] [int] NOT NULL,
	[CandidateID] [int] NOT NULL,
	[LaunchDate] [datetime] NOT NULL,
 CONSTRAINT [PK_kbLearnTrack] PRIMARY KEY CLUSTERED 
(
	[kbLearnTrackID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_kbSearches](
	[kbSearchID] [int] IDENTITY(1,1) NOT NULL,
	[CandidateID] [int] NOT NULL,
	[OfficeVersionCSV] [varchar](30) NULL,
	[ApplicationCSV] [varchar](30) NULL,
	[ApplicationGroupCSV] [varchar](30) NULL,
	[SearchTerm] [varchar](255) NULL,
	[Inadequate] [bit] NOT NULL,
	[SearchDate] [datetime] NOT NULL,
	[BrandCSV] [varchar](30) NULL,
	[CategoryCSV] [varchar](80) NULL,
	[TopicCSV] [varchar](180) NULL,
 CONSTRAINT [PK_kbSearches] PRIMARY KEY CLUSTERED 
(
	[kbSearchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_kbVideoTrack](
	[kbVideoTrackID] [int] IDENTITY(1,1) NOT NULL,
	[TutorialID] [int] NOT NULL,
	[CandidateID] [int] NOT NULL,
	[VideoClickedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_tKBVideoTrack] PRIMARY KEY CLUSTERED 
(
	[kbVideoTrackID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_kbYouTubeTrack](
	[YouTubeTrackID] [int] IDENTITY(1,1) NOT NULL,
	[CandidateID] [int] NOT NULL,
	[YouTubeURL] [nvarchar](256) NOT NULL,
	[VidTitle] [nvarchar](100) NOT NULL,
	[LaunchDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_kbYouTubeTrack] PRIMARY KEY CLUSTERED 
(
	[YouTubeTrackID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_LearnerPortalProgressKeys](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LPGUID] [uniqueidentifier] NOT NULL,
	[ProgressID] [int] NOT NULL,
 CONSTRAINT [PK_LearnerPortalProgressKeys] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_NonCompletedFeedback](
	[NonCompletedFeedbackID] [int] IDENTITY(1,1) NOT NULL,
	[JobGroupID] [int] NOT NULL,
	[CustomisationID] [int] NOT NULL,
	[WhyNotComplete] [tinyint] NOT NULL,
	[R1_Style] [bit] NOT NULL,
	[R2_PreferF2F] [bit] NOT NULL,
	[R3_NotEnjoy] [bit] NOT NULL,
	[R4_KnewItAll] [bit] NOT NULL,
	[R5_TooHard] [bit] NOT NULL,
	[R6_TechIssue] [bit] NOT NULL,
	[R7_DislikeComputers] [bit] NOT NULL,
	[EvalDate] [datetime] NOT NULL,
 CONSTRAINT [PK_NonCompletedFeedback] PRIMARY KEY CLUSTERED 
(
	[NonCompletedFeedbackID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_OfficeApplications](
	[OfficeAppID] [int] IDENTITY(1,1) NOT NULL,
	[OfficeApplication] [nvarchar](50) NOT NULL,
	[Active] [bit] NOT NULL,
	[ImgURL] [nvarchar](255) NULL,
 CONSTRAINT [PK_OfficeApplications] PRIMARY KEY CLUSTERED 
(
	[OfficeAppID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_OfficeVersions](
	[OfficeVersionID] [int] IDENTITY(1,1) NOT NULL,
	[OfficeVersion] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_OfficeVersions] PRIMARY KEY CLUSTERED 
(
	[OfficeVersionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_OrderLines](
	[OrderLineID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[SendByDate] [datetime] NOT NULL,
 CONSTRAINT [PK_OrderLines] PRIMARY KEY CLUSTERED 
(
	[OrderLineID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_Orders](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[CentreID] [int] NOT NULL,
	[OrderNotes] [varchar](250) NULL,
	[OrderDate] [datetime] NOT NULL,
	[SentDate] [datetime] NULL,
	[OrderStatus] [int] NOT NULL,
	[DelName] [varchar](100) NULL,
	[DelAddress1] [varchar](100) NULL,
	[DelAddress2] [varchar](100) NULL,
	[DelAddress3] [varchar](100) NULL,
	[DelAddress4] [varchar](100) NULL,
	[DelPostcode] [varchar](10) NULL,
	[DelTownCity] [varchar](100) NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_pl_CaseContent](
	[CaseContentID] [int] IDENTITY(1,1) NOT NULL,
	[CaseStudyID] [int] NOT NULL,
	[ContentHeading] [nvarchar](100) NULL,
	[ContentText] [nvarchar](max) NULL,
	[ContentImage] [image] NULL,
	[ContentQuoteText] [nvarchar](max) NULL,
	[ContentQuoteAttr] [nvarchar](100) NULL,
	[OrderByNumber] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[ImageWidth] [int] NOT NULL,
 CONSTRAINT [PK_pl_CaseContent] PRIMARY KEY CLUSTERED 
(
	[CaseContentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_pl_CaseStudies](
	[CaseStudyID] [int] IDENTITY(1,1) NOT NULL,
	[CaseHeading] [nvarchar](100) NULL,
	[CaseSubHeading] [nvarchar](500) NULL,
	[CaseDate] [datetime] NULL,
	[ProductID] [int] NULL,
	[BrandID] [int] NULL,
	[Active] [bit] NOT NULL,
	[CaseImage] [image] NULL,
	[CaseStudyGroup] [nvarchar](100) NULL,
 CONSTRAINT [PK_pl_CaseStudies] PRIMARY KEY CLUSTERED 
(
	[CaseStudyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_pl_Features](
	[FeatureID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[FeatureHeading] [nvarchar](100) NOT NULL,
	[FeatureDescription] [nvarchar](max) NULL,
	[FeatureIconClass] [nvarchar](50) NULL,
	[FeatureColourClass] [nvarchar](50) NULL,
	[FeatureScreenshot] [image] NULL,
	[FeatureVideoURL] [nvarchar](255) NULL,
	[Active] [bit] NOT NULL,
	[OrderByNumber] [int] NOT NULL,
 CONSTRAINT [PK_pl_Features] PRIMARY KEY CLUSTERED 
(
	[FeatureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_pl_Products](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[ProductName] [nvarchar](100) NOT NULL,
	[ProductHeading] [nvarchar](100) NULL,
	[ProductTagline] [nvarchar](255) NULL,
	[ProductScreenshot] [image] NULL,
	[ProductDemoVidURL] [nvarchar](255) NULL,
	[OrderByNumber] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[ProductIconClass] [nvarchar](255) NULL,
 CONSTRAINT [PK_pl_Products] PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_pl_Quotes](
	[QuoteID] [int] IDENTITY(1,1) NOT NULL,
	[QuoteText] [nvarchar](500) NOT NULL,
	[AttrIndividual] [nvarchar](100) NULL,
	[AttrOrganisation] [nvarchar](100) NULL,
	[QuoteDate] [datetime] NOT NULL,
	[ProductID] [int] NULL,
	[BrandID] [int] NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_pl_Quotes] PRIMARY KEY CLUSTERED 
(
	[QuoteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_Products](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[Active] [bit] NOT NULL,
	[ProductName] [varchar](100) NOT NULL,
	[ProductDescription] [varchar](250) NULL,
	[InStock] [bit] NOT NULL,
	[QuantityLimit] [int] NOT NULL,
	[ExpectedDate] [datetime] NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_ProgressContributors](
	[ProgressContributorID] [int] IDENTITY(1,1) NOT NULL,
	[ProgressID] [int] NOT NULL,
	[CandidateID] [int] NOT NULL,
	[ContributorRoleID] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[LastAccess] [datetime] NULL,
 CONSTRAINT [PK_ProgressContributors] PRIMARY KEY CLUSTERED 
(
	[ProgressContributorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_ProgressKeyCheckLog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[LPGUID] [uniqueidentifier] NULL,
	[ProgressID] [int] NULL,
	[ReturnVal] [int] NULL,
 CONSTRAINT [PK_ProgressKeyCheckLog] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_pwBulletins](
	[BulletinID] [int] IDENTITY(1,1) NOT NULL,
	[BulletinName] [nvarchar](100) NOT NULL,
	[BulletinDescription] [nvarchar](max) NULL,
	[BulletinFileName] [nvarchar](100) NOT NULL,
	[BulletinDate] [date] NOT NULL,
	[BulletinImage] [image] NULL,
 CONSTRAINT [PK_pwBulletins] PRIMARY KEY CLUSTERED 
(
	[BulletinID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_pwCaseStudies](
	[CaseStudyID] [int] IDENTITY(1,1) NOT NULL,
	[CaseStudyName] [nvarchar](100) NOT NULL,
	[CaseStudyDesc] [nvarchar](500) NOT NULL,
	[CaseStudyImage] [nvarchar](255) NULL,
	[CaseStudyURL] [nvarchar](255) NULL,
	[CaseStudyGroup] [nvarchar](100) NULL,
 CONSTRAINT [PK_pwCaseStudies] PRIMARY KEY CLUSTERED 
(
	[CaseStudyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_pwNews](
	[NewsID] [int] IDENTITY(1,1) NOT NULL,
	[NewsDate] [date] NOT NULL,
	[NewsTitle] [nvarchar](255) NULL,
	[NewsDetail] [nvarchar](max) NULL,
	[Active] [bit] NOT NULL,
	[ProductID] [int] NULL,
	[BrandID] [int] NULL,
 CONSTRAINT [PK_pwNews] PRIMARY KEY CLUSTERED 
(
	[NewsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_pwVisits](
	[VisitID] [int] IDENTITY(1,1) NOT NULL,
	[VisitIP] [varchar](25) NULL,
	[VisitDate] [datetime] NOT NULL,
 CONSTRAINT [PK_pwVisits] PRIMARY KEY CLUSTERED 
(
	[VisitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deprecated_VideoRatings](
	[VideoRatingID] [int] IDENTITY(1,1) NOT NULL,
	[TutorialID] [int] NOT NULL,
	[Rating] [int] NOT NULL,
	[RateDate] [datetime] NOT NULL,
	[KBRate] [bit] NOT NULL,
	[CentreID] [int] NOT NULL,
 CONSTRAINT [PK_VideoRatings] PRIMARY KEY CLUSTERED 
(
	[VideoRatingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[deprecated_aspSelfAssessLog] ADD  CONSTRAINT [DF_aspSelfAssessLog_TutStat]  DEFAULT ((0)) FOR [AssessDescriptorID]
GO
ALTER TABLE [dbo].[deprecated_aspSelfAssessLog] ADD  CONSTRAINT [DF_aspSelfAssessLog_ReviewedByCandidateID]  DEFAULT ((0)) FOR [ReviewedByCandidateID]
GO
ALTER TABLE [dbo].[deprecated_AssessmentTypes] ADD  CONSTRAINT [DF_AssessmentTypes_LayoutHZ]  DEFAULT ((0)) FOR [LayoutHZ]
GO
ALTER TABLE [dbo].[deprecated_AssessmentTypes] ADD  CONSTRAINT [DF_AssessmentTypes_IncludeComments]  DEFAULT ((1)) FOR [IncludeComments]
GO
ALTER TABLE [dbo].[deprecated_AssessmentTypes] ADD  CONSTRAINT [DF_AssessmentTypes_MandatoryComments]  DEFAULT ((0)) FOR [MandatoryComments]
GO
ALTER TABLE [dbo].[deprecated_ConsolidationRatings] ADD  CONSTRAINT [DF_ConsolidationRatings_RateDate]  DEFAULT (getutcdate()) FOR [RateDate]
GO
ALTER TABLE [dbo].[deprecated_FollowUpFeedback] ADD  CONSTRAINT [DF_FollowUpFeedback_Q1]  DEFAULT ((0)) FOR [Q1]
GO
ALTER TABLE [dbo].[deprecated_FollowUpFeedback] ADD  CONSTRAINT [DF_FollowUpFeedback_Q2]  DEFAULT ((0)) FOR [Q2]
GO
ALTER TABLE [dbo].[deprecated_FollowUpFeedback] ADD  CONSTRAINT [DF_FollowUpFeedback_Q3]  DEFAULT ((0)) FOR [Q3]
GO
ALTER TABLE [dbo].[deprecated_FollowUpFeedback] ADD  CONSTRAINT [DF_FollowUpFeedback_Q4]  DEFAULT ((0)) FOR [Q4]
GO
ALTER TABLE [dbo].[deprecated_FollowUpFeedback] ADD  CONSTRAINT [DF_FollowUpFeedback_Q5]  DEFAULT ((0)) FOR [Q5]
GO
ALTER TABLE [dbo].[deprecated_FollowUpFeedback] ADD  CONSTRAINT [DF_FollowUpFeedback_Q6]  DEFAULT ((0)) FOR [Q6]
GO
ALTER TABLE [dbo].[deprecated_FollowUpFeedback] ADD  CONSTRAINT [DF_FollowUpFeedback_EvaluatedDate]  DEFAULT (getutcdate()) FOR [EvaluatedDate]
GO
ALTER TABLE [dbo].[deprecated_kbLearnTrack] ADD  CONSTRAINT [DF_kbLearnTrack_LaunchDate]  DEFAULT (getutcdate()) FOR [LaunchDate]
GO
ALTER TABLE [dbo].[deprecated_kbSearches] ADD  CONSTRAINT [DF_kbSearches_Inadequate]  DEFAULT ((0)) FOR [Inadequate]
GO
ALTER TABLE [dbo].[deprecated_kbSearches] ADD  CONSTRAINT [DF_kbSearches_SearchDate]  DEFAULT (getutcdate()) FOR [SearchDate]
GO
ALTER TABLE [dbo].[deprecated_kbVideoTrack] ADD  CONSTRAINT [DF_Table_1_VideoClickDate]  DEFAULT (getutcdate()) FOR [VideoClickedDate]
GO
ALTER TABLE [dbo].[deprecated_kbYouTubeTrack] ADD  CONSTRAINT [DF_kbYouTubeTrack_LaunchDateTime]  DEFAULT (getutcdate()) FOR [LaunchDateTime]
GO
ALTER TABLE [dbo].[deprecated_LearnerPortalProgressKeys] ADD  CONSTRAINT [DF_LearnerPortalProgressKeys_LPGUID]  DEFAULT (newid()) FOR [LPGUID]
GO
ALTER TABLE [dbo].[deprecated_NonCompletedFeedback] ADD  CONSTRAINT [DF_NonCompletedFeedback_WhyNotComplete]  DEFAULT ((0)) FOR [WhyNotComplete]
GO
ALTER TABLE [dbo].[deprecated_NonCompletedFeedback] ADD  CONSTRAINT [DF_NonCompletedFeedback_R1_Style]  DEFAULT ((0)) FOR [R1_Style]
GO
ALTER TABLE [dbo].[deprecated_NonCompletedFeedback] ADD  CONSTRAINT [DF_NonCompletedFeedback_R2_PreferF2F]  DEFAULT ((0)) FOR [R2_PreferF2F]
GO
ALTER TABLE [dbo].[deprecated_NonCompletedFeedback] ADD  CONSTRAINT [DF_NonCompletedFeedback_R3_NotEnjoy]  DEFAULT ((0)) FOR [R3_NotEnjoy]
GO
ALTER TABLE [dbo].[deprecated_NonCompletedFeedback] ADD  CONSTRAINT [DF_NonCompletedFeedback_R4_KnewItAll]  DEFAULT ((0)) FOR [R4_KnewItAll]
GO
ALTER TABLE [dbo].[deprecated_NonCompletedFeedback] ADD  CONSTRAINT [DF_NonCompletedFeedback_R5_TooHard]  DEFAULT ((0)) FOR [R5_TooHard]
GO
ALTER TABLE [dbo].[deprecated_NonCompletedFeedback] ADD  CONSTRAINT [DF_NonCompletedFeedback_R6_TechIssue]  DEFAULT ((0)) FOR [R6_TechIssue]
GO
ALTER TABLE [dbo].[deprecated_NonCompletedFeedback] ADD  CONSTRAINT [DF_NonCompletedFeedback_R7_DislikeComputers]  DEFAULT ((0)) FOR [R7_DislikeComputers]
GO
ALTER TABLE [dbo].[deprecated_NonCompletedFeedback] ADD  CONSTRAINT [DF_NonCompletedFeedback_EvalDate]  DEFAULT (getutcdate()) FOR [EvalDate]
GO
ALTER TABLE [dbo].[deprecated_OfficeApplications] ADD  CONSTRAINT [DF_OfficeApplications_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[deprecated_Orders] ADD  CONSTRAINT [DF_Orders_OrderDate]  DEFAULT (getutcdate()) FOR [OrderDate]
GO
ALTER TABLE [dbo].[deprecated_Orders] ADD  CONSTRAINT [DF_Orders_OrderStatus]  DEFAULT ((1)) FOR [OrderStatus]
GO
ALTER TABLE [dbo].[deprecated_pl_CaseContent] ADD  CONSTRAINT [DF_pl_CaseContent_OrderByNumber]  DEFAULT ((0)) FOR [OrderByNumber]
GO
ALTER TABLE [dbo].[deprecated_pl_CaseContent] ADD  CONSTRAINT [DF_pl_CaseContent_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[deprecated_pl_CaseContent] ADD  CONSTRAINT [DF_pl_CaseContent_ImageWidth]  DEFAULT ((33)) FOR [ImageWidth]
GO
ALTER TABLE [dbo].[deprecated_pl_CaseStudies] ADD  CONSTRAINT [DF_pl_CaseStudies_CaseDate]  DEFAULT (getutcdate()) FOR [CaseDate]
GO
ALTER TABLE [dbo].[deprecated_pl_CaseStudies] ADD  CONSTRAINT [DF_pl_CaseStudies_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[deprecated_pl_Features] ADD  CONSTRAINT [DF_pl_Features_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[deprecated_pl_Features] ADD  CONSTRAINT [DF_pl_Features_OrderByNumber]  DEFAULT ((0)) FOR [OrderByNumber]
GO
ALTER TABLE [dbo].[deprecated_pl_Products] ADD  CONSTRAINT [DF_pl_Products_OrderByNumber]  DEFAULT ((0)) FOR [OrderByNumber]
GO
ALTER TABLE [dbo].[deprecated_pl_Products] ADD  CONSTRAINT [DF_pl_Products_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[deprecated_pl_Quotes] ADD  CONSTRAINT [DF_pl_Quotes_QuoteDate]  DEFAULT (getutcdate()) FOR [QuoteDate]
GO
ALTER TABLE [dbo].[deprecated_pl_Quotes] ADD  CONSTRAINT [DF_pl_Quotes_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[deprecated_Products] ADD  CONSTRAINT [DF_Products_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[deprecated_Products] ADD  CONSTRAINT [DF_Products_InStock]  DEFAULT ((1)) FOR [InStock]
GO
ALTER TABLE [dbo].[deprecated_Products] ADD  CONSTRAINT [DF_Products_QuantityLimit]  DEFAULT ((10)) FOR [QuantityLimit]
GO
ALTER TABLE [dbo].[deprecated_ProgressContributors] ADD  CONSTRAINT [DF_ProgressContributors_CandidateID]  DEFAULT ((0)) FOR [CandidateID]
GO
ALTER TABLE [dbo].[deprecated_ProgressContributors] ADD  CONSTRAINT [DF_ProgressContributors_ContributorRoleID]  DEFAULT ((1)) FOR [ContributorRoleID]
GO
ALTER TABLE [dbo].[deprecated_ProgressContributors] ADD  CONSTRAINT [DF_ProgressContributors_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[deprecated_pwBulletins] ADD  CONSTRAINT [DF_pwBulletins_BulletinDate]  DEFAULT (getutcdate()) FOR [BulletinDate]
GO
ALTER TABLE [dbo].[deprecated_pwNews] ADD  CONSTRAINT [DF_pwNews_NewsDate]  DEFAULT (getutcdate()) FOR [NewsDate]
GO
ALTER TABLE [dbo].[deprecated_pwNews] ADD  CONSTRAINT [DF_pwNews_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[deprecated_pwVisits] ADD  CONSTRAINT [DF_pwVisits_VisitDate]  DEFAULT (getutcdate()) FOR [VisitDate]
GO
ALTER TABLE [dbo].[deprecated_VideoRatings] ADD  CONSTRAINT [DF_VideoRatings_RateDate]  DEFAULT (getutcdate()) FOR [RateDate]
GO
ALTER TABLE [dbo].[deprecated_VideoRatings] ADD  CONSTRAINT [DF_VideoRatings_KBRate]  DEFAULT ((0)) FOR [KBRate]
GO
ALTER TABLE [dbo].[deprecated_VideoRatings] ADD  CONSTRAINT [DF_VideoRatings_CentreID]  DEFAULT ((0)) FOR [CentreID]
GO
