using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSMVC.DataRepo
{

    public class ThemeVoters : TableEntity
    {
        public string ThemeTitle { get; set; }
        public string UserEmail { get; set; }
        public bool Upvoted { get; set; }
        public bool Downvoted { get; set; }

        public ThemeVoters(string titleEmail) { PartitionKey = "ThemeVoters"; RowKey = titleEmail; }
        public ThemeVoters() { }
    }

    public class ThemeVotersDataRepo
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        public ThemeVotersDataRepo()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("ThemeVotersTable");
            _table.CreateIfNotExists();
        }
        public IQueryable<ThemeVoters> RetrieveAllThemeVoters()
        {
            var results = from g in _table.CreateQuery<ThemeVoters>()
                            where g.PartitionKey == "ThemeVoters"
                            select g;
            return results;
        }
        public void AddThemeVote(ThemeVoters themeVoters)
        { // Samostalni rad: izmestiti tableName u konfiguraciju servisa. 
            TableOperation insertOperation = TableOperation.Insert(themeVoters);
            _table.Execute(insertOperation);
        }

        public bool Exists(string titleEmail)
        {
            return RetrieveAllThemeVoters().Where(s => s.RowKey == titleEmail).FirstOrDefault() != null;
        }

        public void RemoveThemeVoters(string titleEmail)
        {
            ThemeVoters themeVoters = RetrieveAllThemeVoters().Where(s => s.RowKey == titleEmail).FirstOrDefault();

            if (themeVoters != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(themeVoters);
                _table.Execute(deleteOperation);
            }
        }

        public ThemeVoters GetThemeVoters(string titleEmail)
        {
            try
            {
                return RetrieveAllThemeVoters().Where(p => p.RowKey == titleEmail).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public void UpdateThemeVoters(ThemeVoters themeVoters)
        {
            TableOperation updateOperation = TableOperation.Replace(themeVoters);
            _table.Execute(updateOperation);
        }
    }
}
