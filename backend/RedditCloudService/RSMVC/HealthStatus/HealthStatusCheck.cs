using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSMVC.HealthStatus
{
    public class HealthStatusCheck : IHealthMonitoring
    {
        public bool CheckStatus()
        {
            return true;
        }
    }
}