using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ImagesService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public int LoginTries { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockExpires { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public virtual List<Image> Images { get; set; }

        public virtual List<Device> Devices { get;set; }
    }
}
