using System.Threading.Tasks;

namespace TimeIt.Interfaces
{
    public interface ICustomDialogService
    {
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
    }
}
