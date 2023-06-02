using ImagesService.Models.DTO;

namespace ImagesService.Services.Interfaces
{
    public interface IUserRegisterService
    {
        public void AddUser(UserRegisterDTO user);
        bool ChangePassword(string jwt, string password, string confirmPassword);
        public bool CheckIfUserExist(string email);
        public bool CheckPassword(string password);
        bool CreateResetToken(string email);
        bool ResetPassword(ResetPasswordReq resetPasswordReq);
        public bool ValidateUserData(UserRegisterDTO user);
        bool VerifyUser(string token);
        public bool CheckIfPasswordIsSecureAgainstDictionaryAttact(string password);
    }
}
