using FarmStation.Models.Db;
using FarmStationDb;
using FarmStationDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FarmStationMessager;

public class NotificationSender
{
    private readonly NotificationRepository _notificationRepository;
    private readonly NetworkCredential _emailCredentials;

    public NotificationSender(NotificationRepository notificationRepository,
                            NetworkCredential emailCredentials)
    {
        _notificationRepository = notificationRepository;
        _emailCredentials = emailCredentials;
    }

    public async Task SendNotificationsAsync()
    {
        var tasks = new List<Task>();
        var notifications = await _notificationRepository.GetNotificationsAsync();
      
        // https://stackoverflow.com/questions/12337671/using-async-await-for-multiple-tasks
        //await Parallel.ForEachAsync(notifications,
        //    async (notification, token) => 
        // Not done in parallel because Google shutdown's the service (spam).        

        var sentNotifications = false;
        // loop for individual notifications. 
        foreach (var notification in notifications.Where( n => n.Type != "block" || n.Name?.ToLower() == "xch") )
        {
            await SendNotificationAsync(notification);
            await Task.Delay(2000); // avoding getting blocked
            await _notificationRepository.RemoveNotification(notification);
			sentNotifications = true; 
		}

		var cutoffCooldownBlockNotifications = DateTime.UtcNow.AddDays(-1); // groups block notifications for minor currencies by 24 hours

        var gNotifications = notifications
           .Where(n => n.Type == "block" && n.Name?.ToLower() != "xch")
           .GroupBy(n => new { n.User, n.Type, n.Name })
           .Select(p => new GroupedNotification
           {
               User = p.Key.User!,
               Type = p.Key.Type!,
               Name = p.Key.Name!,
               MinTimeStamp = p.Min(groupnotification => groupnotification.TimeStamp),
               Count = p.Count()
           }
           );

        foreach (var gNotification in gNotifications.Where(gn => gn.MinTimeStamp < cutoffCooldownBlockNotifications))
        {
			await SendGroupNotificationAsync(gNotification);
			await Task.Delay(2000); // avoding getting blocked

            foreach (var notification in notifications.Where(n => n.User == gNotification.User && n.Type == gNotification.Type && n.Name == gNotification.Name))
            {				
                await _notificationRepository.RemoveNotification(notification);
            }
			sentNotifications = true;
		}

        var countNotificationsCoolingDown = gNotifications.Where(gn => !(gn.MinTimeStamp < cutoffCooldownBlockNotifications)).Sum(gn => gn.Count);

        if (countNotificationsCoolingDown > 0)
		{
			Console.WriteLine($"{countNotificationsCoolingDown} notifications cooling down - not sent.");
		}

		if (!sentNotifications) {
            if(notifications.Count == 0) Console.WriteLine("No notifications found. No notifications to send!");
            else Console.WriteLine("No notifications to send!");
		}
	}

    private async Task SendGroupNotificationAsync(GroupedNotification notification)
    {
        SmtpClient smtpClient = GetSmtpClient();
        string subject, message;

        switch (notification.Type)
        {
            case "block":
                var plural = notification.Count > 1 ? "s" : "";
                subject = $"{notification.Name} found {notification.Count} block{plural}!";
                message = $"🤑 {notification.Name} found {notification.Count} block{plural}!";
                break;
			default:
				subject = $"{notification.Name} unknown notification {notification.Count}X";
				message = $"{notification.Type} {notification.Count}X";
				break;
		}
		Console.WriteLine($"Sending notification ({notification.Count}X-{notification.Name}-{notification.Type}) to {notification.User}");

		var email = new MailMessage
		{
			Subject = subject,
			Body = message,
			From = new MailAddress(_emailCredentials.UserName, "FarmStation")
		};
		email.To.Add(new MailAddress(notification.User!));
		await smtpClient.SendMailAsync(email);
	}


    private async Task SendNotificationAsync(Notification notification)
    {
        SmtpClient smtpClient = GetSmtpClient();
        string subject, message;

        switch (notification.Type)
        {
            case "block":
                subject = $"{notification.Name} just found a block!";
                message = $"🤑 {notification.Name} just found a block!";
                break;
            case "coldBlock":
                subject = $"{notification.Name} just received funds";
                message = $"\U0001f976 {notification.Name} just received funds. Is it a block?";
                break;
            case "plot":
                subject = $"{notification.Name} just completed another plot.";
                message = $"🤑 {notification.Name} just completed another plot.";
                break;
            case "offline":
                subject = $"Lost connection to {notification.Name}!";
                message = $"☠️ Lost connection to {notification.Name}!";
                break;
            case "online":
                subject = $"{notification.Name} has reconnected!";
                message = $"😊 {notification.Name} has reconnected!";
                break;
            case "stopped":
                subject = $"{notification.Name} stopped farming/harvesting!";
                message = $"😱 {notification.Name} stopped farming/harvesting!";
                break;
            case "started":
                subject = $"{notification.Name} started farming/harvesting!";
                message = $"😎 {notification.Name} started farming/harvesting!";
                break;
            case "drive":
                subject = $"{notification.Name} lost one of its drives!";
                message = $"💿 {notification.Name} lost one of its drives!";
                break;
            default:
                subject = $"{notification.Name} unknown notification";
                message = $"{notification.Type}";
                break;
        }

        Console.WriteLine($"Sending notification ({notification.NotificationId}-{notification.Name}-{notification.Type}) to {notification.User}");

        var email = new MailMessage
        {
            Subject = subject,
            Body = message,
            From = new MailAddress(_emailCredentials.UserName, "FarmStation")
        };
        email.To.Add(new MailAddress(notification.User!));
        await smtpClient.SendMailAsync(email);
    }

    private SmtpClient GetSmtpClient()
    {
        return new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            EnableSsl = true,
            Credentials = _emailCredentials
        };
    }


}
