using System.ComponentModel.DataAnnotations;

namespace ImagesService.Models.DTO
{
    public class UserRegisterDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
