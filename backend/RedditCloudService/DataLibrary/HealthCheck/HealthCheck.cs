using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.HealthCheck
{
    public class HealthCheck : TableEntity
    {
        public string Status { get; set; }
        public string Guid { get; set; }

        public HealthCheck(string guid) { PartitionKey = "HealthCheck"; RowKey = guid; }
        public HealthCheck()
        {

        }
    }
}

