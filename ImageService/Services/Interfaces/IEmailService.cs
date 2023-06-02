using ImagesService.Models.DTO;

namespace ImagesService.Services.Interfaces
{
        public interface IEmailService
        {
            void SendEmail(EmailDTO emailDTO);
        }
    
}
