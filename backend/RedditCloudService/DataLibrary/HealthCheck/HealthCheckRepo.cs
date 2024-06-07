using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.HealthCheck
{
    public class HealthCheckRepo
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        public HealthCheckRepo()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("HealthCheckTable");
            _table.CreateIfNotExists();
        }
        public IQueryable<HealthCheck> RetrieveForLastHour()
        {
            var results = from g in _table.CreateQuery<HealthCheck>()
                          where g.PartitionKey == "HealthCheck"
                          select g;

            DateTime startDate = DateTime.Now.AddHours(-1);
            var ret = results.Where(hc => hc.Timestamp > startDate);

            return ret;
        }

        public IQueryable<HealthCheck> RetrieveForPasth24Hours()
        {
            var results = from g in _table.CreateQuery<HealthCheck>()
                          where g.PartitionKey == "HealthCheck"
                          select g;

            DateTime startDate = DateTime.Now.AddHours(-24);
            var ret = results.Where(hc => hc.Timestamp > startDate);

            return ret;
        }
        public void AddHealthCheck(HealthCheck newHealthCheck)
        { 
            TableOperation insertOperation = TableOperation.Insert(newHealthCheck);
            _table.Execute(insertOperation);
        }
    }
}
