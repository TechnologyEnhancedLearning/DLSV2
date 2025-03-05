using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.Common;
using DigitalLearningSolutions.Data.Models.Email;
using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Data.Models.Frameworks.Import;
using DigitalLearningSolutions.Data.Models.SelfAssessments;
using System.Collections.Generic;
using AssessmentQuestion = DigitalLearningSolutions.Data.Models.Frameworks.AssessmentQuestion;
using CompetencyResourceAssessmentQuestionParameter =
    DigitalLearningSolutions.Data.Models.Frameworks.CompetencyResourceAssessmentQuestionParameter;

namespace DigitalLearningSolutions.Web.Services
{
    public interface IFrameworkService
    {
        DashboardData? GetDashboardDataForAdminID(int adminId);

        IEnumerable<DashboardToDoItem> GetDashboardToDoItems(int adminId);

        DetailFramework? GetFrameworkDetailByFrameworkId(int frameworkId, int adminId);

        BaseFramework? GetBaseFrameworkByFrameworkId(int frameworkId, int adminId);

        BrandedFramework? GetBrandedFrameworkByFrameworkId(int frameworkId, int adminId);

        DetailFramework? GetDetailFrameworkByFrameworkId(int frameworkId, int adminId);
        IEnumerable<CompetencyFlag> GetSelectedCompetencyFlagsByCompetecyIds(int[] ids);
        IEnumerable<CompetencyFlag> GetSelectedCompetencyFlagsByCompetecyId(int competencyId);
        IEnumerable<CompetencyFlag> GetCompetencyFlagsByFrameworkId(int frameworkId, int? competencyId, bool? selected = null);
        IEnumerable<Flag> GetCustomFlagsByFrameworkId(int? frameworkId, int? flagId);

        IEnumerable<BrandedFramework> GetFrameworkByFrameworkName(string frameworkName, int adminId);

        IEnumerable<BrandedFramework> GetFrameworksForAdminId(int adminId);

        IEnumerable<BrandedFramework> GetAllFrameworks(int adminId);

        int GetAdminUserRoleForFrameworkId(int adminId, int frameworkId);

        string? GetFrameworkConfigForFrameworkId(int frameworkId);

        //  Collaborators:
        IEnumerable<CollaboratorDetail> GetCollaboratorsForFrameworkId(int frameworkId);

        CollaboratorNotification? GetCollaboratorNotification(int id, int invitedByAdminId);

        //  Competencies/groups:
        IEnumerable<FrameworkCompetencyGroup> GetFrameworkCompetencyGroups(int frameworkId);

        IEnumerable<FrameworkCompetency> GetFrameworkCompetenciesUngrouped(int frameworkId);

        CompetencyGroupBase? GetCompetencyGroupBaseById(int Id);

        FrameworkCompetency? GetFrameworkCompetencyById(int Id);

        int GetMaxFrameworkCompetencyID();

        int GetMaxFrameworkCompetencyGroupID();
        IEnumerable<BulkCompetency> GetBulkCompetenciesForFramework(int frameworkId);
        List<int> GetFrameworkCompetencyOrder(int frameworkId, List<int> frameworkCompetencyIds);

        //  Assessment questions:
        IEnumerable<AssessmentQuestion> GetAllCompetencyQuestions(int adminId);

        IEnumerable<AssessmentQuestion> GetFrameworkDefaultQuestionsById(int frameworkId, int adminId);

        IEnumerable<AssessmentQuestion> GetCompetencyAssessmentQuestionsByFrameworkCompetencyId(
            int frameworkCompetencyId,
            int adminId
        );

        IEnumerable<AssessmentQuestion> GetCompetencyAssessmentQuestionsById(int competencyId, int adminId);

        IEnumerable<GenericSelectList> GetAssessmentQuestionInputTypes();

        IEnumerable<GenericSelectList> GetAssessmentQuestions(int frameworkId, int adminId);

        FrameworkDefaultQuestionUsage GetFrameworkDefaultQuestionUsage(int frameworkId, int assessmentQuestionId);

