using ImagesService.Data;
using ImagesService.Models;
using ImagesService.Models.DTO;
using ImagesService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ImagesService.Services
{
    public class UserLoginService : IUserLoginService
    {
        private readonly ImageServiceContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;


        public UserLoginService(IConfiguration config, IEmailService emailService)
        {
            _context = new ImageServiceContext();
            _config = config;
            _emailService = emailService;
        }



        public bool CheckPassedData(UserLoginDTO user)
        {
            Regex regEmail;
            regEmail = new Regex(@"^[a-z][a-z0-9_]*@[a-z0-9]*\.[a-z]{2,3}$");
            if (!regEmail.IsMatch(user.Email) || user.Email.Length > 30)
            {
                return false;
            }

	        Regex regPassword;
	        regPassword = new Regex(@"^[a-zA-Z0-9!#$&()*+,\-./:><?@[\]^_{}|~]*$");
            if(!regPassword.IsMatch(user.Password) || user.Password.Length < 6  || user.Password.Length > 30)
            {
                return false;
            }

            return true;
        }

        public User? VerifyPassword(string email, string password)
        {
            User? user = _context.Users.FirstOrDefault(u=> u.Email == email);
            if (user == null)
            {
                throw new BadUserDataExcpetion();
            }

            if(user.VerifiedAt == null)
            {
                throw new BadUserDataExcpetion();
            }


            if (user.IsLocked == true)
            {
                if (user.LockExpires > DateTime.Now)
                {
                    throw new LockedAccountException();
                }else
                {
                    user.IsLocked = false;
                    _context.SaveChangesAsync();
                }
            }

            string savedPasswordHash = user.PasswordHash;

            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            // get salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            string? pepper = _config.GetSection("Pepper").Value;
            int iteriations = int.Parse(_config.GetSection("Iterations").Value ?? "10000");
            var pbkdf2 = new Rfc2898DeriveBytes(password + pepper, salt, iteriations, HashAlgorithmName.SHA512);
            byte[] hash = pbkdf2.GetBytes(20);

            // compare
            for (int i = 0; i < 20; i++)
            {
                // od 16 ponieważ pierwsze 16 to sól
                if (hashBytes[i + 16] != hash[i])
                {
                    user.LoginTries++;
                    user = LockUser(user);
                    _context.SaveChangesAsync();
                    throw new BadUserDataExcpetion();
                }
            }

            user.LoginTries = 0;
            _context.SaveChangesAsync();

            return user;
        }


        private User LockUser(User user)
        {
            if(user.LoginTries < 5)
            {
                return user;
            }

            if (user.LoginTries == 5)
            {
                user.IsLocked = true;
                user.LockExpires = DateTime.Now.AddMinutes(5);
            }else if (user.LoginTries == 10)
            {
                user.IsLocked = true;
                user.LockExpires = DateTime.Now.AddMinutes(10);
            }else if (user.LoginTries == 15) {
                user.IsLocked = true;
                user.LockExpires = DateTime.Now.AddHours(1);
            }
            else if(user.LoginTries == 20 )
            {
                user.IsLocked = true;
                user.LockExpires = DateTime.Now.AddHours(5);
            }
            else if (user.LoginTries >= 25 && user.LoginTries%5 == 0)
            {
                user.IsLocked = true;
                user.LockExpires = DateTime.Now.AddDays(1);
            }
            return user;

        }


        public void AddDevice(string deviceToken, string email, string userAgent)
        {
            User? user = _context.Users.FirstOrDefault(u => u.Email == email);
            Device device = new Device
            {
                Token = deviceToken,
            };

            try
            {
                _emailService.SendEmail(new EmailDTO
                {
                    Body =
                        "New device has logged into your account! <br>" +
                        "User Agent: <br>" + userAgent,
                    To = user.Email,
                    Subject = "New device logged in",
                });

            }
            catch(Exception ex)
            {

            }

            user.Devices.Add(device);
            _context.SaveChanges();
        }

    }


    public class LockedAccountException : Exception
    {

        public LockedAccountException() { }
        public LockedAccountException(string message) : base(message)
        {
        }
    }

    public class BadUserDataExcpetion : Exception
    {
        public BadUserDataExcpetion() { }
        public BadUserDataExcpetion(string message) : base(message)
        {

        }
        
    }
}
