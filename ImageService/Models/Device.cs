namespace ImagesService.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public virtual User User { get; set; } 
    }
}
