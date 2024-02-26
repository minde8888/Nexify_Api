namespace Nexify.Domain.Entities.Attributes
{
    public class Attributes
    {
        public Guid Id { get; set; }
        public string AttributeName { get; set; }
        public List<string> ImagesNames { get; set; } = new List<string>();
        public List<string> ImageDescriptions { get; set; } = new List<string>();
    }
}
