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
using System.Drawing;
using RedditService.Repository;
using RedditService.Models;
using RedditService;

namespace ImageConverter
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            CloudQueue queue = QueueHelper.GetQueueReference("vezba");
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("ImageConverter_WorkerRole entry point called", "Information");

            while (true)
            {
                CloudQueueMessage message = queue.GetMessage();
                if (message == null)
                {
                    Trace.TraceInformation("Trenutno ne postoji poruka u redu.", "Information");
                }
                else
                {
                    Trace.TraceInformation(String.Format("Poruka glasi: {0}", message.AsString), "Information");

                    if (message.DequeueCount > 3)
                    {
                        queue.DeleteMessage(message);
                    }

                    ResizeImage(message.AsString);

                    Trace.TraceInformation(String.Format("Poruka procesuirana: {0}", message.AsString), "Information");
                }

                Thread.Sleep(5000);
                Trace.TraceInformation("Working", "Information");
            }

        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("ImageConverter_WorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("ImageConverter_WorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("ImageConverter_WorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }

        
        public void ResizeImage(string email)
        {
            UserDataRepository udr = new UserDataRepository();
            User user = udr.GetUser(email);
            if (user == null)
            {
                Trace.TraceInformation(String.Format("User with email {0} does not exist!", email), "Information");
                return;
            }

            BlobHelper blobHelper = new BlobHelper();
            string uniqueBlobName = string.Format("image_{0}", user.RowKey);


            Image image = blobHelper.DownloadImage("vezba", uniqueBlobName);
            image = ImageConverters.ConvertImage(image);
            string thumbnailUrl = blobHelper.UploadImage(image, "vezba", uniqueBlobName + "thumb");

            user.ThumbnailUrl = thumbnailUrl;
            udr.UpdateUser(user);
        }
    }
}
