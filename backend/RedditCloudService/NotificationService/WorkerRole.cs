using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using DataLibrary.QueueHelper;
using DataLibrary.Comment;
using DataLibrary.UserData;
using Common;
using System.ServiceModel;
using PostmarkDotNet;
using PostmarkDotNet.Model;
using DataLibrary.NotificationData;

namespace NotificationService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        CommentDataRepo commentDataRepo = new CommentDataRepo();
        ThemeSubscribersDataRepo themeSubscriberRepo = new ThemeSubscribersDataRepo();
        NotificationDataRepo NotificationDataRepo = new NotificationDataRepo();

        private ServiceHost serviceHost;

        public object ConfigurationManager { get; private set; }

        public override void Run()
        {
            Trace.TraceInformation("NotificationService is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            serviceHost = new ServiceHost(typeof(HealthStatusCheck));
            NetTcpBinding binding = new NetTcpBinding();
            serviceHost.AddServiceEndpoint(typeof(IHealthMonitoring), binding, new
            Uri("net.tcp://localhost:6001/HealthMonitoring"));
            serviceHost.Open();
            Console.WriteLine("Server ready and waiting for requests.");

            bool result = base.OnStart();

            Trace.TraceInformation("NotificationService has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("NotificationService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("NotificationService has stopped");
        }

        private async Task ReadFromQueueAndSendMail()
        {
            CloudQueue queue = QueueHelper.GetQueueReference("commentnotificationqueue");
            CloudQueueMessage message = await queue.GetMessageAsync();
            int emailsSend = 0;
            if(message == null || message.AsString == "")
            {
                return;
            }
            else if(message.DequeueCount == 1)
            {
                string commentId = message.AsString;
                queue.DeleteMessage(message);
                Comment comment = commentDataRepo.GetComment(commentId);
                string themeTitle = comment.ThemeTitle;
                List<string> subscribersEmail = themeSubscriberRepo.RetrieveAllThemeSubscribers().Where(ts => ts.ThemeTitle == themeTitle).Select(ts => ts.UserEmail).ToList();
                foreach (var email in subscribersEmail)
                {
                   emailsSend += await SendEmail(email, comment.Content);
                }

                WriteCommentTraceToTable(commentId, emailsSend);
            }
        }

        public async Task SendEmailsToAdmin()
        {
            CloudQueue queue = QueueHelper.GetQueueReference("adminnotificationqueue");
            CloudQueueMessage message = await queue.GetMessageAsync();

            if (message == null || message.AsString == "")
            {
                return;
            }
            else if (message.DequeueCount == 1)
            {
                string email = message.AsString;
                queue.DeleteMessage(message);

                await SendEmail(email, "Warning! Reddit Service is down!");
            }
        }

        private async Task<int> SendEmail(string email, string content)
        {
            int counter = 0;
            Trace.TraceWarning($"Saljem Email na {email} sa sadrzajem {content}");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Send an email asynchronously:
            var emailMessage = new PostmarkMessage()
            {
                To = email,
                From = "radojicic.pr142.2020@uns.ac.rs",
                TrackOpens = true,
                Subject = "Reddit test",
                TextBody = $"{content}",
                HtmlBody = "HTML goes here",
                Tag = "New Year's Email Campaign",
                Headers = new HeaderCollection
                {
                   new MailHeader() {Name="X-CUSTOM-HEADER", Value="Header content"},

                }
            };

            string postmark_api_token = System.Configuration.ConfigurationManager.AppSettings["postmark_api"];
            var client = new PostmarkClient("ee60dd27-9290-4176-8024-424e3ef77fd7");
            var sendResult = await client.SendMessageAsync(emailMessage);

            if (sendResult.Status == PostmarkStatus.Success) { counter = 1; }
            else { /* Resolve issue.*/ }

            return counter;

        }

        private void WriteCommentTraceToTable(string commentId, int emailCounter)
        {
            string guid = Guid.NewGuid().ToString();
            NotificationData notificationData = new NotificationData(guid) { CommentId = commentId, EmailCounter = emailCounter};
            NotificationDataRepo.AddNotificationData(notificationData);
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await ReadFromQueueAndSendMail();
                    await SendEmailsToAdmin();
                }
                catch(Exception e)
                {
                    Trace.TraceWarning(e.ToString());
                }
                Trace.TraceInformation("Working");
                await Task.Delay(10000);
            }
        }
    }
}
