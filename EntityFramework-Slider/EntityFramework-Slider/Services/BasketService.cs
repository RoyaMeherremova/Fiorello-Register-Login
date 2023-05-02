using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Newtonsoft.Json;

namespace EntityFramework_Slider.Services
{
    public class BasketService : IBasketService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;  //-bu Serviclerde Coockideki datalara catmaq ucundu

        public BasketService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public void AddProductToBasket(BasketVM? existProduct, Product dbProduct, List<BasketVM> basket)
        {

            if (existProduct == null)
            {
                basket?.Add(new BasketVM
                {    //Liste Add edirik yeni BasketVM yani yeni data
                    Id = dbProduct.Id,
                    Count = 1
                });
            }
            else
            {
                existProduct.Count++;
            }

            _httpContextAccessor.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));  //ve append edirik Coockiye
        }

        public void DeleteProductFromBasket(int id)
        {
            List<BasketVM> basketProducts = JsonConvert.DeserializeObject<List<BasketVM>>(_httpContextAccessor.HttpContext.Request.Cookies["basket"]);

            BasketVM deletedProduct = basketProducts.FirstOrDefault(m => m.Id == id);

            basketProducts.Remove(deletedProduct);

            _httpContextAccessor.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketProducts));
        }

        public List<BasketVM> GetBasketDatas()
        {

            List<BasketVM> basket; /*Bos List teyin edirik*/

            if (_httpContextAccessor.HttpContext.Request.Cookies["basket"] != null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(_httpContextAccessor.HttpContext.Request.Cookies["basket"]); //eyer coockide data varsa yani null deyilse coockide olan datani goturub = DeserializeObject<List<BasketVM>>.esayn edirik elmizde olan List<BasketVM>e
            }
            else
            {
                basket = new List<BasketVM>();   /*yoxdusa data teze List yaradir*/
            }
            return basket;
        }
    }
}
