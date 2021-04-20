using System;

namespace PlanningPoker.Client.Notifications
{
    public class Notification
    {
        public Notification(NotificationType type)
        {
            Type = type;
            switch (type)
            {
                case NotificationType.Closed:
                    Text = "Disconnected";
                    Icon = "oi-circle-x";
                    AlertClass = "alert-danger";
                    break;
                case NotificationType.Reconnecting:
                    Text = "Reconnecting";
                    Icon = "oi-ellipses";
                    AlertClass = "alert-warning";
                    break;
                case NotificationType.Connected:
                    Text = "Connected";
                    Icon = "oi-circle-check";
                    AlertClass = "alert-success";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public string Icon { get; }

        public string Text { get; }

        public string AlertClass { get; }

        public NotificationType Type { get; }
    }
}