        IEnumerable<GenericSelectList> GetAssessmentQuestionsForCompetency(int frameworkCompetencyId, int adminId);

        AssessmentQuestionDetail GetAssessmentQuestionDetailById(int assessmentQuestionId, int adminId);

        LevelDescriptor GetLevelDescriptorForAssessmentQuestionId(int assessmentQuestionId, int adminId, int level);

        IEnumerable<CompetencyResourceAssessmentQuestionParameter>
            GetSignpostingResourceParametersByFrameworkAndCompetencyId(int frameworkId, int competencyId);

        IEnumerable<LevelDescriptor> GetLevelDescriptorsForAssessmentQuestionId(
            int assessmentQuestionId,
            int adminId,
            int minValue,
            int maxValue,
            bool zeroBased
        );

        Competency? GetFrameworkCompetencyForPreview(int frameworkCompetencyId);

        //  Comments:
        IEnumerable<CommentReplies> GetCommentsForFrameworkId(int frameworkId, int adminId);

        CommentReplies? GetCommentRepliesById(int commentId, int adminId);

        Comment? GetCommentById(int adminId, int commentId);

        List<Recipient> GetCommentRecipients(int frameworkId, int adminId, int? replyToCommentId);

        // Reviews:
        IEnumerable<CollaboratorDetail> GetReviewersForFrameworkId(int frameworkId);

        IEnumerable<FrameworkReview> GetFrameworkReviewsForFrameworkId(int frameworkId);

        FrameworkReview? GetFrameworkReview(int frameworkId, int adminId, int reviewId);

        FrameworkReviewOutcomeNotification? GetFrameworkReviewNotification(int reviewId);

        //INSERT DATA
        BrandedFramework CreateFramework(DetailFramework detailFramework, int adminId);

        int InsertCompetencyGroup(string groupName, string? groupDescription, int adminId, int? frameworkId = null);

        int InsertFrameworkCompetency(int competencyId, int? frameworkCompetencyGroupID, int adminId, int frameworkId, bool alwaysShowDescription = false);

        IEnumerable<FrameworkCompetency> GetAllCompetenciesForAdminId(string name, int adminId);

        int InsertCompetency(string name, string? description, int adminId);

        int InsertFrameworkCompetencyGroup(int groupId, int frameworkID, int adminId);

        int AddCollaboratorToFramework(int frameworkId, string userEmail, bool canModify, int? centreID);

        int AddCustomFlagToFramework(int frameworkId, string flagName, string flagGroup, string flagTagClass);
        void UpdateFrameworkCustomFlag(int frameworkId, int id, string flagName, string flagGroup, string flagTagClass);

        void AddFrameworkDefaultQuestion(int frameworkId, int assessmentQuestionId, int adminId, bool addToExisting);

        CompetencyResourceAssessmentQuestionParameter?
            GetCompetencyResourceAssessmentQuestionParameterByCompetencyLearningResourceId(
                int competencyResourceAssessmentQuestionParameterId
            );

        LearningResourceReference? GetLearningResourceReferenceByCompetencyLearningResouceId(
            int competencyLearningResourceID
        );

        int EditCompetencyResourceAssessmentQuestionParameter(CompetencyResourceAssessmentQuestionParameter parameter);

        void AddCompetencyAssessmentQuestion(int frameworkCompetencyId, int assessmentQuestionId, int adminId);

        int InsertAssessmentQuestion(
            string question,
            int assessmentQuestionInputTypeId,
            string? maxValueDescription,
            string? minValueDescription,
            string? scoringInstructions,
            int minValue,
            int maxValue,
            bool includeComments,
            int adminId,
            string? commentsPrompt,
            string? commentsHint
        );

        int GetCompetencyAssessmentQuestionRoleRequirementsCount(int assessmentQuestionId, int competencyId);

        void InsertLevelDescriptor(
            int assessmentQuestionId,
            int levelValue,
            string levelLabel,
            string? levelDescription,
            int adminId
        );

