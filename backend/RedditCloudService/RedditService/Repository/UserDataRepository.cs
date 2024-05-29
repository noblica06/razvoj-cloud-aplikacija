using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using RedditService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditService.Repository
{
    public class UserDataRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        public UserDataRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("UserTable");
            //_table.CreateIfNotExists();
        }
        public IQueryable<User> RetrieveAllUsers()
        {
            var results = from g in _table.CreateQuery<User>()
                          where g.PartitionKey == "User"
                          select g;
            return results;
        }
        public void AddUser(User newUser)
        { // Samostalni rad: izmestiti tableName u konfiguraciju servisa. 
            TableOperation insertOperation = TableOperation.Insert(newUser);
            _table.Execute(insertOperation);
        }

        public bool Exists(string email)
        {
            return RetrieveAllUsers().Where(s => s.RowKey == email).FirstOrDefault() != null;
        }

        public void RemoveUser(string id)
        {
            User user = RetrieveAllUsers().Where(s => s.RowKey == id).FirstOrDefault();

            if (user != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(user);
                _table.Execute(deleteOperation);
            }
        }

        public User GetUser(string email)
        {
            try
            {
                return RetrieveAllUsers().Where(p => p.RowKey == email).FirstOrDefault();
            }
            catch(Exception e)
            {
                return null;
            }
            
        }

        public void UpdateUser(User user)
        {
            TableOperation updateOperation = TableOperation.Replace(user);
            _table.Execute(updateOperation);
        }
    }
}
