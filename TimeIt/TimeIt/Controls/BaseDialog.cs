using Rg.Plugins.Popup.Pages;
using System.Threading.Tasks;
using TimeIt.Interfaces;
using Xamarin.Forms;

namespace TimeIt.Controls
{
    public class BaseDialog<T> : PopupPage
    {
        // the awaitable task
        public Task<T> PageClosedTask
            => PageClosedTaskCompletionSource.Task;

        // the task completion source
        public TaskCompletionSource<T> PageClosedTaskCompletionSource { get; set; }

        public BaseDialog(IConfirmationDialog contentBody)
        {
            Content = contentBody as View;

            // init the task completion source
            PageClosedTaskCompletionSource = new TaskCompletionSource<T>();
        }

        protected override bool OnBackButtonPressed()
        {
            // Prevent back button pressed action on android (return true)
            (Content as IConfirmationDialog).OnOptionSelected?.Invoke(false);
            return base.OnBackButtonPressed();
        }

        // Invoced when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Prevent background clicked action(return false)
            (Content as IConfirmationDialog).OnOptionSelected?.Invoke(false);
            return base.OnBackgroundClicked();
        }
    }
}
