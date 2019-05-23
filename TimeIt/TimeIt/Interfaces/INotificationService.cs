using System;

namespace TimeIt.Interfaces
{
    //TODO: THE NOTIFICATION SOUND GETS CUT WHEN YOU OPEN THE NOTIFICATION BAR/CENTER
    //TODO: CLICKING THE NOTIFICATION SHOULD OPEN THE ACTIVITY CORRECTLY
    public interface INotificationService
    {
        /// <summary>
        /// Show a local notification
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        /// <param name="soundNotificationPath">The path to the audio that will be played</param>
        void Show(string title, string body, string soundNotificationPath = null);

        /// <summary>
        /// Show a local notification at a specified time
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        /// <param name="notifyTime">Time to show notification</param>
        /// <param name="soundNotificationPath">The path to the audio that will be played</param>
        void Show(string title, string body, int id, DateTime deliveryOn, string soundNotificationPath = null);

        /// <summary>
        /// Cancel a local notification
        /// </summary>
        /// <param name="id">Id of the notification to cancel</param>
        void Cancel(int id);
    }
}
