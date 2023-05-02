using System.ComponentModel.DataAnnotations;

namespace EntityFramework_Slider.Models
{
    public class Category:BaseEntity
    {
        [Required(ErrorMessage ="Don't be empty")]  //Required-Name null ola bilmez,ErrorMessage-olsa error mesage yazilsin
        
     /*   [StringLength(20,ErrorMessage = "The name length must be max 20 characters")] */ //maximum 10 herfden ibaret ola biler
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }   /*Categoriden  yola cixanda Producta catmaq ucun*/
    }
}
