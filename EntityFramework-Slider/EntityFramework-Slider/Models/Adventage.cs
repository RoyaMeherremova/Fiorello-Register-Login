namespace EntityFramework_Slider.Models
{
    public class Adventage:BaseEntity

    {
        public string? Description { get; set; }
        public string? Image { get; set; }

        public int AboutId { get; set; }

        public About About { get; set; }
    }
}
