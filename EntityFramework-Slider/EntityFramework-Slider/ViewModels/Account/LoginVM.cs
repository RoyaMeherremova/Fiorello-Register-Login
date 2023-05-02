using System.ComponentModel.DataAnnotations;

namespace EntityFramework_Slider.ViewModels.Account
{
    public class LoginVM
    {
        [Required]
        public string EmaiOrUsername { get; set; }
        [Required]
        [DataType(DataType.Password)] 
        public string Password { get; set; }
    }
}
