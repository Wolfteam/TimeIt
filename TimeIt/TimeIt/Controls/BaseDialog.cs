using Rg.Plugins.Popup.Pages;
using System.Threading.Tasks;
using TimeIt.Interfaces;
using Xamarin.Forms;

namespace TimeIt.Controls
{
    public class BaseDialog<T> : PopupPage
    {
        // the awaitable task
        public Task<(T, bool)> PageClosedTask
            => PageClosedTaskCompletionSource.Task;

        // the task completion source
        public TaskCompletionSource<(T, bool)> PageClosedTaskCompletionSource { get; set; }

        public BaseDialog(IConfirmationDialog<T> contentBody)
        {
            Content = contentBody as View;
            // init the task completion source
            PageClosedTaskCompletionSource = new TaskCompletionSource<(T, bool)>();
            contentBody.OnOptionSelected = (result, isDismissed)
                => PageClosedTaskCompletionSource.SetResult((result, isDismissed));
        }

        protected override bool OnBackButtonPressed()
        {
            // Prevent back button pressed action on android (return true)
            (Content as IConfirmationDialog<T>).OnOptionSelected?.Invoke(default(T), true);
            return base.OnBackButtonPressed();
        }

        // Invoced when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Prevent background clicked action(return false)
            (Content as IConfirmationDialog<T>).OnOptionSelected?.Invoke(default(T), true);
            return base.OnBackgroundClicked();
        }
    }
}
