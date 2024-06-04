using Common;
using System;
using System.Collections.Generic;
using System.Linq;
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
