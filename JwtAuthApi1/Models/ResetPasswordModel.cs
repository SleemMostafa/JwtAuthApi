using System.ComponentModel.DataAnnotations;

namespace JwtAuthApi.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "UserName is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "NewPassword is required")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "ComfirmNewPassword is required")]
        public string ConfirmNewPassword { get; set; }
        public string Token { get; set; }
    }
}
