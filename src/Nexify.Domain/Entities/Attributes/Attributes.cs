namespace Nexify.Domain.Entities.Attributes
{
    public class Attributes
    {
        public Guid Id { get; set; }
        public string AttributeName { get; set; }
        public List<string> ImageDescription { get; set; }
        public List<string> ImagesNames { get; set; }
    }
}