        int InsertComment(int frameworkId, int adminId, string comment, int? replyToCommentId);

        void InsertFrameworkReview(int frameworkId, int frameworkCollaboratorId, bool required);

        int InsertFrameworkReReview(int reviewId);

        //UPDATE DATA
        BrandedFramework? UpdateFrameworkBranding(
            int frameworkId,
            int brandId,
            int categoryId,
            int topicId,
            int adminId
        );

        bool UpdateFrameworkName(int frameworkId, int adminId, string frameworkName);

        void UpdateFrameworkDescription(int frameworkId, int adminId, string? frameworkDescription);

        void UpdateFrameworkConfig(int frameworkId, int adminId, string? frameworkConfig);

        void UpdateFrameworkCompetencyGroup(
            int frameworkCompetencyGroupId,
            int competencyGroupId,
            string name,
            string? description,
            int adminId
        );

        void UpdateFrameworkCompetency(int frameworkCompetencyId, string name, string? description, int adminId, bool? alwaysShowDescription = false);
        int UpdateCompetencyFlags(int frameworkId, int competencyId, int[] selectedFlagIds);

        void MoveFrameworkCompetencyGroup(int frameworkCompetencyGroupId, bool singleStep, string direction);

        void MoveFrameworkCompetency(int frameworkCompetencyId, bool singleStep, string direction);

        void UpdateAssessmentQuestion(
            int id,
            string question,
            int assessmentQuestionInputTypeId,
            string? maxValueDescription,
            string? minValueDescription,
            string? scoringInstructions,
            int minValue,
            int maxValue,
            bool includeComments,
            int adminId,
            string? commentsPrompt,
            string? commentsHint
        );

        void UpdateLevelDescriptor(int id, int levelValue, string levelLabel, string? levelDescription, int adminId);

        void ArchiveComment(int commentId);

        void UpdateFrameworkStatus(int frameworkId, int statusId, int adminId);

        void SubmitFrameworkReview(int frameworkId, int reviewId, bool signedOff, int? commentId);

        void UpdateReviewRequestedDate(int reviewId);

        void ArchiveReviewRequest(int reviewId);

        void MoveCompetencyAssessmentQuestion(
            int competencyId,
            int assessmentQuestionId,
            bool singleStep,
            string direction
        );

        //Delete data
        void RemoveCustomFlag(int flagId);
        void RemoveCollaboratorFromFramework(int frameworkId, int id);

        void DeleteFrameworkCompetencyGroup(int frameworkCompetencyGroupId, int competencyGroupId, int adminId);

        void DeleteFrameworkCompetency(int frameworkCompetencyId, int adminId);

        void DeleteFrameworkDefaultQuestion(
            int frameworkId,
            int assessmentQuestionId,
            int adminId,
            bool deleteFromExisting
        );

        void DeleteCompetencyAssessmentQuestion(int frameworkCompetencyId, int assessmentQuestionId, int adminId);

        void DeleteCompetencyLearningResource(int competencyLearningResourceId, int adminId);
    }
    public class FrameworkService : IFrameworkService
    {
        private readonly IFrameworkDataService frameworkDataService;
        public FrameworkService(IFrameworkDataService frameworkDataService)
        {
            this.frameworkDataService = frameworkDataService;
        }

        public int AddCollaboratorToFramework(int frameworkId, string userEmail, bool canModify, int? centreID)
        {
            return frameworkDataService.AddCollaboratorToFramework(frameworkId, userEmail, canModify, centreID);
        }

        public void AddCompetencyAssessmentQuestion(int frameworkCompetencyId, int assessmentQuestionId, int adminId)
        {
            frameworkDataService.AddCompetencyAssessmentQuestion(frameworkCompetencyId, assessmentQuestionId, adminId);
        }

        public int AddCustomFlagToFramework(int frameworkId, string flagName, string flagGroup, string flagTagClass)
        {
            return frameworkDataService.AddCustomFlagToFramework(frameworkId, flagName, flagGroup, flagTagClass);
        }

