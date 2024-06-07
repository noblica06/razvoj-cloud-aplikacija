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

        ServiceHost serviceHost;

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

            /*serviceHost = new ServiceHost(typeof(AdminEmailService));
            NetTcpBinding binding = new NetTcpBinding();
            serviceHost.AddServiceEndpoint(typeof(ISendAdminEmails), binding, new
            Uri("net.tcp://localhost:6003/AdminEmailService"));
            serviceHost.Open();
            Console.WriteLine("Server ready and waiting for requests."); */

            RoleInstanceEndpoint inputEndPoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["HMEndpoint"];
            string endpoint = string.Format("net.tcp://{0}/{1}", inputEndPoint.IPEndpoint, "AdminEmailService");
            serviceHost = new ServiceHost(typeof(AdminEmailService));
            NetTcpBinding binding = new NetTcpBinding();
            serviceHost.AddServiceEndpoint(typeof(ISendAdminEmails), binding, endpoint);
            serviceHost.Open();

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

        /* public void ConnectToRedditService()
         {
             var binding = new NetTcpBinding();
             ChannelFactory<IHealthMonitoring> factory = new
             ChannelFactory<IHealthMonitoring>(binding, new
             EndpointAddress("net.tcp://localhost:6000/HealthMonitoring"));
             redditProxy = factory.CreateChannel();
         } */

        public void ConnectToRedditService()
        {
            var endpoint = RoleEnvironment.Roles["RSMVC"].Instances[0].InstanceEndpoints["InternalEndpoint"];
            var address = new EndpointAddress($"net.tcp://{endpoint.IPEndpoint}/HealthMonitoring");
            var binding = new NetTcpBinding();

            ChannelFactory<IHealthMonitoring> factory = new ChannelFactory<IHealthMonitoring>(binding, address);
            redditProxy = factory.CreateChannel();

        }

        /*public void ConnectToNotificationService()
        {
            var binding = new NetTcpBinding();
            ChannelFactory<IHealthMonitoring> factory = new
            ChannelFactory<IHealthMonitoring>(binding, new
            EndpointAddress("net.tcp://localhost:6001/HealthMonitoring"));
            notificationProxy = factory.CreateChannel();
        } */

        public void ConnectToNotificationService()
        {
            var endpoint = RoleEnvironment.Roles["NotificationService"].Instances[0].InstanceEndpoints["InternalEndpoint"];
            var address = new EndpointAddress($"net.tcp://{endpoint.IPEndpoint}/HealthMonitoring");
            var binding = new NetTcpBinding();

            ChannelFactory<IHealthMonitoring> factory = new ChannelFactory<IHealthMonitoring>(binding, address);
            notificationProxy = factory.CreateChannel();
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
                        healthCheckRepo.AddHealthCheck(healthCheck);
                    }
                    else
                    {
                        //upisi u tabelu NotOK
                        Trace.TraceWarning("RSMVC is NOT WORKING");
                        string guid = Guid.NewGuid().ToString();
                        HealthCheck healthCheck = new HealthCheck(guid) { Status = "NOT_OK", ServiceName = "RSMVC" };
                        healthCheckRepo.AddHealthCheck(healthCheck);
                        //posalji mejl notification servicu
                        AdminEmailService.AddEmailsToQueue();

                    }

                    if (notificationProxy.CheckStatus())
                    {
                        Trace.TraceWarning("Email notification service is WORKING");
                        string guid = Guid.NewGuid().ToString();
                        HealthCheck healthCheck = new HealthCheck(guid) { Status = "OK", ServiceName = "NotificationService" };
                        healthCheckRepo.AddHealthCheck(healthCheck);
                    }
                    else
                    {
                        Trace.TraceWarning("Email notification service is NOT WORKING");
                        string guid = Guid.NewGuid().ToString();
                        HealthCheck healthCheck = new HealthCheck(guid) { Status = "NOT_OK", ServiceName = "NotificationService" };
                        healthCheckRepo.AddHealthCheck(healthCheck);
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
