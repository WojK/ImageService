using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using ImagesService.Data;
using ImagesService.Models;
using ImagesService.Models.DTO;
using ImagesService.Services.Interfaces;
using ImagesService.Utils;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Ocsp;

namespace ImagesService.Services
{
    public class UserRegisterService : IUserRegisterService
    {

        private readonly ImageServiceContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;


        public UserRegisterService(IConfiguration config, IEmailService emailService)
        {
            _context = new ImageServiceContext();
            _config = config;
            _emailService = emailService;
        }

        public void AddUser(UserRegisterDTO userDTO)
        {
            User newUser = new User()
            {
                Name = userDTO.Name,
                Surname = userDTO.Surname,
                Email = userDTO.Email,
            };
            string savedPasswordHash = HashPassword(userDTO.Password);
            newUser.PasswordHash = savedPasswordHash;

            newUser.VerificationToken = CreateRandomToken();
            newUser.IsLocked = false;

            try
            {
                _emailService.SendEmail(new EmailDTO
                {
                    Body =
                        "Hello in our Website! <br>"
                        + "To complete registration please click on the link below: <br>"
                        + "\t http://localhost:3000/verification-token/"
                        + newUser.VerificationToken,
                    To = newUser.Email,
                    Subject = "Hello in Our Website",
                });
            }catch (Exception ex) { }
            

            _context.Users.Add(newUser);
            _context.SaveChangesAsync();
        }

        private string HashPassword(string password)
        {
            string? pepper = _config.GetSection("Pepper").Value;
            int Iterations = Int32.Parse(_config.GetSection("Iterations").Value ?? "10000");
            byte[] salt = CreateSalt();

            var pbkdf2 = new Rfc2898DeriveBytes(password + pepper, salt, 10000, HashAlgorithmName.SHA512);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[salt.Length + hash.Length];
            Array.Copy(salt, 0, hashBytes, 0, salt.Length);
            Array.Copy(hash, 0, hashBytes, salt.Length, hash.Length);

            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        public byte[] CreateSalt(int size = 16)
        {
            var gen = System.Security.Cryptography.RandomNumberGenerator.Create();
            byte[] salt = new byte[size];
            gen.GetBytes(salt);

            return salt;
        } 

        public bool CheckIfUserExist(string email)
        {
            if (_context.Users.FirstOrDefault<User>(u => u.Email == email) == null)
            {
                return false;
            }
            return true;
        }

        public bool ValidateUserData(UserRegisterDTO user)
        {
            Regex regEmail;
            regEmail = new Regex(@"^[a-z][a-z0-9_]*@[a-z0-9]*\.[a-z]{2,3}$");
            if (!regEmail.IsMatch(user.Email) || user.Email.Length > 30)
            {
                return false;
            }

            Regex regNameSurname;
            regNameSurname = new Regex(@"^[a-zA-ZżźćńółęąśŻŹĆĄŚĘŁÓŃ ]*$");
            if (!regNameSurname.IsMatch(user.Name) || !regNameSurname.IsMatch(user.Surname))
            {
                return false;
            }

            return true;
        }

        public bool CheckPassword(string password)
        {
            double entropy = Math.Floor(PasswordUtils.CountEntropy(password));
            if(entropy <= 50)
            {
                return false;
            }

            Regex regPassword;
            regPassword = new Regex(@"^[a-zA-Z0-9!#$&()*+,\-./:><?@[\]^_{}|~]*$");
            if (!regPassword.IsMatch(password) || password.Length < 6 || password.Length > 30)
            {
                return false;
            }

            return true;
        }

        public string CreateRandomToken()
        {
            return Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64))
                .Replace("/", "")
                .Replace("=", "")
                .Replace("%", "")
                .Replace("+", "");
        }

        public bool VerifyUser(string token)
        {
            User? user = _context.Users.FirstOrDefault(u => u.VerificationToken == token);
            if (user == null)
            {
                return false;
            }
            user.VerifiedAt = DateTime.Now;
            _context.SaveChanges();
            return true;
        }

        public bool CreateResetToken(string email)
        {
            User? user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return false;
            }
            user.ResetToken = CreateRandomToken();
            user.ResetTokenExpires= DateTime.Now.AddHours(1);
            _context.SaveChanges();

            _emailService.SendEmail(new EmailDTO
            {
                Body =
        "Process of restarting your password has been started! <br>"
        + "To reset password please click on the link below: <br>"
        + "\t http://localhost:3000/change-password/"
        + user.ResetToken,
                To = email,
                Subject = "Reset Password",
            });


            return true;
        }

        public bool ResetPassword(ResetPasswordReq req)
        {
            User? user = _context.Users.FirstOrDefault(u => u.ResetToken == req.Token);
            if(user == null || user.ResetTokenExpires < DateTime.Now)
            {
                return false;
            }
            string savedPasswordHash = HashPassword(req.Password);
            user.PasswordHash = savedPasswordHash;
            user.ResetToken = null;
            user.ResetTokenExpires = null;
            _context.SaveChanges();
            return true;

        }


        public bool CheckIfPasswordIsSecureAgainstDictionaryAttact(string password)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string file = "yahoo.txt";

            using (StreamReader reader = new StreamReader(Path.Combine(path, "Databases", file)))
            {
                string line;
                int i = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Equals(password))
                    {

                        return false;
                    }
                }
            }

            return true;
        }


        public bool ChangePassword(string jwt, string password, string confirmPassword)
        {

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            var email = token.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;
            User? user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return false;
            }

            string passwordHash = HashPassword(password);
            user.PasswordHash = passwordHash;
            _context.SaveChanges();

            return true;
        }
    }
}