        public void AddFrameworkDefaultQuestion(int frameworkId, int assessmentQuestionId, int adminId, bool addToExisting)
        {
            frameworkDataService.AddFrameworkDefaultQuestion(frameworkId, assessmentQuestionId, adminId, addToExisting);
        }

        public void ArchiveComment(int commentId)
        {
            frameworkDataService.ArchiveComment(commentId);
        }

        public void ArchiveReviewRequest(int reviewId)
        {
            frameworkDataService.ArchiveReviewRequest(reviewId);
        }

        public BrandedFramework CreateFramework(DetailFramework detailFramework, int adminId)
        {
            return frameworkDataService.CreateFramework(detailFramework, adminId);
        }

        public void DeleteCompetencyAssessmentQuestion(int frameworkCompetencyId, int assessmentQuestionId, int adminId)
        {
            frameworkDataService.DeleteCompetencyAssessmentQuestion(frameworkCompetencyId, assessmentQuestionId, adminId);
        }

        public void DeleteCompetencyLearningResource(int competencyLearningResourceId, int adminId)
        {
            frameworkDataService.DeleteCompetencyLearningResource(competencyLearningResourceId, adminId);
        }

        public void DeleteFrameworkCompetency(int frameworkCompetencyId, int adminId)
        {
            frameworkDataService.DeleteFrameworkCompetency(frameworkCompetencyId, adminId);
        }

        public void DeleteFrameworkCompetencyGroup(int frameworkCompetencyGroupId, int competencyGroupId, int adminId)
        {
            frameworkDataService.DeleteFrameworkCompetencyGroup(frameworkCompetencyGroupId, competencyGroupId, adminId);
        }

        public void DeleteFrameworkDefaultQuestion(int frameworkId, int assessmentQuestionId, int adminId, bool deleteFromExisting)
        {
            frameworkDataService.DeleteFrameworkDefaultQuestion(frameworkId, assessmentQuestionId, adminId, deleteFromExisting);
        }

        public int EditCompetencyResourceAssessmentQuestionParameter(CompetencyResourceAssessmentQuestionParameter parameter)
        {
            return frameworkDataService.EditCompetencyResourceAssessmentQuestionParameter(parameter);
        }

        public int GetAdminUserRoleForFrameworkId(int adminId, int frameworkId)
        {
            return frameworkDataService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
        }

        public IEnumerable<FrameworkCompetency> GetAllCompetenciesForAdminId(string name, int adminId)
        {
            return frameworkDataService.GetAllCompetenciesForAdminId(name, adminId);
        }

        public IEnumerable<AssessmentQuestion> GetAllCompetencyQuestions(int adminId)
        {
            return frameworkDataService.GetAllCompetencyQuestions(adminId);
        }

        public IEnumerable<BrandedFramework> GetAllFrameworks(int adminId)
        {
            return frameworkDataService.GetAllFrameworks(adminId);
        }

        public AssessmentQuestionDetail GetAssessmentQuestionDetailById(int assessmentQuestionId, int adminId)
        {
            return frameworkDataService.GetAssessmentQuestionDetailById(assessmentQuestionId, adminId);
        }

        public IEnumerable<GenericSelectList> GetAssessmentQuestionInputTypes()
        {
            return frameworkDataService.GetAssessmentQuestionInputTypes();
        }

        public IEnumerable<GenericSelectList> GetAssessmentQuestions(int frameworkId, int adminId)
        {
            return frameworkDataService.GetAssessmentQuestions(frameworkId, adminId);
        }

        public IEnumerable<GenericSelectList> GetAssessmentQuestionsForCompetency(int frameworkCompetencyId, int adminId)
        {
            return frameworkDataService.GetAssessmentQuestionsForCompetency(frameworkCompetencyId, adminId);
        }

        public BaseFramework? GetBaseFrameworkByFrameworkId(int frameworkId, int adminId)
        {
            return frameworkDataService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
        }

