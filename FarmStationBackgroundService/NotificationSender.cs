using FarmStation.Models.Db;
using FarmStationDb;
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
        if (notifications.Count == 0) Console.WriteLine("No notifications to send!");


        foreach (var notification in notifications) 
        {
            await SendNotificationAsync(notification);
            await Task.Delay(2000); // avoding getting blocked
            await _notificationRepository.RemoveNotification(notification);
        }
    }

    private async Task SendNotificationAsync(Notification notification)
    {
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            EnableSsl = true,
            Credentials = _emailCredentials 
        };
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
            From = new MailAddress("josea.system@gmail.com", "FarmStation")            
        };
        email.To.Add(new MailAddress(notification.User));
        await smtpClient.SendMailAsync(email);
    }



}
