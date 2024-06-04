using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataLibrary.UserData
{
    public class UserDataRepo
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        public UserDataRepo()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("UserDataTable");
            _table.CreateIfNotExists();
        }
        public IQueryable<UserData> RetrieveAllUserDatas()
        {
            var results = from g in _table.CreateQuery<UserData>()
                          where g.PartitionKey == "UserData"
                          select g;
            return results;
        }
        public void AddUserData(UserData newUserData)
        { // Samostalni rad: izmestiti tableName u konfiguraciju servisa. 
            TableOperation insertOperation = TableOperation.Insert(newUserData);
            _table.Execute(insertOperation);
        }

        public bool Exists(string email)
        {
            return RetrieveAllUserDatas().Where(s => s.RowKey == email).FirstOrDefault() != null;
        }

        public void RemoveUserData(string email)
        {
            UserData userData = RetrieveAllUserDatas().Where(s => s.RowKey == email).FirstOrDefault();

            if (userData != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(userData);
                _table.Execute(deleteOperation);
            }
        }

        public UserData GetUserData(string email)
        {
            try
            {
                return RetrieveAllUserDatas().Where(p => p.RowKey == email).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public void UpdateUserData(UserData userData)
        {
            TableOperation updateOperation = TableOperation.Replace(userData);
            _table.Execute(updateOperation);
        }
    }
}