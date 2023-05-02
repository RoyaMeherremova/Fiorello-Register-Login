
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework_Slider.Models
{
    public class Slider:BaseEntity
    {
        public string Image { get; set; }
        [NotMapped] //NotMapped-yani Databzaya dusmesin bu properti
        [Required(ErrorMessage = "Don't be empty")]
        public IFormFile Photo { get; set; }   //IFormFile-fayllarnan islemek ucun meselen yukleyende butipden gebul etmek ucun

    }
}
