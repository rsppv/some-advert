using System.ComponentModel.DataAnnotations;

namespace SomeAdvert.Web.Models
{
    public class ConfirmAccountViewModel
    {
        [Required]
        [Display(Name = "Verification code")]
        public string Code { get; set; }
    }
}