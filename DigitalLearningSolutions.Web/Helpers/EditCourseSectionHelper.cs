namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using Microsoft.AspNetCore.Mvc;

    public static class EditCourseSectionHelper
    {
        public const string SelectAllDiagnosticAction = "diagnostic-select-all";
        public const string DeselectAllDiagnosticAction = "diagnostic-deselect-all";
        public const string SelectAllLearningAction = "learning-select-all";
        public const string DeselectAllLearningAction = "learning-deselect-all";

        public static IActionResult? ProcessBulkSelect(
            EditCourseSectionFormData model,
            string action
        )
        {
            switch (action)
            {
                case SelectAllDiagnosticAction:
                    SelectAllDiagnostics(model);
                    break;
                case DeselectAllDiagnosticAction:
                    DeselectAllDiagnostics(model);
                    break;
                case SelectAllLearningAction:
                    SelectAllLearning(model);
                    break;
                case DeselectAllLearningAction:
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
