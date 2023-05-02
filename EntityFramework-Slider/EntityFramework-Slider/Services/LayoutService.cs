using EntityFramework_Slider.Data;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Newtonsoft.Json;

namespace EntityFramework_Slider.Services
{
    public class LayoutService : ILayoutService  //Layouta Databzadan data gondere bilek deye istifade edirik
    {
        private readonly AppDbContext _context;


        private readonly IBasketService _basketService;
        public LayoutService(AppDbContext context,
                             IBasketService basketService)
        {
            _context = context;
            _basketService = basketService;
        }

        public LayoutVM GetSettingDatas()   //VM istiafde edirik Layouta ferqli data gondermek ucun
        {
            Dictionary<string,string> settings = _context.Settings.AsEnumerable().ToDictionary(m => m.Key, m => m.Value);  //AsEnumerable().ToDictionary-yazib Dictionariye ceviririk Settigden gelenleri(Databazadan data gonderirik Layouta)

            List<BasketVM> basketDatas = _basketService.GetBasketDatas();  //basketden data gonderirik Layouta
                             
            return new LayoutVM { Settings = settings, BasketCount = basketDatas.Sum(m => m.Count) }; //basketdeki butun productlarin cem countu gonderik Layouta
        }
     


    }
}
