using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using RSMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSMVC.DataRepo
{
    public class ThemeDataRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        public ThemeDataRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("ThemeTable");
            _table.CreateIfNotExists();
        }
        public IQueryable<Theme> RetrieveAllThemes()
        {
            var results = from g in _table.CreateQuery<Theme>()
                          where g.PartitionKey == "Theme"
                          select g;
            return results;
        }
        public void AddTheme(Theme newTheme)
        { // Samostalni rad: izmestiti tableName u konfiguraciju servisa. 
            TableOperation insertOperation = TableOperation.Insert(newTheme);
            _table.Execute(insertOperation);
        }

        public bool Exists(string title)
        {
            return RetrieveAllThemes().Where(s => s.RowKey == title).FirstOrDefault() != null;
        }

        public void RemoveTheme(string title)
        {
            Theme theme = RetrieveAllThemes().Where(s => s.RowKey == title).FirstOrDefault();

            if (theme != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(theme);
                _table.Execute(deleteOperation);
            }
        }

        public Theme GetTheme(string title)
        {
            try
            {
                return RetrieveAllThemes().Where(p => p.RowKey == title).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public void UpdateTheme(Theme theme)
        {
            TableOperation updateOperation = TableOperation.Replace(theme);
            _table.Execute(updateOperation);
        }
    }
}