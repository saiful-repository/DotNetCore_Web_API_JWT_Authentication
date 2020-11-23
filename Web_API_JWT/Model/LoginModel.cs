using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web_API_JWT.Model
{
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name Is Required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password Is Required")]
        public string Password { get; set; }
    }
}
