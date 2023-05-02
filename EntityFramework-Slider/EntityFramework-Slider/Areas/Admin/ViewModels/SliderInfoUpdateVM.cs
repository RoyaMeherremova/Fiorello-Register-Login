using System.ComponentModel.DataAnnotations;

namespace EntityFramework_Slider.Areas.Admin.ViewModels
{
    public class SliderInfoUpdateVM
    {
        public string SignatureImage { get; set; }


        [Required(ErrorMessage = "Don't be empty")]
        public string Title { get; set; }
        

        [Required(ErrorMessage = "Don't be empty")]
        public string Description { get; set; }

        public IFormFile Photo { get; set; }
    }
}
