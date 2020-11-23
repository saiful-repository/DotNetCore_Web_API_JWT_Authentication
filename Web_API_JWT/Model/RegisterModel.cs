using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web_API_JWT.Model
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name Is Required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "User Name Is Required")]
        [EmailAddress(ErrorMessage = "Enter Valid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        public string Password { get; set; }
    }
}
