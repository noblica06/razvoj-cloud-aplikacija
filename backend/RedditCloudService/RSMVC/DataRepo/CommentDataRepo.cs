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
    public class CommentDataRepo
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        public CommentDataRepo()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("CommentTable");
            _table.CreateIfNotExists();
        }
        public IQueryable<Comment> RetrieveAllComments()
        {
            var results = from g in _table.CreateQuery<Comment>()
                          where g.PartitionKey == "Comment"
                          select g;
            return results;
        }
        public void AddComment(Comment newComment)
        { // Samostalni rad: izmestiti tableName u konfiguraciju servisa. 
            TableOperation insertOperation = TableOperation.Insert(newComment);
            _table.Execute(insertOperation);
        }

        public bool Exists(string guid)
        {
            return RetrieveAllComments().Where(s => s.RowKey == guid).FirstOrDefault() != null;
        }

        public void RemoveComment(string guid)
        {
            Comment comment = RetrieveAllComments().Where(s => s.RowKey == guid).FirstOrDefault();

            if (comment != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(comment);
                _table.Execute(deleteOperation);
            }
        }

        public Comment GetComment(string guid)
        {
            try
            {
                return RetrieveAllComments().Where(p => p.RowKey == guid).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public void UpdateComment(Comment comment)
        {
            TableOperation updateOperation = TableOperation.Replace(comment);
            _table.Execute(updateOperation);
        }
    }
}