        public BrandedFramework? GetBrandedFrameworkByFrameworkId(int frameworkId, int adminId)
        {
            return frameworkDataService.GetBrandedFrameworkByFrameworkId(frameworkId, adminId);
        }

        public IEnumerable<BulkCompetency> GetBulkCompetenciesForFramework(int frameworkId)
        {
            return frameworkDataService.GetBulkCompetenciesForFramework(frameworkId);
        }

        public List<int> GetFrameworkCompetencyOrder(int frameworkId, List<int> frameworkCompetencyIds)
        {
            return frameworkDataService.GetFrameworkCompetencyOrder(frameworkId, frameworkCompetencyIds);
        }

        public CollaboratorNotification? GetCollaboratorNotification(int id, int invitedByAdminId)
        {
            return frameworkDataService.GetCollaboratorNotification(id, invitedByAdminId);
        }

        public IEnumerable<CollaboratorDetail> GetCollaboratorsForFrameworkId(int frameworkId)
        {
            return frameworkDataService.GetCollaboratorsForFrameworkId(frameworkId);
        }

        public Comment? GetCommentById(int adminId, int commentId)
        {
            return frameworkDataService.GetCommentById(adminId, commentId);
        }

        public List<Recipient> GetCommentRecipients(int frameworkId, int adminId, int? replyToCommentId)
        {
            return frameworkDataService.GetCommentRecipients(frameworkId, adminId, replyToCommentId);
        }

        public CommentReplies? GetCommentRepliesById(int commentId, int adminId)
        {
            return frameworkDataService.GetCommentRepliesById(commentId, adminId);
        }

        public IEnumerable<CommentReplies> GetCommentsForFrameworkId(int frameworkId, int adminId)
        {
            return frameworkDataService.GetCommentsForFrameworkId(frameworkId, adminId);
        }

        public int GetCompetencyAssessmentQuestionRoleRequirementsCount(int assessmentQuestionId, int competencyId)
        {
            return frameworkDataService.GetCompetencyAssessmentQuestionRoleRequirementsCount(assessmentQuestionId, competencyId);
        }

        public IEnumerable<AssessmentQuestion> GetCompetencyAssessmentQuestionsByFrameworkCompetencyId(int frameworkCompetencyId, int adminId)
        {
            return frameworkDataService.GetCompetencyAssessmentQuestionsByFrameworkCompetencyId(frameworkCompetencyId, adminId);
        }

        public IEnumerable<AssessmentQuestion> GetCompetencyAssessmentQuestionsById(int competencyId, int adminId)
        {
            return frameworkDataService.GetCompetencyAssessmentQuestionsById(competencyId, adminId);
        }

        public IEnumerable<CompetencyFlag> GetCompetencyFlagsByFrameworkId(int frameworkId, int? competencyId, bool? selected)
        {
            return frameworkDataService.GetCompetencyFlagsByFrameworkId(frameworkId, competencyId, selected);
        }

        public CompetencyGroupBase? GetCompetencyGroupBaseById(int Id)
        {
            return frameworkDataService.GetCompetencyGroupBaseById(Id);
        }

        public CompetencyResourceAssessmentQuestionParameter? GetCompetencyResourceAssessmentQuestionParameterByCompetencyLearningResourceId(int competencyResourceAssessmentQuestionParameterId)
        {
            return frameworkDataService.GetCompetencyResourceAssessmentQuestionParameterByCompetencyLearningResourceId(competencyResourceAssessmentQuestionParameterId);
        }

        public IEnumerable<Flag> GetCustomFlagsByFrameworkId(int? frameworkId, int? flagId)
        {
            return frameworkDataService.GetCustomFlagsByFrameworkId(frameworkId, flagId);
        }

        public DashboardData? GetDashboardDataForAdminID(int adminId)
        {
            return frameworkDataService.GetDashboardDataForAdminID(adminId);
        }

        public IEnumerable<DashboardToDoItem> GetDashboardToDoItems(int adminId)
        {
            return frameworkDataService.GetDashboardToDoItems(adminId);
        }

