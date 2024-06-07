using Common;
using DataLibrary.QueueHelper;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringService
{
    public class AdminEmailService : ISendAdminEmails
    {
        public static List<string> adminEmails = new List<string>();
        public void SendEmails(string email)
        {
            adminEmails.Add(email);
        }

        public static void AddEmailsToQueue()
        {
            foreach(var email in adminEmails)
            {
                CloudQueue queue = QueueHelper.GetQueueReference("adminnotificationqueue");
                queue.AddMessage(new CloudQueueMessage(email), null, TimeSpan.FromMilliseconds(30));
            }
            if (adminEmails.Count == 0) return;
            adminEmails.RemoveRange(0, adminEmails.Count - 1);
        }
    }
}
