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

        private IHealthMonitoring redditProxy;

        public void ConnectToRedditService()
        {
            /* // var endpoint = RoleEnvironment.Roles["RSMVC"].Instances[0].InstanceEndpoints["HealthMonitoring"];
             // var address = new EndpointAddress($"net.tcp://{endpoint.IPEndpoint}/HealthStatusCheck");
             var address = new EndpointAddress("net.tcp://localhost:8081/HealthStatusCheck");
             //var address = new EndpointAddress("net.tcp://127.0.0.1:6000/Service");
             var binding = new NetTcpBinding();

             ChannelFactory<IHealthMonitoring> factory = new ChannelFactory<IHealthMonitoring>(binding, address);
             redditProxy = factory.CreateChannel(); */

            var binding = new NetTcpBinding();
            ChannelFactory<IHealthMonitoring> factory = new
            ChannelFactory<IHealthMonitoring>(binding, new
            EndpointAddress("net.tcp://localhost:6000/HealthMonitoring"));
            redditProxy = factory.CreateChannel();

            Trace.WriteLine("Proxy: " + redditProxy.ToString());
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if(redditProxy == null) ConnectToRedditService();

                    if (redditProxy.CheckStatus())
                    {
                        //upisi u tabelu OK
                        Trace.TraceWarning("Service is working!");
                        string guid = Guid.NewGuid().ToString();
                        HealthCheck healthCheck = new HealthCheck(guid) { Status = "OK" };
                        //healthCheckRepo.AddHealthCheck(healthCheck); //OTKOMENTARISI
                    }
                    else
                    {
                        //upisi u tabelu NotOK
                        Trace.TraceWarning("Service is not working");
                        HealthCheck healthCheck = new HealthCheck(new Guid().ToString()) { Status = "NOT_OK" };
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
