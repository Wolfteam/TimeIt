using Plugin.Toasts;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using TimeIt.Controls;
using TimeIt.Interfaces;
using TimeIt.Pages.Dialogs;

namespace TimeIt.Services
{
    public class DialogService : ICustomDialogService
    {
        private readonly IToastNotificator _toastNotificator;
        private readonly ISimpleMessage _simpleMessage;

        public DialogService(IToastNotificator toastNotificator, ISimpleMessage simpleMessage)
        {
            _toastNotificator = toastNotificator;
            _simpleMessage = simpleMessage;
        }

        public async Task ShowNotification(string title, string msg)
        {
            var options = new NotificationOptions()
            {
                Title = title,
                Description = msg,
            };
            var result = await _toastNotificator.Notify(options);
        }

        public void ShowSimpleMessage(string message, bool longDelay = false)
        {
            _simpleMessage.ShowMessage(message, longDelay);
        }

        public async Task<bool> ShowConfirmationDialogAsync(string title, string message)
        {
            return await ShowConfirmationDialogAsync(title, message, "Ok", "Cancel");
        }

        public async Task<bool> ShowConfirmationDialogAsync(string title, string message, string okButtonText, string cancelButtonText)
        {
            var dialog = new ConfirmationDialog(title, message, okButtonText, cancelButtonText);
            var popup = new BaseDialog<bool>(dialog);
            dialog.OnOptionSelected = (optionSelected) =>
            {
                popup.PageClosedTaskCompletionSource.SetResult(optionSelected);
            };

            await PopupNavigation.Instance.PushAsync(popup);

            var result = await popup.PageClosedTask;

            await PopupNavigation.Instance.PopAsync();

            return result;
        }

    }
}
