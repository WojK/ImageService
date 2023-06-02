using System.ComponentModel.DataAnnotations;

namespace ImagesService.Models.DTO
{
    public class ChangePasswordReq
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string PasswordConfirm { get; set; }
    }
}
