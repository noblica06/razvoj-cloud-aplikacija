using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RSMVC.Models
{
    public class CreateThemeViewModel
    {
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public HttpPostedFileBase File { get; set; }
    }

}