namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using Microsoft.AspNetCore.Mvc;

    public static class EditCourseSectionHelper
    {
        public static IActionResult? ProcessBulkSelect(
            EditCourseSectionFormData model,
            string action
        )
        {
            switch (action)
            {
                case CourseSetupController.SelectAllDiagnosticAction:
                    SelectAllDiagnostics(model);
                    break;
                case CourseSetupController.DeselectAllDiagnosticAction:
                    DeselectAllDiagnostics(model);
                    break;
                case CourseSetupController.SelectAllLearningAction:
                    SelectAllLearning(model);
                    break;
                case CourseSetupController.DeselectAllLearningAction:
                    DeselectAllLearning(model);
                    break;
                default:
                    return new StatusCodeResult(400);
            }

            return null;
        }

        private static void SelectAllDiagnostics(EditCourseSectionFormData model)
        {
            foreach (var tutorial in model.Tutorials)
            {
                tutorial.DiagnosticEnabled = true;
            }
        }

        private static void DeselectAllDiagnostics(EditCourseSectionFormData model)
        {
            foreach (var tutorial in model.Tutorials)
            {
                tutorial.DiagnosticEnabled = false;
            }
        }

        private static void SelectAllLearning(EditCourseSectionFormData model)
        {
            foreach (var tutorial in model.Tutorials)
            {
                tutorial.LearningEnabled = true;
            }
        }

        private static void DeselectAllLearning(EditCourseSectionFormData model)
        {
            foreach (var tutorial in model.Tutorials)
            {
                tutorial.LearningEnabled = false;
            }
        }
    }
}
