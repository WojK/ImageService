using System.Net;
using System.Text.Json;
using ImagesService.Models;
using ImagesService.Models.DTO;
using ImagesService.Services;
using ImagesService.Services.Interfaces;
using ImagesService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImagesService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IUserRegisterService _registerService;

        public RegisterController(IUserRegisterService registerService)
        {
            _registerService = registerService;
        }

        
        [HttpPost("/register")]
        public IActionResult RegisterUserAsync([FromBody] UserRegisterDTO user)
        {

            bool IsUserExist = _registerService.CheckIfUserExist(user.Email);
            if(IsUserExist)
            {
                return BadRequest("User already exist");
            }


            bool PassedDataCorrect = _registerService.ValidateUserData(user);
            if (!PassedDataCorrect)
            {
                return BadRequest(
                    JsonSerializer.Serialize(new MessageDTO
                    {
                        Message = "Incorrect user data \n" +
                    "Name and Surname can only contain alphabetic characters \n" +
                    "Email must be valid"
                    })) ;
            }


            bool IsPasswordCorrect = _registerService.CheckPassword(user.Password);
            if (!IsPasswordCorrect)
            {
                return BadRequest(
                    JsonSerializer.Serialize(new MessageDTO
                    {
                        Message =
                    "Incorrect Password\n" +
                    "Entropy should be at least 50 \n" +
                    "Password should have min 6 characters \n" +
                    "You can use only specific special characters: \n" +
                    @"!#$&()*+,\-./:><?@[]^_{}|~"
                    }));
            }


            _registerService.AddUser(user);

            return Ok("Added new user");
        }

        [HttpPost("/verify-account")]
        public IActionResult VerifyUser(string token)
        {
            bool success = _registerService.VerifyUser(token);
            if(success)
            {
                return Ok("User Verified");

            }
            return BadRequest();
        }

        [HttpPost("/forgot-password")]
        public IActionResult ForgotPassword(string email)
        {
            bool success = _registerService.CreateResetToken(email);
            if (success)
            {
                return Ok("Created reset token");

            }
            return BadRequest("User not found");
        }

        [HttpPost("/reset-password")]
        public IActionResult ResetPassword(ResetPasswordReq resetPasswordReq)
        {
            bool success = _registerService.ResetPassword(resetPasswordReq);
            if (success)
            {
                return Ok("Password has been reseted");

            }
            return BadRequest("Token has expired or reset token does not exist");
        }

        [HttpPost("/count-entropy")]
        public IActionResult CountEntropy([FromBody] CountPasswordEntropyReq req)
        {
            double entropy = Math.Floor(PasswordUtils.CountEntropy(req.Password));
            return Ok(JsonSerializer.Serialize(new { Entropy = entropy }));
        }

        [Authorize]
        [HttpPost("/change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordReq changePasswordReq)
        {
            string? jwt = Request.Headers["Authorization"]; 
            if (jwt == null)
            {
                return BadRequest("No JWT");

            }

            if (!changePasswordReq.Password.Equals(changePasswordReq.PasswordConfirm))
            {
                return BadRequest(
                     JsonSerializer.Serialize(new MessageDTO
                     {
                         Message = "Passwords are not the same"
                     })
                    );
            }

            bool isPasswordSecureAgainstDictionaryAttack = _registerService.CheckIfPasswordIsSecureAgainstDictionaryAttact(changePasswordReq.Password);
            if (!isPasswordSecureAgainstDictionaryAttack)
            {
                return BadRequest(
                     JsonSerializer.Serialize(new MessageDTO
                     {
                         Message = "Password is not secure against dictionary attack"
                     })
                    );
            }

            jwt = jwt.Replace("Bearer ", string.Empty);
            bool result = _registerService.ChangePassword(jwt, changePasswordReq.Password, changePasswordReq.PasswordConfirm);
            if(result)
            {
                return Ok();

            }
            return BadRequest();
        }


    }
}
