using ImagesService.Models;
using ImagesService.Models.DTO;

namespace ImagesService.Services.Interfaces
{
    public interface IUserLoginService
    {
        void AddDevice(string deviceToken, string email, string userAgent);
        bool CheckPassedData(UserLoginDTO user);
        public User? VerifyPassword(string email, string password);

    }
}