        public DetailFramework? GetDetailFrameworkByFrameworkId(int frameworkId, int adminId)
        {
            return frameworkDataService.GetDetailFrameworkByFrameworkId(frameworkId, adminId);
        }

        public IEnumerable<BrandedFramework> GetFrameworkByFrameworkName(string frameworkName, int adminId)
        {
            return frameworkDataService.GetFrameworkByFrameworkName(frameworkName, adminId);
        }

        public IEnumerable<FrameworkCompetency> GetFrameworkCompetenciesUngrouped(int frameworkId)
        {
            return frameworkDataService.GetFrameworkCompetenciesUngrouped(frameworkId);
        }

        public FrameworkCompetency? GetFrameworkCompetencyById(int Id)
        {
            return frameworkDataService.GetFrameworkCompetencyById(Id);
        }

        public Competency? GetFrameworkCompetencyForPreview(int frameworkCompetencyId)
        {
            return frameworkDataService.GetFrameworkCompetencyForPreview(frameworkCompetencyId);
        }

        public IEnumerable<FrameworkCompetencyGroup> GetFrameworkCompetencyGroups(int frameworkId)
        {
            return frameworkDataService.GetFrameworkCompetencyGroups(frameworkId);
        }

        public string? GetFrameworkConfigForFrameworkId(int frameworkId)
        {
            return frameworkDataService.GetFrameworkConfigForFrameworkId(frameworkId);
        }

        public IEnumerable<AssessmentQuestion> GetFrameworkDefaultQuestionsById(int frameworkId, int adminId)
        {
            return frameworkDataService.GetFrameworkDefaultQuestionsById(frameworkId, adminId);
        }

        public FrameworkDefaultQuestionUsage GetFrameworkDefaultQuestionUsage(int frameworkId, int assessmentQuestionId)
        {
            return frameworkDataService.GetFrameworkDefaultQuestionUsage(frameworkId, assessmentQuestionId);
        }

        public DetailFramework? GetFrameworkDetailByFrameworkId(int frameworkId, int adminId)
        {
            return frameworkDataService.GetFrameworkDetailByFrameworkId(frameworkId, adminId);
        }

        public FrameworkReview? GetFrameworkReview(int frameworkId, int adminId, int reviewId)
        {
            return frameworkDataService.GetFrameworkReview(frameworkId, adminId, reviewId);
        }

        public FrameworkReviewOutcomeNotification? GetFrameworkReviewNotification(int reviewId)
        {
            return frameworkDataService.GetFrameworkReviewNotification(reviewId);
        }

        public IEnumerable<FrameworkReview> GetFrameworkReviewsForFrameworkId(int frameworkId)
        {
            return frameworkDataService.GetFrameworkReviewsForFrameworkId(frameworkId);
        }

        public IEnumerable<BrandedFramework> GetFrameworksForAdminId(int adminId)
        {
            return frameworkDataService.GetFrameworksForAdminId(adminId);
        }

        public LearningResourceReference? GetLearningResourceReferenceByCompetencyLearningResouceId(int competencyLearningResourceID)
        {
            return frameworkDataService.GetLearningResourceReferenceByCompetencyLearningResouceId(competencyLearningResourceID);
        }

        public LevelDescriptor GetLevelDescriptorForAssessmentQuestionId(int assessmentQuestionId, int adminId, int level)
        {
            return frameworkDataService.GetLevelDescriptorForAssessmentQuestionId(assessmentQuestionId, adminId, level);
        }

        public IEnumerable<LevelDescriptor> GetLevelDescriptorsForAssessmentQuestionId(int assessmentQuestionId, int adminId, int minValue, int maxValue, bool zeroBased)
        {
            return frameworkDataService.GetLevelDescriptorsForAssessmentQuestionId(assessmentQuestionId, adminId, minValue, maxValue, zeroBased);
        }

