using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Linq;
using TimeIt.Enums;
using TimeIt.Interfaces;
using TimeIt.UWP.Implementations;
using Windows.UI.Notifications;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationService))]
namespace TimeIt.UWP.Implementations
{
    public class NotificationService : INotificationService
    {
        public void Cancel(int id)
        {
            var scheduledToast = ToastNotificationManager
                .CreateToastNotifier()
                .GetScheduledToastNotifications()
                .FirstOrDefault(st => st.Id == $"{id}");

            if (scheduledToast != null)
                ToastNotificationManager.CreateToastNotifier().RemoveFromSchedule(scheduledToast);
        }

        public void CancelAll()
        {
            var scheduledToasts = ToastNotificationManager
                .CreateToastNotifier()
                .GetScheduledToastNotifications();

            foreach (var toast in scheduledToasts)
            {
                ToastNotificationManager.CreateToastNotifier().RemoveFromSchedule(toast);
            }
        }

        public void Show(string title, string body, SoundType? soundType)
        {
            var content = GenerateSimpleToastContent(title, body, GetSoundPath(soundType));
            var toastNotifcation = new ToastNotification(content.GetXml());
            ShowToastNotification(toastNotifcation);
        }

        public void Show(
            string title, 
            string body, 
            int id, 
            DateTime deliveryOn,
            SoundType? soundType)
        {
            bool showNow = deliveryOn <= DateTime.Now;

            if (showNow)
            {
                Show(title, body, soundType);
                return;
            }

            var content = GenerateSimpleToastContent(title, body, GetSoundPath(soundType));
            var toastNotification = new ScheduledToastNotification(content.GetXml(), deliveryOn)
            {
                Id = $"{id}",
                //Tag = "TaskReminderToastTag"
            };
            ScheduleToastNotification(toastNotification);
        }

        private ToastContent GenerateSimpleToastContent(
            string title,
            string content,
            string soundPath = null)
        {
            var toastContent =  new ToastContent()
            {
                Scenario = ToastScenario.Default,
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = title
                            },
                            new AdaptiveText()
                            {
                                Text = content
                            }
                        }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButtonDismiss()
                    }
                }
            };

            if (!string.IsNullOrEmpty(soundPath))
            {
                toastContent.Audio = new ToastAudio
                {
                    Src = new Uri(soundPath)
                };
                //the sound gets cut if we use a scenario that hides the toast
                toastContent.Scenario = ToastScenario.Reminder;
            }
                

            return toastContent;
        }

        private void ShowToastNotification(ToastNotification toast)
            => ToastNotificationManager.CreateToastNotifier().Show(toast);

        private void ScheduleToastNotification(ScheduledToastNotification scheduledToast)
            => ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduledToast);

        private string GetSoundPath(SoundType? soundType)
        {
            var soundProvider = Xamarin.Forms.DependencyService.Get<INotificationSoundProvider>();
            string soundNotificationPath = soundType.HasValue
                ? soundProvider.GetSoundPath(soundType.Value)
                : null;

            return soundNotificationPath;
        }
    }
}
