using TimeIt.Delegates;

namespace TimeIt.Interfaces
{
    public interface IConfirmationDialog
    {
        /// <summary>
        /// Event used to indicate that a button (ok / cancel / dismiss) was clicked
        /// </summary>
        OnConfirmDialogButtonClick OnOptionSelected { get; set; }
    }
}
