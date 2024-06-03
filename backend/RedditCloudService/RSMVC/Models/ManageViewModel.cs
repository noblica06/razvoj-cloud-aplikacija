using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RSMVC.Models
{

    public class UpdateProfileViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}