        public int GetMaxFrameworkCompetencyGroupID()
        {
            return frameworkDataService.GetMaxFrameworkCompetencyGroupID();
        }

        public int GetMaxFrameworkCompetencyID()
        {
            return frameworkDataService.GetMaxFrameworkCompetencyID();
        }

        public IEnumerable<CollaboratorDetail> GetReviewersForFrameworkId(int frameworkId)
        {
            return frameworkDataService.GetReviewersForFrameworkId(frameworkId);
        }

        public IEnumerable<CompetencyFlag> GetSelectedCompetencyFlagsByCompetecyId(int competencyId)
        {
            return frameworkDataService.GetSelectedCompetencyFlagsByCompetecyId(competencyId);
        }

        public IEnumerable<CompetencyFlag> GetSelectedCompetencyFlagsByCompetecyIds(int[] ids)
        {
            return frameworkDataService.GetSelectedCompetencyFlagsByCompetecyIds(ids);
        }

        public IEnumerable<CompetencyResourceAssessmentQuestionParameter> GetSignpostingResourceParametersByFrameworkAndCompetencyId(int frameworkId, int competencyId)
        {
            return frameworkDataService.GetSignpostingResourceParametersByFrameworkAndCompetencyId(frameworkId, competencyId);
        }

        public int InsertAssessmentQuestion(string question, int assessmentQuestionInputTypeId, string? maxValueDescription, string? minValueDescription, string? scoringInstructions, int minValue, int maxValue, bool includeComments, int adminId, string? commentsPrompt, string? commentsHint)
        {
            return frameworkDataService.InsertAssessmentQuestion(question, assessmentQuestionInputTypeId, maxValueDescription, minValueDescription, scoringInstructions, minValue, maxValue, includeComments, adminId, commentsPrompt, commentsHint);
        }

        public int InsertComment(int frameworkId, int adminId, string comment, int? replyToCommentId)
        {
            return frameworkDataService.InsertComment(frameworkId, adminId, comment, replyToCommentId);
        }

        public int InsertCompetency(string name, string? description, int adminId)
        {
            return frameworkDataService.InsertCompetency(name, description, adminId);
        }

        public int InsertCompetencyGroup(string groupName, string? groupDescription, int adminId, int? frameworkId)
        {
            return frameworkDataService.InsertCompetencyGroup(groupName, groupDescription, adminId, frameworkId);
        }

        public int InsertFrameworkCompetency(int competencyId, int? frameworkCompetencyGroupID, int adminId, int frameworkId, bool alwaysShowDescription = false)
        {
            return frameworkDataService.InsertFrameworkCompetency(competencyId, frameworkCompetencyGroupID, adminId, frameworkId, alwaysShowDescription);
        }

        public int InsertFrameworkCompetencyGroup(int groupId, int frameworkID, int adminId)
        {
            return frameworkDataService.InsertFrameworkCompetencyGroup(groupId, frameworkID, adminId);
        }

        public int InsertFrameworkReReview(int reviewId)
        {
            return frameworkDataService.InsertFrameworkReReview(reviewId);
        }

        public void InsertFrameworkReview(int frameworkId, int frameworkCollaboratorId, bool required)
        {
            frameworkDataService.InsertFrameworkReview(frameworkId, frameworkCollaboratorId, required);
        }

        public void InsertLevelDescriptor(int assessmentQuestionId, int levelValue, string levelLabel, string? levelDescription, int adminId)
        {
            frameworkDataService.InsertLevelDescriptor(assessmentQuestionId, levelValue, levelLabel, levelDescription, adminId);
        }

        public void MoveCompetencyAssessmentQuestion(int competencyId, int assessmentQuestionId, bool singleStep, string direction)
        {
            frameworkDataService.MoveCompetencyAssessmentQuestion(competencyId, assessmentQuestionId, singleStep, direction);
        }

        public void MoveFrameworkCompetency(int frameworkCompetencyId, bool singleStep, string direction)
        {
            frameworkDataService.MoveFrameworkCompetency(frameworkCompetencyId, singleStep, direction);
        }

