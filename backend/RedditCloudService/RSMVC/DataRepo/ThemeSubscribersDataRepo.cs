using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSMVC.DataRepo
{
    public class ThemeSubscriber : TableEntity
    {
        public string ThemeTitle { get; set; }
        public string UserEmail { get; set; }

        public ThemeSubscriber(string titleEmail) { PartitionKey = "ThemeSubscriber"; RowKey = titleEmail; }
        public ThemeSubscriber() { }
    }

    public class ThemeSubscribersDataRepo
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        public ThemeSubscribersDataRepo()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("ThemeSubscribersTable");
            _table.CreateIfNotExists();
        }
        public IQueryable<ThemeSubscriber> RetrieveAllThemeSubscribers()
        {
            var results = from g in _table.CreateQuery<ThemeSubscriber>()
                          where g.PartitionKey == "ThemeSubscriber"
                          select g;
            return results;
        }
        public void AddThemeSubscriber(ThemeSubscriber themeSubscriber)
        {
            TableOperation insertOperation = TableOperation.Insert(themeSubscriber);
            _table.Execute(insertOperation);
        }

        public bool Exists(string titleEmail)
        {
            return RetrieveAllThemeSubscribers().Where(s => s.RowKey == titleEmail).FirstOrDefault() != null;
        }

        public void RemoveThemeSubscriber(string titleEmail)
        {
            ThemeSubscriber themeSubscriber = RetrieveAllThemeSubscribers().Where(s => s.RowKey == titleEmail).FirstOrDefault();

            if (themeSubscriber != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(themeSubscriber);
                _table.Execute(deleteOperation);
            }
        }

        public ThemeSubscriber GetThemeSubscriber(string titleEmail)
        {
            try
            {
                return RetrieveAllThemeSubscribers().Where(p => p.RowKey == titleEmail).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public void UpdateThemeSubscriber(ThemeSubscriber themeSubscriber)
        {
            TableOperation updateOperation = TableOperation.Replace(themeSubscriber);
            _table.Execute(updateOperation);
        }
    }
}

