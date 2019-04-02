using Amporis.Xamarin.Forms.ColorPicker;
using Plugin.Toasts;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using TimeIt.Controls;
using TimeIt.Interfaces;
using TimeIt.Pages.Dialogs;
using Xamarin.Forms;

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

        public void ShowNotification(string title, string msg)
        {
            var options = new NotificationOptions()
            {
                Title = title,
                Description = msg,
                IsClickable = false,
                AndroidOptions = new AndroidOptions
                {
                    HexColor = Color.Red.ToHex()                    
                }
            };
            _toastNotificator.Notify(options);
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

            await PopupNavigation.Instance.PushAsync(popup);

            var (result, isDismissed) = await popup.PageClosedTask;

            await PopupNavigation.Instance.PopAsync();

            return isDismissed ? false : result;
        }

        public async Task<double?> ShowSliderDialogAsync(
            string title,
            double currentValue,
            double min = 0,
            double max = 100,
            double steps = 1)
        {
            var dialog = new SliderDialog(title, min, max, steps, currentValue);
            var popup = new BaseDialog<double>(dialog);

            await PopupNavigation.Instance.PushAsync(popup);

            var (result, isDismissed) = await popup.PageClosedTask;

            await PopupNavigation.Instance.PopAsync();

            return isDismissed ? (double?)null : result;
        }
    }
}
