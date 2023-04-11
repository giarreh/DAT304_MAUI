﻿using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using System;
using static Android.Resource;


namespace MauiApp8.Platforms.Android.AndroidServices
{
    public class NotificationHelper
    {
        private readonly Context _context;

        public NotificationHelper(Context context)
        {
            _context = context;
        }
        private const string channelId = "my_notification_channel";
        private const string channelName = "My Notification Channel";
        private const string channelDescription = "Description for my notification channel.";

        public void CreateNotificationChannel()
        {
            var channel = new NotificationChannel(channelId, channelName, NotificationImportance.Default)
            {
                Description = channelDescription
            };

            var notificationManager = (NotificationManager)_context.GetSystemService(Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
        public void CreateNotification(string title, string message)
        {
            var notificationBuilder = new NotificationCompat.Builder(_context, channelId)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Resource.Drawable.trashcan)
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetAutoCancel(true);
            var notification = notificationBuilder.Build();

            
            var notificationManager = NotificationManagerCompat.From(_context);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
        //private readonly Context _context;

        //public NotificationHelper(Context context)
        //{
        //    _context = context;
        //}

        public void ShowNotification()
        {
            CreateNotificationChannel();

            var notification = new NotificationRequest
            {
                NotificationId = 666,
                Title = "NOTIFICATION",
                Subtitle = "WAR",
                Description = "IHOPE",
                BadgeNumber = 10,
                CategoryType = NotificationCategoryType.Alarm,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(5),
                    NotifyRepeatInterval = TimeSpan.FromSeconds(20),
                },
                Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions
                {

                    AutoCancel = true,
                    IconSmallName = new AndroidIcon("xwar")

                },

            };

            LocalNotificationCenter.Current.Show(notification);
        }

        //private void CreateNotificationChannel()
        //{
        //    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        //    {
        //        var channelName = "My Notification Channel";
        //        var channelDescription = "Notification channel for my app";
        //        var channel = new NotificationChannel("channel_id", channelName, NotificationImportance.Default)
        //        {
        //            Description = channelDescription
        //        };

        //        var notificationManager = (NotificationManager)_context.GetSystemService(Context.NotificationService);
        //        notificationManager.CreateNotificationChannel(channel);
        //    }
        //}
    }
}
