using System.ComponentModel.DataAnnotations;

namespace EntityFramework_Slider.ViewModels.Account
{
    public class RegisterVM
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.EmailAddress,ErrorMessage = "E-mail is not valid")]  //email formatinda olsun email
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]  //pasword formatinda olsun ulduz kimi
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password),Compare(nameof(Password))]   //Compare-muqayise ele Pasword propertisi ile eyni dimi
        public string ConfirmPassword { get; set; }
    }
}
