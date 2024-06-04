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

namespace NotificationService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        CommentDataRepo commentDataRepo = new CommentDataRepo();
        ThemeSubscribersDataRepo themeSubscriberRepo = new ThemeSubscribersDataRepo();

        private ServiceHost serviceHost;

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

            string commentId = message.AsString;

            Comment comment = commentDataRepo.GetComment(commentId);

            string themeTitle = comment.ThemeTitle;

            List<string> subscribersEmail = themeSubscriberRepo.RetrieveAllThemeSubscribers().Where(ts => ts.ThemeTitle == themeTitle).Select(ts => ts.UserEmail).ToList();

            foreach(var email in subscribersEmail)
            {
                SendEmail(email, comment.Content);
                WriteCommentTraceToTable();
            }
        }

        private void SendEmail(string email, string content)
        {
            Trace.TraceWarning($"Saljem Email na {email} sa sadrzajem {content}");


        }

        private void WriteCommentTraceToTable()
        {

        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await ReadFromQueueAndSendMail();
                }
                catch
                {

                }
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}
