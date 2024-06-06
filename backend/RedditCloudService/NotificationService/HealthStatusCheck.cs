using Common;
using DataLibrary.QueueHelper;
using Microsoft.WindowsAzure.Storage.Queue;
using PostmarkDotNet;
using PostmarkDotNet.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService
{
    public class HealthStatusCheck : IHealthMonitoring
    {
        public bool CheckStatus()
        {
            return true;
        }
    }
}