        public void MoveFrameworkCompetencyGroup(int frameworkCompetencyGroupId, bool singleStep, string direction)
        {
            frameworkDataService.MoveFrameworkCompetencyGroup(frameworkCompetencyGroupId, singleStep, direction);
        }

        public void RemoveCollaboratorFromFramework(int frameworkId, int id)
        {
            frameworkDataService.RemoveCollaboratorFromFramework(frameworkId, id);
        }

        public void RemoveCustomFlag(int flagId)
        {
            frameworkDataService.RemoveCustomFlag(flagId);
        }

        public void SubmitFrameworkReview(int frameworkId, int reviewId, bool signedOff, int? commentId)
        {
            frameworkDataService.SubmitFrameworkReview(frameworkId, reviewId, signedOff, commentId);
        }

        public void UpdateAssessmentQuestion(int id, string question, int assessmentQuestionInputTypeId, string? maxValueDescription, string? minValueDescription, string? scoringInstructions, int minValue, int maxValue, bool includeComments, int adminId, string? commentsPrompt, string? commentsHint)
        {
            frameworkDataService.UpdateAssessmentQuestion(id, question, assessmentQuestionInputTypeId, maxValueDescription, minValueDescription, scoringInstructions, minValue, maxValue, includeComments, adminId, commentsPrompt, commentsHint);
        }

        public int UpdateCompetencyFlags(int frameworkId, int competencyId, int[] selectedFlagIds)
        {
            return frameworkDataService.UpdateCompetencyFlags(frameworkId, competencyId, selectedFlagIds);
        }

        public BrandedFramework? UpdateFrameworkBranding(int frameworkId, int brandId, int categoryId, int topicId, int adminId)
        {
            return frameworkDataService.UpdateFrameworkBranding(frameworkId, brandId, categoryId, topicId, adminId);
        }

        public void UpdateFrameworkCompetency(int frameworkCompetencyId, string name, string? description, int adminId, bool? alwaysShowDescription)
        { 
            frameworkDataService.UpdateFrameworkCompetency(frameworkCompetencyId, name, description, adminId, alwaysShowDescription);
        }

        public void UpdateFrameworkCompetencyGroup(int frameworkCompetencyGroupId, int competencyGroupId, string name, string? description, int adminId)
        {
            frameworkDataService.UpdateFrameworkCompetencyGroup(frameworkCompetencyGroupId, competencyGroupId, name, description, adminId);
        }

        public void UpdateFrameworkConfig(int frameworkId, int adminId, string? frameworkConfig)
        {
            frameworkDataService.UpdateFrameworkConfig(frameworkId, adminId, frameworkConfig);
        }

        public void UpdateFrameworkCustomFlag(int frameworkId, int id, string flagName, string flagGroup, string flagTagClass)
        {
            frameworkDataService.UpdateFrameworkCustomFlag(frameworkId, id, flagName, flagGroup, flagTagClass);
        }

        public void UpdateFrameworkDescription(int frameworkId, int adminId, string? frameworkDescription)
        {
            frameworkDataService.UpdateFrameworkDescription(frameworkId, adminId, frameworkDescription);
        }

        public bool UpdateFrameworkName(int frameworkId, int adminId, string frameworkName)
        {
            return frameworkDataService.UpdateFrameworkName(frameworkId, adminId, frameworkName);
        }

        public void UpdateFrameworkStatus(int frameworkId, int statusId, int adminId)
        {
            frameworkDataService.UpdateFrameworkStatus(frameworkId, statusId, adminId);
        }

        public void UpdateLevelDescriptor(int id, int levelValue, string levelLabel, string? levelDescription, int adminId)
        {
            frameworkDataService.UpdateLevelDescriptor(id, levelValue, levelLabel, levelDescription, adminId);
        }

        public void UpdateReviewRequestedDate(int reviewId)
        {
            frameworkDataService.UpdateReviewRequestedDate(reviewId);
        }
    }
}
