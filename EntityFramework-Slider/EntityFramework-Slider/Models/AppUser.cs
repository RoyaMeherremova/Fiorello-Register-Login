using Microsoft.AspNetCore.Identity;

namespace EntityFramework_Slider.Models
{
    public class AppUser:IdentityUser //biz login-user  yazanda usere elave properti lazim olduqda geydiyat ucun biz yeni model yaradib instans aliriq idenityty clasdan(onun propertileri var deye bize lazim olan)
    {
        public string FullName { get; set; }
    }
}
