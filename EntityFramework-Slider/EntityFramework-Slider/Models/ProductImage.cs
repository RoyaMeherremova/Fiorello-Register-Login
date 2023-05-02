namespace EntityFramework_Slider.Models
{
    public class ProductImage:BaseEntity
    {

        //ProductImage-table lazimdir bize her bir productun bir neceden sekli ola biler deye
        public string? Image { get; set; }
        public bool IsMain { get; set; } = false;

        //ProductId-properti yaziriq =many terefde one.
        public int ProductId { get; set; }

        //Product Product -yaziriq imageden yola dusende producta herseye cata bilek deye
        public Product Product { get; set; }
    }
}
