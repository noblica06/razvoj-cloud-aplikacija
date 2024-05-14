using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditService.Models
{
    public class User : TableEntity
    {
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public string ThumbnailUrl { get; set; }

        public User(string email) { PartitionKey = "User"; RowKey = email; }
        public User()
        {

        }

        public override string ToString()
        {
            return $"{FirstName} {LastName} {Email} {Password}";
        }
    }
}
