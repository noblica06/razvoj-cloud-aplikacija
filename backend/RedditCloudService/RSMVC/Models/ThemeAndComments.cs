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
    }
}