using System.ComponentModel;

namespace NeuralNet_Core
{
    public interface INotificationChanged : INotifyPropertyChanged
    {
        string Notification { get; set; }
    }
    public class NotificationChangedBase : PropertyChangedBase, INotificationChanged
    {
        #region INotificationChanged

        string notification;

        public string Notification
        {
            get
            {
                return notification;
            }
            set
            {
                // No equality check here since repeated identic statuses are possible.
                notification = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
