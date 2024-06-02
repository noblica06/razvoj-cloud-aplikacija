using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSMVC.Models
{
    public class Theme : TableEntity
    {
        public string UserEmail { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        //public string CommentsJson { get; set; }
        public int UpVotes { get; set; } = 0;
        public List<string> Upvoters { get; set; } = new List<string>();
        public int DownVotes { get; set; } = 0;
        public List<string> Downvoters { get; set; } = new List<string>();
        public List<string> SubscribedUsers { get; set; } = new List<string>();
        //public string SubscribedUsersJson { get; set; }
        public String PhotoUrl { get; set; }
        public String ThumbnailUrl { get; set; }

        public Theme(string guid) { PartitionKey = "Theme"; RowKey = guid; }
        public Theme()
        {

        }
    }
}