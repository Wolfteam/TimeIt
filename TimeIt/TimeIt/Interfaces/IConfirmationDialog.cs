using TimeIt.Delegates;

namespace TimeIt.Interfaces
{
    public interface IConfirmationDialog<T>
    {
        /// <summary>
        /// Event used to indicate that a button (ok / cancel / dismiss) was clicked
        /// </summary>
        OnConfirmDialogButtonClick<T> OnOptionSelected { get; set; }
    }
}
