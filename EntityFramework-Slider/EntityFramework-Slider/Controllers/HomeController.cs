using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;

namespace EntityFramework_Slider.Controllers
{
    public class HomeController : Controller
    {
        #region Gizli Datalar ucun
        //private readonly ILogger<HomeController> _logger;

        //private readonly IConfiguration _configuration;


        //public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        //{
        //    _logger = logger;
        //    _configuration = configuration;
          
        //}

        //public IActionResult Test()
        //{
        //    var user = _configuration.GetSection("Login:User").Value;

        //    var mail = _configuration.GetSection("Login:Mail").Value;

        //    return Content($"{user} {mail}");
        //}


        #endregion




        private readonly AppDbContext _context;

        private readonly IBasketService _basketService;

        private readonly IProductService _productService;

        private readonly ICategoryService _categoryService;
        public HomeController(AppDbContext context,
                             IBasketService basketService,
                             IProductService productService,
                             ICategoryService categoryService)
        {
            _context = context;
            _basketService = basketService;
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet] 
        public async Task<IActionResult> Index()
        {
            
            //HttpContext.Session.SetString("name", "Pervin");   //Seyfe acilan kimi sessiona sesiona data qoyuram


            //Response.Cookies.Append("surname", "Rehimli", new CookieOptions { MaxAge = TimeSpan.FromMinutes(30) }); //Cocikye data yerlesdirmey key adi ve valusu.TimeSpan-geyd edirik nece degeden sonra silinsin ordan bu cockie.TimeSpan-vaxt ferqi tapmaq ucun istifade olunur  

            //Book book = new Book
            //{
            //    Id = 1,
            //    Name = "Xosrov ve Shirin"
            //};

            //Response.Cookies.Append("book",JsonConvert.SerializeObject(book));    /*JsonConvert.SerializeObject=Objecti Jsona ceviir.Json bize string verir deye Jsona cevirib Coockiye yerlesdiririk.Coockie bizden Int ve string gebul edir*/

            //Linkque querileri =Blogs.Where(m => !m.SoftDelete)-Silinimeyen bloglari ver

            //List miras alir IIEnumerabldan.List-in methodlari var IEnumerable-un yoxdu.Datani viewa IEnumerable kimi gondermek yaxsdidir cunku- 
            //elave methodlar yoxdu daha suretli isleyir

            //IQueryable-query yaradiriq RAMda saxlayiriq hele data getirmirik
            //IQueryable<Slider> slide = _context.Sliders.AsQueryable();
            //sonra serte uyqun datani getiririk.ToList()-yazdiqda request gedir DataBazaya
            //List<Slider> query = slide.Where(m => m.Id >5).ToList();

            //List<int> nums = new List<int>() { 1, 2, 4, 5, 6 };
            //FirstOrDefault()--sertde gondermek olur ,serte uyqun datadan bir necedenedise 1-cini gorsedir,yoxdusa default deyerini gorsedir
            //var res = nums.FirstOrDefault(m => m ==3);

            //First()--varsa data serte uyqunun verir,yoxdusa exception cixarir
            //var res2 = nums.First(m => m == 3);

            //SingleOrDefault()--serte uyqun data  birdene varsa verir,bir necedenedise exception verir,yoxdusa data default deyerin verir
            //Single()--serte uyqun data  birdene varsa verir,bir necedenedise exception verir,yoxdusa exception verir
            //var res = nums.SingleOrDefault(m => m == 3);          
            //ViewBag.num = res;



            List<Slider> sliders = await _context.Sliders.Where(m => !m.SoftDelete).ToListAsync();

            SliderInfo? sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync();

            IEnumerable<Category> categories = await _categoryService.GetAll();


            //Inlude()--Relation  qurduqumuz tablerda istifade edirik.-Tablin icindeki basqa table catmaq istirsense.Many olan terefe.

            IEnumerable<Product> products = await _productService.GetAll();

            About abouts = await _context.Abouts.Include(m => m.Adventages).FirstOrDefaultAsync();

            IEnumerable<Experts> experts = await _context.Experts.Where(m => !m.SoftDelete).ToListAsync();

            ExpertsHeader expertsheaders = await _context.ExpertsHeaders.FirstOrDefaultAsync();

            Subscribe subscribs = await _context.Subscribs.FirstOrDefaultAsync();

            BlogHeader blogheaders = await _context.BlogHeaders.FirstOrDefaultAsync();
            IEnumerable<Say> says = await _context.Says.Where(m => !m.SoftDelete).ToListAsync();

            IEnumerable<Instagram> instagrams = await _context.Instagrams.Where(m => !m.SoftDelete).ToListAsync();

            HomeVM model = new()
            {
                Sliders = sliders,
                SliderInfo = sliderInfo,
                Categories = categories,
                Products = products,
                Abouts = abouts,
                Experts = experts,
                ExpertsHeaders = expertsheaders,
                Subscribs = subscribs,
                BlogHeaders = blogheaders,
                Says = says,
                Instagrams = instagrams
            };

            return View(model);
        }


