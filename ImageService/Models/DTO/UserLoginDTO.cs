using System.ComponentModel.DataAnnotations;

namespace ImagesService.Models.DTO
{
    public class UserLoginDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}
