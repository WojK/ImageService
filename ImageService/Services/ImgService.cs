using ImagesService.Services.Interfaces;
using nClam;

namespace ImagesService.Services
{
    public class ImgService : IImgService
    {

        private readonly IConfiguration _config;

        public ImgService(IConfiguration config)
        {
            _config = config;
        }


        public async Task<bool> IsImageSave(IFormFile file)
        {
            var ms = new MemoryStream();
            file.OpenReadStream().CopyTo(ms);
            byte[] fileBytes = ms.ToArray();

            try
            {
                var clam = new ClamClient(_config["ClamAVServer:URL"],
                          Convert.ToInt32(_config["ClamAVServer:Port"]));
                var scanResult = await clam.SendAndScanFileAsync(fileBytes);
                switch (scanResult.Result)
                {
                    case ClamScanResults.Clean:
                        return true;
                    case ClamScanResults.VirusDetected:
                        return false;
                    case ClamScanResults.Error:
                        return false;
                    case ClamScanResults.Unknown:
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }
    }
}
