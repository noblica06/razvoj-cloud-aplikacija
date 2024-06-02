using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSMVC.Models
{
    public class ThemeAndComments
    {
        public Theme Theme { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<string> Upvoters { get; set; } = new List<string>();
        public List<string> Downvoters { get; set; } = new List<string>();
        public List<string> Subscribers { get; set; } = new List<string>();
    }
}