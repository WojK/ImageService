using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ImagesService.Data;
using ImagesService.Models;
using ImagesService.Models.DTO;
using ImagesService.Services;
using ImagesService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ImagesService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserLoginService _userLoginService;
        private readonly ImageServiceContext _context = new ImageServiceContext();
        public LoginController(IUserLoginService userLoginService) {
            _userLoginService = userLoginService;
        }

        [HttpPost("/login")]
        public IActionResult Login([FromBody] UserLoginDTO user)
        {

            bool correctUserData = _userLoginService.CheckPassedData(user);

            if (!correctUserData) {
                return BadRequest(JsonSerializer.Serialize(new MessageDTO { Message = "Invalid User Data" }));
            }

            User? userAuth;
            try
            {
                userAuth = _userLoginService.VerifyPassword(user.Email, user.Password);
            
            }catch (LockedAccountException)
            {
                User? u = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                return BadRequest(JsonSerializer.Serialize(new MessageDTO { Message = "Your account is locked to: " +  u.LockExpires.ToString()}));
            }
            catch(BadUserDataExcpetion )
            {
                return BadRequest(JsonSerializer.Serialize(new MessageDTO { Message = "Invalid user data" }));

            }

            if (userAuth != null)
            {
                    var claims = new List<Claim>()
                    {
                    new Claim(ClaimTypes.Email, $"{userAuth.Email}"),
                    };

                    var authSettings = AuthenticationSettings.authSettings;

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.JwtKey));
                    var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var expires = DateTime.Now.AddDays(authSettings.JwtExpiresDays);
                    var token = new JwtSecurityToken(authSettings.Issuer,
                        authSettings.Issuer,
                        claims,
                        expires: expires,
                        signingCredentials: cred
                        );
                    var tokenHandler = new JwtSecurityTokenHandler();
                    string tokenString = tokenHandler.WriteToken(token);

                var cookie = Request.Cookies["Device"];
                if (cookie != null)
                {
                    if (userAuth.Devices != null && !userAuth.Devices.Exists(x => x.Token == cookie))
                    {
                        
                            string? userAgent = Request.Headers["User-Agent"];
                            _userLoginService.AddDevice(cookie, userAuth.Email, userAgent);
                        
                    }

                }
                else
                {
                      AddCoookie(userAuth.Email);
                }


                return Ok(JsonSerializer.Serialize(new { Token = tokenString }));


            }
            return BadRequest(JsonSerializer.Serialize(new MessageDTO { Message = "Invalid User Data"}));
        }

        private void AddCoookie(string email)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddYears(20);
            option.Path = "/";
            option.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
            option.Secure = true;


            string deviceToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(12));
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            deviceToken = rgx.Replace(deviceToken, "x");

            string? userAgent = Request.Headers["User-Agent"];
            _userLoginService.AddDevice(deviceToken, email, userAgent);

            Response.Cookies.Append("Device", deviceToken, option);
        }
    }
}
