using System.Threading.Tasks;

namespace TimeIt.Interfaces
{
    public interface ICustomDialogService
    {
        /// <summary>
        /// Shows a notification (android) or a 
        /// toast notification (uwp) 
        /// </summary>
        /// <param name="title">The title</param>
        /// <param name="message">The msg</param>
        /// <returns>Task</returns>
        Task ShowNotification(string title, string message);

        /// <summary>
        /// Shows a toast notification (android) or a
        /// flyout (uwp) with the message
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="longDelay">If true the notification will be visible for a little more time</param>
        void ShowSimpleMessage(string message, bool longDelay = false);

        /// <summary>
        /// Opens a modal confirmation dialog.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task&lt;System.Nullable&lt;System.Boolean&gt;&gt;.</returns>
        Task<bool> ShowConfirmationDialogAsync(string title, string message);

        /// <summary>
        /// Opens a modal confirmation dialog.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="yesButtonText">The 'Yes' button text.</param>
        /// <param name="noButtonText">The 'No' button text.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ShowConfirmationDialogAsync(string title, string message, string okButtonText, string cancelButtonText);

        /// <summary>
        /// Opens a modal with a slider whoose values
        /// are between <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="title">The title</param>
        /// <param name="currentValue">The current slider value</param>
        /// <param name="min">The min value</param>
        /// <param name="max">The max value</param>
        /// <param name="steps">The steps</param>
        /// <returns>If pressed ok button, it returns the selected value, otherwise null</returns>
        Task<double?> ShowSliderDialogAsync(
            string title,
            double currentValue,
            double min = 0,
            double max = 100,
            double steps = 1);
    }
}
