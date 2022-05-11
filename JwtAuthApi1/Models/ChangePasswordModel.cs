using System.ComponentModel.DataAnnotations;

namespace JwtAuthApi.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "UserName is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "CurrentPassword is required")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "NewPassword is required")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "ComfirmNewPassword is required")]
        public string ConfirmNewPassword { get; set; }

    }
}
