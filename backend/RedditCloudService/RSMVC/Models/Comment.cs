using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSMVC.Models
{
    public class Comment : TableEntity
    {
        public string Guid { get; set; }
        public string ThemeTitle { get; set; }
        public string UserEmail { get; set; }
        public string Content { get; set; }
        public string UserImage { get; set; }

        public Comment(string guid) { PartitionKey = "Comment"; RowKey = guid; }
        public Comment()
        {

        }
    }
}