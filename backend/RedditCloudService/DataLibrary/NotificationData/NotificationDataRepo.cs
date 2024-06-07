using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.NotificationData
{
    public class NotificationDataRepo
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        public NotificationDataRepo()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("NotificationDataTable");
            _table.CreateIfNotExists();
        }
        public IQueryable<NotificationData> RetrieveAllNotificationData()
        {
            var results = from g in _table.CreateQuery<NotificationData>()
                          where g.PartitionKey == "NotificationData"
                          select g;
            return results;
        }
        public void AddNotificationData(NotificationData notificationData)
        { // Samostalni rad: izmestiti tableName u konfiguraciju servisa. 
            TableOperation insertOperation = TableOperation.Insert(notificationData);
            _table.Execute(insertOperation);
        }

        public NotificationData GetNotificationData(string guid)
        {
            try
            {
                return RetrieveAllNotificationData().Where(p => p.RowKey == guid).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }

        }
    }
}
