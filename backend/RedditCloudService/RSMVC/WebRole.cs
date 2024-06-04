using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Common;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using RSMVC.HealthStatus;

namespace RSMVC
{
    public class WebRole : RoleEntryPoint
    {
        private ServiceHost serviceHost;
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.
            /*string baseAddress = $"net.tcp://{RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["HealthMonitoring"].IPEndpoint}/";
            serviceHost = new ServiceHost(typeof(HealthStatusCheck), new Uri(baseAddress));

            serviceHost.AddServiceEndpoint(typeof(IHealthMonitoring), new NetTcpBinding(), baseAddress);

            serviceHost.Open(); */

            serviceHost = new ServiceHost(typeof(HealthStatusCheck));
            NetTcpBinding binding = new NetTcpBinding();
            serviceHost.AddServiceEndpoint(typeof(IHealthMonitoring), binding, new
            Uri("net.tcp://localhost:6000/HealthMonitoring"));
            serviceHost.Open();
            Console.WriteLine("Server ready and waiting for requests.");
            Console.WriteLine("Server ready and waiting for requests.");

            return base.OnStart();
        }
    }
}
