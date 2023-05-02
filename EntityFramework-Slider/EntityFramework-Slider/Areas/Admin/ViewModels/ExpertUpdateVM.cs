using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework_Slider.Areas.Admin.ViewModels
{
    public class ExpertUpdateVM
    {
        public string Image { get; set; }


        [Required(ErrorMessage = "Don't be empty")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Don't be empty")]
        public string Position { get; set; }

        public IFormFile Photo { get; set; }
    }
}