        //    *COOCKIE AND SESSION STORAGE*  
        //public IActionResult Test()  /* stroragden datani goturub UI-a gorseden method*/
        //{

        //    var sessionData = HttpContext.Session.GetString("name");  //Sessionda olan datani gotururuk keyine gore
        //    var cokieData = Request.Cookies["surname"];  //cockiyede olan datani gotururuk keyine gore
        //    var objectData = JsonConvert.DeserializeObject<Book>(Request.Cookies["book"]);       /*JsonConvert.DeserializeObject < Book >. DeserializeObject-Json -DuplicateWaitObjectException cevirmek -Version tipini geyd edirik Book olsun*/


        //    return Json(objectData);
        //}






        #region ADD BASKET METHOD
        //[HttpPost] /*Post yani data daxil edirik*/
        //[ValidateAntiForgeryToken]  //yoxlayir bize gelen Token bu methoda gire biler ya yox,yani bu saytdan giribse user methodu islet.
        //public async Task<IActionResult> AddBasket(int? id)
        //{

        //    //eyer id silinirse URL-den
        //    if (id == null) return BadRequest();

        //    Product dbProduct = await _context.Products.FindAsync(id);

        //    /*eyer URL-de databazada olmayan Id yazilarsa*/
        //    if (dbProduct == null) return NotFound();

        //    //BasketVM-yaradiriq cunku bize hemin Listden yaliz bir nece data lazimdir
        //    //Ve Coockiye List<BasketVM> gonderik cunku bir nece data gelecek Coockiye

        //    List<BasketVM> basket; /*Bos List teyin edirik*/

        //    if (Request.Cookies["basket"] != null)
        //    {
        //        basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]); //eyer coockide data varsa yani null deyilse coockide olan datani goturub = DeserializeObject<List<BasketVM>>.esayn edirik elmizde olan List<BasketVM>e
        //    }
        //    else
        //    {
        //        basket = new List<BasketVM>();   /*yoxdusa data teze List yaradir*/
        //    }

        //    BasketVM? existProduct = basket.FirstOrDefault(m => m.Id == dbProduct.Id);  //existProduct=Finf(elmizdeki List<basketVM> Id-si = databazda olan data Id)

        //    if (existProduct == null)
        //    {
        //        basket?.Add(new BasketVM
        //        {    //Liste Add edirik yeni BasketVM yani yeni data
        //            Id = dbProduct.Id,
        //            Count = 1
        //        });
        //    }
        //    else
        //    {
        //        existProduct.Count++;
        //    }





        //    Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));  //ve append edirik Coockiye


        //    return RedirectToAction(nameof(Index)); /*basqa actiona yonlendiririk ki add edende sonra getsin Home seyfeye yeniden productlari gorsetsin bize */
        //}

        #endregion








        #region ADD BASKET METHODUN(YUXARDAKI) QISA YAZILISI
        [HttpPost] /*Post yani data daxil edirik*/
        /* [ValidateAntiForgeryToken]*/  //yoxlayir bize gelen Token bu methoda gire biler ya yox,yani bu saytdan giribse user methodu islet.
        public async Task<IActionResult> AddBasket(int? id)  //bize id gelir cartdaki ProductId
        {


            if (id == null) return BadRequest();   //eyer id silinirse URL-den

            Product dbProduct = await _productService.GetById((int)id);   //ve biz  AddBasket methoda gelen Id-li producti databazadan gotururuk.ve-GetProductById((int) id)-idni kast edirik cunku method bizden parametr kimi reqem isteyir ve biz ora sadece id yazsaq string kimi goturur


            if (dbProduct == null) return NotFound();    /*eyer URL-de databazada olmayan Id yazilarsa*/

            //BasketVM-yaradiriq cunku bize hemin Listden yaliz bir nece data lazimdir
            //Ve Coockiye List<BasketVM> gonderik cunku bir nece data gelecek Coockiye

            List<BasketVM> basket = _basketService.GetBasketDatas();   //Coockiden gelen Listi assayn edirik elmizdeki Liste

            BasketVM? existProduct = basket.FirstOrDefault(m => m.Id == dbProduct.Id);  //existProduct=Databazada olan Productun Id-si varmi Coockie Listinde?


            _basketService.AddProductToBasket(existProduct, dbProduct, basket);    //Ve Listi Add edirik Coockiye

            int basketCount = basket.Sum(m => m.Count);  //Sonra basketdeki datalarin countlarinin toplamini tapriq(sebetin supu ucun)
            return Ok(basketCount);
        }
     
        #endregion
    }

    //class Book
    //{
    //    public int Id { get; set;}

    //    public string Name { get; set; }
    //}
}