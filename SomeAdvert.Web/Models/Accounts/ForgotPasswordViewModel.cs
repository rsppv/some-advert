using System.ComponentModel.DataAnnotations;

namespace SomeAdvert.Web.Models.Accounts
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}