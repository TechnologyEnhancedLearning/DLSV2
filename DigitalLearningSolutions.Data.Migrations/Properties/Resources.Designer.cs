﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DigitalLearningSolutions.Data.Migrations.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DigitalLearningSolutions.Data.Migrations.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///SET ANSI_NULLS ON
        ///GO
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///-- =============================================
        ///-- Author:		Kevin Whittaker
        ///-- Create date: 16/10/2020
        ///-- Description:	Updates a customisation based on form values
        ///-- V2 Adds @CCEmail
        ///-- V3 Adds ApplyLPDefaultsToSelfEnrol
        ///-- =============================================
        ///CREATE PROCEDURE [dbo].[UpdateCustomisation_V3]
        ///	-- Add the parameters for the stored procedure here
        ///	@CustomisationID As Int,
        ///	@Active as bit,
        ///	@CustomisationName as nvarcha [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ApplyLPDefaultsSPChanges {
            get {
                return ResourceManager.GetString("ApplyLPDefaultsSPChanges", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- =============================================
        ///-- Author:		Kevin Whittaker
        ///-- Create date: 04/01/2021
        ///-- Description:	Reorders the FrameworkCompetencyGroups in a given Framework - moving the given group up or down.
        ///-- =============================================
        ///CREATE OR ALTER PROCEDURE [dbo].[ReorderFrameworkCompetencyGroup]
        ///	-- Add the parameters for the stored procedure here
        ///	@FrameworkCompetencyGroupID int,
        ///	@Direction nvarchar(4) = &apos;&apos;,
        ///	@SingleStep bit
        ///AS
        ///BEGIN
        ///	-- SET NOCOUNT ON added  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string CreateOrAlterReorderFrameworkCompetenciesAndGroupsSPs {
            get {
                return ResourceManager.GetString("CreateOrAlterReorderFrameworkCompetenciesAndGroupsSPs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /****** Object:  StoredProcedure [dbo].[InsertCustomisation_V3]    Script Date: 20/11/2020 14:12:52 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///
        ///-- =============================================
        ///-- Author:		Kevin Whittaker
        ///-- Create date: 28 February 2020
        ///-- V2 Adds @CCCompletion field
        ///-- =============================================
        ///CREATE OR ALTER PROCEDURE [dbo].[InsertCustomisation_V3] 
        ///	@Active as bit,
        ///	@ApplicationID as int,
        ///	@CentreID as int,
        ///	@CustomisationName as nvarch [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DLSV2_106_CreateOrAlterInsertCustomisation_V3 {
            get {
                return ResourceManager.GetString("DLSV2_106_CreateOrAlterInsertCustomisation_V3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /****** Object:  StoredProcedure [dbo].[InsertCustomisation_V3]    Script Date: 20/11/2020 14:12:52 ******/
        ///DROP PROCEDURE [dbo].[InsertCustomisation_V3]
        ///GO
        ///
        ///
        ///.
        /// </summary>
        internal static string DLSV2_106_DropInsertCustomisation_V3 {
            get {
                return ResourceManager.GetString("DLSV2_106_DropInsertCustomisation_V3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /****** Object:  StoredProcedure [dbo].[GetFilteredCompetencyResponsesForCandidate]    Script Date: 22/09/2020 09:22:43 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///-- =============================================
        ///-- Author:		Kevin Whittaker
        ///-- Create date: 22/09/2020
        ///-- Description:	Returns user self assessment responses (AVG) for Filtered competency
        ///-- =============================================
        ///CREATE OR ALTER PROCEDURE [dbo].[GetFilteredCompetencyResponsesForCandidate]
        ///	-- Add t [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DLSV2_133_AdjustScoresForFilteredSP {
            get {
                return ResourceManager.GetString("DLSV2_133_AdjustScoresForFilteredSP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /****** Object:  StoredProcedure [dbo].[GetFilteredCompetencyResponsesForCandidate]    Script Date: 22/09/2020 09:22:43 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///-- =============================================
        ///-- Author:		Kevin Whittaker
        ///-- Create date: 22/09/2020
        ///-- Description:	Returns user self assessment responses (AVG) for Filtered competency
        ///-- =============================================
        ///CREATE OR ALTER PROCEDURE [dbo].[GetFilteredCompetencyResponsesForCandidate]
        ///	-- Add t [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DLSV2_133_UnAdjustScoresForFilteredSP {
            get {
                return ResourceManager.GetString("DLSV2_133_UnAdjustScoresForFilteredSP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ////****** Object:  UserDefinedFunction [dbo].[GetSelfAssessmentSummaryForCandidate]    Script Date: 28/01/2021 07:45:22 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///
        ///CREATE OR ALTER FUNCTION [dbo].[GetSelfAssessmentSummaryForCandidate]
        ///(
        ///	@CandidateID int,
        ///	@SelfAssessmentID int
        ///)
        ///RETURNS @ResTable TABLE 
        ///(
        ///	CompetencyGroupID int,
        ///	Confidence float,
        ///	Relevance float
        ///)
        ///
        ///AS	  
        ///BEGIN
        ///INSERT INTO @ResTable
        ///	SELECT CompetencyGroupID, [1] AS Confidence, [2] AS Relevance
        ///FROM    [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DLSV2_153_DropFilteredFunctionTweak {
            get {
                return ResourceManager.GetString("DLSV2_153_DropFilteredFunctionTweak", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///
        ////****** Object:  StoredProcedure [dbo].[GetFilteredCompetencyResponsesForCandidate]    Script Date: 27/01/2021 16:01:15 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///
        ///-- =============================================
        ///-- Author:		Kevin Whittaker
        ///-- Create date: 22/09/2020
        ///-- Description:	Returns user self assessment responses (AVG) for Filtered competency
        ///-- =============================================
        ///CREATE OR ALTER PROCEDURE [dbo].[GetFilteredCompetencyResponsesForCandidate]
        ///	 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DLSV2_153_DropFilteredSPFixes {
            get {
                return ResourceManager.GetString("DLSV2_153_DropFilteredSPFixes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /****** Object:  UserDefinedFunction [dbo].[GetSelfAssessmentSummaryForCandidate]    Script Date: 28/01/2021 07:43:39 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///CREATE OR ALTER FUNCTION [dbo].[GetSelfAssessmentSummaryForCandidate]
        ///(
        ///	@CandidateID int,
        ///	@SelfAssessmentID int
        ///)
        ///RETURNS @ResTable TABLE 
        ///(
        ///	CompetencyGroupID int,
        ///	Confidence float,
        ///	Relevance float
        ///)
        ///
        ///AS	  
        ///BEGIN
        ///INSERT INTO @ResTable
        ///	SELECT CompetencyGroupID, [1] AS Confidence, [2] AS Relevance
        ///FROM   (SELEC [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DLSV2_153_FilteredFunctionTweak {
            get {
                return ResourceManager.GetString("DLSV2_153_FilteredFunctionTweak", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /****** Object:  StoredProcedure [dbo].[GetFilteredCompetencyResponsesForCandidate]    Script Date: 27/01/2021 15:29:35 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///-- =============================================
        ///-- Author:		Kevin Whittaker
        ///-- Create date: 22/09/2020
        ///-- Description:	Returns user self assessment responses (AVG) for Filtered competency
        ///-- =============================================
        ///CREATE OR ALTER   PROCEDURE [dbo].[GetFilteredCompetencyResponsesForCandidate]
        ///	-- Add [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DLSV2_153_FilteredSPFixes {
            get {
                return ResourceManager.GetString("DLSV2_153_FilteredSPFixes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --DLSV2-95 Adds System Versioning to auditable tables (UP)
        ///
        ///--Frameworks table
        ///ALTER TABLE Frameworks
        ///    ADD
        ///        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN
        ///            CONSTRAINT DF_Frameworks_SysStart DEFAULT SYSUTCDATETIME()
        ///      , SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN
        ///            CONSTRAINT DF_Frameworks_SysEnd DEFAULT CONVERT(DATETIME2, &apos;9999-12-31 23:59:59.9999999&apos;),
        ///        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
        ///GO
        ///
        ///ALTER TABLE Framework [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DLSV2_95_AddSystemVersioning {
            get {
                return ResourceManager.GetString("DLSV2_95_AddSystemVersioning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --DLSV2-95 Removes System Versioning to auditable tables (DOWN)
        ///
        ///
        ///-- Remove versioning from FrameworkCompetencies table
        ///ALTER TABLE FrameworkCompetencies SET (SYSTEM_VERSIONING = OFF);
        ///DROP TABLE dbo.FrameworkCompetencies;
        ///DROP TABLE dbo.FrameworkCompetenciesHistory;
        ///GO
        ///
        ///-- Remove versioning from FrameworkCompetencyGroups table
        ///ALTER TABLE FrameworkCompetencyGroups SET (SYSTEM_VERSIONING = OFF);
        ///DROP TABLE dbo.FrameworkCompetencyGroups;
        ///DROP TABLE dbo.FrameworkCompetencyGroupsHistory;
        ///GO
        ///
        ///--  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DLSV2_95_RemoveSystemVersioning {
            get {
                return ResourceManager.GetString("DLSV2_95_RemoveSystemVersioning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 䕓⁔乁䥓也䱕卌传ൎ䜊൏匊呅儠何䕔彄䑉久䥔䥆剅传ൎ䜊൏ഊⴊ‭㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽ഽⴊ‭畁桴牯ऺ䬉癥湩圠楨瑴歡牥਍ⴭ䌠敲瑡⁥慤整›㔱䘠扥畲牡⁹〲㈱਍ⴭ䐠獥牣灩楴湯ऺ牃慥整⁳桴⁥牐杯敲獳愠摮愠灳牐杯敲獳爠捥牯⁤潦⁲⁡敮⁷獵牥਍ⴭ删瑥牵獮ऺ〉㨠猠捵散獳‬牰杯敲獳挠敲瑡摥਍ⴭ†††ठㄉ㨠䘠楡敬⁤‭牰杯敲獳愠牬慥祤攠楸瑳൳ⴊ‭†††उ〱‰›慆汩摥ⴠ䌠湥牴䥥⁄湡⁤畃瑳浯獩瑡潩䥮⁄潤❮⁴慭捴൨ⴊ‭†††उ〱‱›慆汩摥ⴠ䌠湥牴䥥⁄湡⁤慃摮摩瑡䥥⁄潤❮⁴慭捴൨ഊⴊ‭㍖挠慨杮獥椠据畬敤ഺഊⴊ‭桃捥獫琠慨⁴硥獩楴杮瀠潲牧獥⁳慨湳琧戠敥⁮敒潭敶⁤牯删晥敲桳摥戠晥牯⁥敲畴楲楮杮攠牲牯മⴊ‭摁獤瀠牡浡瑥牥⁳潦⁲湅潲汬敭瑮洠瑥潨⁤湡⁤摡業⁮䑉਍ⴭ㴠㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽㴽਍䱁䕔⁒剐䍏䑅剕⁅摛潢⹝畛灳牃慥整牐杯敲獳敒潣摲噟崳਍䀉慃摮摩瑡䥥⁄湩ⱴ਍䀉畃瑳浯獩瑡潩䥮⁄湩ⱴ਍䀉敃瑮敲䑉椠瑮ബऊ䕀牮汯浬湥䵴瑥潨䥤⁄湩ⱴ਍䀉湅潲汬摥祂摁業䥮⁄湩൴䄊൓䈊䝅义਍ⴉ‭䕓⁔低佃乕⁔乏愠摤摥琠⁯牰癥湥⁴硥牴⁡敲畳瑬猠瑥⁳牦浯਍ⴉ‭湩整晲牥湩⁧楷桴匠䱅䍅⁔瑳瑡浥湥獴മऊ䕓⁔低佃乕⁔乏഻ऊⴭ਍ⴉ‭桔牥⁥牡⁥慶楲畯⁳桴湩獧琠 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DropApplyLPDefaultsSPChanges {
            get {
                return ResourceManager.GetString("DropApplyLPDefaultsSPChanges", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///DROP PROCEDURE [dbo].[GetFilteredCompetencyResponsesForCandidate]
        ///GO
        ///DROP PROCEDURE [dbo].[GetFilteredProfileForCandidate]
        ///GO
        ///DROP FUNCTION [dbo].[GetSelfAssessmentSummaryForCandidate]
        ///GO
        ///DROP FUNCTION [dbo].[GetFilteredAPISeniorityID]
        ///GO
        ///
        ///
        ///
        ///
        ///
        ///.
        /// </summary>
        internal static string DropFilteredSPs {
            get {
                return ResourceManager.GetString("DropFilteredSPs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DROP PROCEDURE [dbo].[GetActiveAvailableCustomisationsForCentreFiltered_V5]
        ///GO.
        /// </summary>
        internal static string DropGetActiveAvailableV5 {
            get {
                return ResourceManager.GetString("DropGetActiveAvailableV5", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /****** Object:  StoredProcedure [dbo].[ReorderTutorial]    Script Date: 04/01/2021 16:17:57 ******/
        ///DROP PROCEDURE [dbo].[ReorderFrameworkCompetency]
        ///GO
        ///DROP PROCEDURE [dbo].[ReorderFrameworkCompetencyGroup]
        ///GO
        ///
        ///.
        /// </summary>
        internal static string DropReorderFrameworkCompetenciesAndGroupsSPs {
            get {
                return ResourceManager.GetString("DropReorderFrameworkCompetenciesAndGroupsSPs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///CREATE FUNCTION [dbo].[GetSelfAssessmentSummaryForCandidate]
        ///(
        ///	@CandidateID int,
        ///	@SelfAssessmentID int
        ///)
        ///RETURNS @ResTable TABLE 
        ///(
        ///	CompetencyGroupID int,
        ///	Confidence float,
        ///	Relevance float
        ///)
        ///
        ///AS	  
        ///BEGIN
        ///INSERT INTO @ResTable
        ///	SELECT CompetencyGroupID, [1] AS Confidence, [2] AS Relevance
        ///FROM   (SELECT comp.CompetencyGroupID, sar.AssessmentQuestionID, sar.Result*1.0 AS Result
        ///             FROM    Competencies AS comp INNER JOIN
        ///                           SelfAssessmentResults AS sar [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string FilteredSPs {
            get {
                return ResourceManager.GetString("FilteredSPs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /****** Object:  StoredProcedure [dbo].[GetActiveAvailableCustomisationsForCentreFiltered_V5]    Script Date: 14/10/2020 10:02:34 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///
        ///-- =============================================
        ///-- Author:		Kevin Whittaker
        ///-- Create date: 05/10/2020
        ///-- Description:	Returns active available customisations for centre v5 adds SelfAssessments.
        ///-- =============================================
        ///CREATE PROCEDURE [dbo].[GetActiveAvailableCustomisationsForCentreF [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GetActiveAvailableV5 {
            get {
                return ResourceManager.GetString("GetActiveAvailableV5", resourceCulture);
            }
        }
    }
}
