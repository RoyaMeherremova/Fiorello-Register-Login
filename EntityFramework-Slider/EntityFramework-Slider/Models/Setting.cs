namespace EntityFramework_Slider.Models
{
    public class Setting:BaseEntity  //static datalari saxlamaq ucun meselen Layoutda olan datalar
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
