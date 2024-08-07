using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StoreService.Models.Web
{
  [JsonSerializable(typeof(LoginRequest))]
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
