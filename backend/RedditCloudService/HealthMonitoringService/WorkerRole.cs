using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Common;
using DataLibrary.HealthCheck;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace HealthMonitoringService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        HealthCheckRepo healthCheckRepo = new HealthCheckRepo();

        private IHealthMonitoring redditProxy;
        private IHealthMonitoring notificationProxy;

        public override void Run()
        {
            Trace.TraceInformation("HealthMonitoringService is running");

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

            bool result = base.OnStart();

            Trace.TraceInformation("HealthMonitoringService has been started");

            return result;
        }

        public override void OnStop()
        {
            ConnectToRedditService();
            Trace.TraceInformation("HealthMonitoringService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("HealthMonitoringService has stopped");
        }

        public void ConnectToRedditService()
        {
            var binding = new NetTcpBinding();
            ChannelFactory<IHealthMonitoring> factory = new
            ChannelFactory<IHealthMonitoring>(binding, new
            EndpointAddress("net.tcp://localhost:6000/HealthMonitoring"));
            redditProxy = factory.CreateChannel();

            //Trace.WriteLine("Proxy: " + redditProxy.ToString());
        }

        public void ConnectToNotificationService()
        {
            var binding = new NetTcpBinding();
            ChannelFactory<IHealthMonitoring> factory = new
            ChannelFactory<IHealthMonitoring>(binding, new
            EndpointAddress("net.tcp://localhost:6001/HealthMonitoring"));
            notificationProxy = factory.CreateChannel();

            //Trace.WriteLine("Proxy: " + redditProxy.ToString());
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if(redditProxy == null) ConnectToRedditService();
                    if (notificationProxy == null) ConnectToNotificationService();

                    if (redditProxy.CheckStatus())
                    {
                        //upisi u tabelu OK
                        Trace.TraceWarning("RSMVC is WORKING!");
                        string guid = Guid.NewGuid().ToString();
                        HealthCheck healthCheck = new HealthCheck(guid) { Status = "OK", ServiceName = "RSMVC" };
                        //healthCheckRepo.AddHealthCheck(healthCheck); //OTKOMENTARISI
                    }
                    else
                    {
                        //upisi u tabelu NotOK
                        Trace.TraceWarning("RSMVC is NOT WORKING");
                        HealthCheck healthCheck = new HealthCheck(new Guid().ToString()) { Status = "NOT_OK", ServiceName = "RSMVC" };
                        healthCheckRepo.AddHealthCheck(healthCheck);
                    }

                    if (notificationProxy.CheckStatus())
                    {
                        Trace.TraceWarning("Email notification service is WORKING");
                        string guid = Guid.NewGuid().ToString();
                        HealthCheck healthCheck = new HealthCheck(guid) { Status = "OK", ServiceName = "NotificationService" };
                        healthCheckRepo.AddHealthCheck(healthCheck); //OTKOMENTARISI
                    }
                    else
                    {
                        Trace.TraceWarning("Email notification service is NOT WORKING");
                        string guid = Guid.NewGuid().ToString();
                        HealthCheck healthCheck = new HealthCheck(guid) { Status = "NOT_OK", ServiceName = "NotificationService" };
                        //healthCheckRepo.AddHealthCheck(healthCheck); //OTKOMENTARISI
                    }
                }
                catch(Exception e)
                {
                    Trace.TraceWarning(e.InnerException.Message);
                    Trace.TraceWarning("Service not alive anymore!");
                }


                Trace.TraceInformation("Working");
                await Task.Delay(3000);
            }
        }
    }
}
