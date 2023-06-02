namespace ImagesService.Models.DTO
{
    public class EmailDTO
    {
        public string? Body { get; set; }
        public string? Subject { get; internal set; }
        public string? To { get; set; }
    }
}
