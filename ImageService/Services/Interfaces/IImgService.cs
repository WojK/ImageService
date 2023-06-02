namespace ImagesService.Services.Interfaces
{
    public interface IImgService
    {

        public Task<bool> IsImageSave(IFormFile file);
    }
}
