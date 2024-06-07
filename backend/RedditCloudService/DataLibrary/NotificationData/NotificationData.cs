using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.NotificationData
{
    public class NotificationData : TableEntity
    {
        public string Guid { get; set; }
        public string CommentId { get; set; }
        public int EmailCounter { get; set; }

        public NotificationData(string guid) { PartitionKey = "NotificationData"; RowKey = guid; }
        public NotificationData() { }
    }
}
