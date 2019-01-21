using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using TimeIt.Controls;
using TimeIt.Interfaces;
using TimeIt.Pages.Dialogs;

namespace TimeIt.Services
{
    public class DialogService : ICustomDialogService
    {
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
