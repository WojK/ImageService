namespace ImagesService.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public bool Public { get; set; }
        public bool Private { get; set; }
        public bool SelectedUsers { get; set; }
        public virtual User User { get; set; }
    }
}
