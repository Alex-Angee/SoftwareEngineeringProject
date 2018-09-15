using System;
using System.ComponentModel.DataAnnotations;

namespace FIU_Chat.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Email")]
        public string inputEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string  inputPassword { get; set; }

        
        [Display(Name = "Remember")]
        public bool rememberMe { get; set; }

    }
}