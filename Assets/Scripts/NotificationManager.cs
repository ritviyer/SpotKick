using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Notifications.Android;

public class NotificationManager : MonoBehaviour
{
    private void Start()
    {
        AndroidNotificationCenter.CancelAllNotifications();
        CreateNotificationChannel();
    }
    public void CreateNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Casual Notifications",
            Importance = Importance.High,
            Description = "Casual Notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        for (int i = 1; i < 9; i++)
        {
            if (i == 3 || i == 6)
                continue;
            DateTime notTime = DateTime.Today.AddDays(i).AddHours(21.5f);
            SendNotification(notTime);
        }
    }

    public void SendNotification(DateTime notificationTime)
    {
        var notification = new AndroidNotification();
        notification.Title = "Geared up and waiting for practice";
        notification.Text = "Don't let your player's skills get rusty.";
        notification.SmallIcon = "small_icon";
        notification.LargeIcon = "large_icon";
        notification.FireTime = notificationTime;

        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }
}