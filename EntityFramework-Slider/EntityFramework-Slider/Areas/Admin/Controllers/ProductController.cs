using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Heplers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Text.RegularExpressions;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;
        public ProductController(IProductService productService, 
                                 ICategoryService categoryService,
                                 IWebHostEnvironment env,AppDbContext context)
        {
            _productService = productService;
            _categoryService = categoryService;
            _env = env;
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1,int take = 4) //page-hansi seyfededi hecne gondermesek Default 1-ci seyfeni goturur ama gelirse hemin reqemin seyfesin goturur,take-necedene gotursun data
        {
            List<Product> products = await _productService.GetPaginatedDatas(page,take);  //databzadaki butun produktlar

            List<ProductListVM> mappedDatas = GetMappedDatas(products);  //VM -nen gonderik datanin lazimsiz propertileri getmesin deye           

            int pageCount = await GetPageCountAsync(take); //method seyfedeki pagination sayini tapmaq ucun 

            ViewBag.take = take;

            Paginate<ProductListVM> paginaDatas = new(mappedDatas, page, pageCount);//paginate- gonderirik secilmis propertili datalari=datas,
                                                                                    //page=hansi seyfedeyik,
                                                                                    //pagecountu=paginatlari siralamaq ucun fora salib
          
            return View(paginaDatas);

        }

        private List<ProductListVM> GetMappedDatas(List<Product> products)
        {
            List<ProductListVM> mappedDatas = new(); //bir clasi istifade etmek ucun instans almaq lazimdi
            foreach (var product in products)
            {
                ProductListVM productVM = new()   //her VM bir product(VM propertisini beraber edirik productun bize lazim olan propertilerine)
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Count = product.Count,
                    CategoryName = product.Category.Name,
                    MainImage = product.Images.Where(m => m.IsMain).FirstOrDefault()?.Image

                };
                mappedDatas.Add(productVM); //sonra VM-leri yani her producti yiqiriq Liste
            }
            return mappedDatas;
        }  //method mapped-yani beraberlesdirilmis-VM modele beraberlesdiririk databazadaki productun propertilerini.
                                                                                //bu method bize lazimdiki butun propertileri gondermemek ucun viewa ancaq lazim olanlari!

        private async Task<int> GetPageCountAsync(int take)  //method seyfedeki pagination sayini tapmaq ucun 
        {
            var productCount = await _productService.GetCountAsync();  //productlari cem sayi

            return (int) Math.Ceiling((decimal)productCount / take);  //productlari cem sayi boluruk gotureceymiz product sayina(her seyfede nece olsun product sayina)
                                                                        //Math.Ceeling-istifade edirik yuvarlasdirmaq. meselen(product 5 / page 2)-bize 3 versin deye
        }                                                               //math ceeling methodu bizden decimal teleb edir deye kast edirik decimala
                                                                        //sonra return etdiyimizi yeniden kast edirik int-e return type int-di deye.











        //-----------------Product-Create-With Relation------------------
        [HttpGet]
        public async Task<IActionResult> Create()
        {
           //IEnumerable<Category> categories = await _categoryService.GetAll();  //GetAll-IEnumerable<Category> qaytarir
            //ViewBag.categories = new SelectList(categories, "Id", "Name");  //select tagi ici ucun  categorileri yiqiram,Id ve Name goturem icinden.Id-gedecek selectin valusuna,Name-selectin textine
            ViewBag.categories = await GetCategoriesAsync();
            return View();
        }



        //-----------------Product-Create-With Relation------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM model)
        {
            try
            {
                ViewBag.categories = await  GetCategoriesAsync(); //categorileri gonderirik viewa
               
                if(!ModelState.IsValid) //bos gelirse inputda nese  seyfeye qaytar
                {
                    return View(model); 
                }
                foreach (var photo in model.Photos)     //sekil type yoxla
                {
                    if (!photo.CheckFileType("image/"))   
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        return View();
                    }
                    //if (!photo.CheckFileSize(200))  //sekil size yoxla
                    //{
                    //    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    //    return View();
                    //}
                }

                List<ProductImage> productImages = new();  //bir productun bir nece sekli olar deye Sekiler tipinden Liste yiq
                foreach (var photo in model.Photos)  //her gelen seklilleri foreacha sal
                {
                  
                    string fileName = Guid.NewGuid().ToString() + " " + photo.FileName;
                    string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);
                    await FileHelper.SaveFileAsync(newPath, photo); //her gelen sekli save ele projecte

                    ProductImage newProductImage = new()
                    {
                        Image = fileName  // //her sekli beraber ele databazadaki Image tablin propertisine 
                    };
                    productImages.Add(newProductImage); //her productu beraber eleyende sonr propertiye yiq Liste

                }

                productImages.FirstOrDefault().IsMain = true; //yeni yuklediyimiz sekilerden biri ismaini true olsunki ekranda gorsensin
                decimal convertedPrice = decimal.Parse(model.Price.Replace(".", ","));  //databazada decimaldi deye price decimal kimi gonderik.ve viewdan bize string gelir deye price onu decimala parse edirik ve noqteni vergule deyisirik(deyismesek bize ancaq butov eded verecek qaliq yox)
                Product product = new()                                                  //ProductCreateVM-de propertini stringden qoymusuqki Replace methodu islesin
                {
                    Name = model.Name, //iputdan gelir
                    Price = convertedPrice, //price inputdan gelir
                    Count = model.Count, //count inputdan gelir
                    Description = model.Description, //description inputdan gelir
                    CategoryId = model.CategoryId,  //categori Id-si gelir select-inputdan
                    Images = productImages   //ve Image Listi gelir controllerden (viewdan gebul edib Liste yiqiriq controllerde)
                };

                await _context.ProductImages.AddRangeAsync(productImages); //Listin icinde List elave edende Addrange methodun istifade edirik(Product Image Table-Listdi,butun tablar Listdi!)
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }
            
        }
        private async Task<SelectList> GetCategoriesAsync()
        {
            IEnumerable<Category> categories = await _categoryService.GetAll(); //submit olandada categoriler chekbox gorsensin deye burdada cagiririq
            return  new SelectList(categories, "Id", "Name");
        }


        //----------Product-datalari coxdu deye silmemis Viewa gorsedirik herseyini-----------------------
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id== null) return BadRequest();
           Product product = await _productService.GetFullDataById((int)id);
           if(product == null) return NotFound();
            ViewBag.description = Regex.Replace(product.Description, "<.*?>", String.Empty);  //tagin icinden text goturmeyin yollari(cunku asp-for ucun HTMl.Raw islemir)
            return View(product);                                                     // "<.*?>"=yani bulari atir icinden ve yalniz texti verir
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]  //eyni parametr ve adda actiondu deye ayirmaq ucun basqa ad qoyub ama actiona Name-eyni geyd edirik

        //---------Delete product------------
        public async Task<IActionResult> DeleteProduct(int? id) 
        {
            Product product = await _productService.GetFullDataById((int)id);
            foreach (var item in product.Images)   //bir productun bir nece sekli var deye Listi foreache salib ele silirik bir bir
            {
                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", item.Image);  
                FileHelper.DeleteFile(path);
            }
             
            _context.Products.Remove(product); //databazadan silirik.product silinir deye databazadan ona aid olan sekiler ve categoriler silinir avtomatik
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }





        //------------UPDATE VIEW----------
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null) return BadRequest(); 
            Product product = await _productService.GetFullDataById((int)id);
            if (product == null) return NotFound();
            ViewBag.categories = await GetCategoriesAsync();
            ProductUpdateVM model = new()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Count = product.Count,
                CategoryId = product.CategoryId,
                Images = product.Images
            };
       

            return View(model);


        }

        //------------UPDATE----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,ProductUpdateVM product)
        {
            try
            {
                if (id == null) return BadRequest();

                Product dbProduct = await _productService.GetFullDataById((int)id);

                if (dbProduct == null) return NotFound();
                ViewBag.categories = await GetCategoriesAsync();
                ProductUpdateVM model = new() //kohneynen beraberlesdiririk eyer sekilin size ve tipinde problem olsa viewda kohne datalar qalsin
                {
                    Name = dbProduct.Name,
                    Description = dbProduct.Description,
                    Price = dbProduct.Price,
                    Count = dbProduct.Count,
                    CategoryId = dbProduct.CategoryId,
                    Images = dbProduct.Images

                };
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                if (product.Photos != null)
                {
                    foreach (var photo in product.Photos)
                    {
                        if (!photo.CheckFileType("image/"))
                        {
                            ModelState.AddModelError("Photo", "File type must be image");
                            return View(model);
                        }


                        //if (!photo.CheckFileSize(200))
                        //{
                        //    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        //    return View(model);
                        //}
                    }
                    foreach (var item in dbProduct.Images)
                    {
                        string path = FileHelper.GetFilePath(_env.WebRootPath, "img", item.Image);
                        FileHelper.DeleteFile(path);
                    }

                    List<ProductImage> productImages = new();

                    foreach (var photo in product.Photos)  //her gelen seklilleri foreacha sal
                    {

                        string fileName = Guid.NewGuid().ToString() + " " + photo.FileName;
                        string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);
                        await FileHelper.SaveFileAsync(newPath, photo); //her gelen sekli save ele projecte

                        ProductImage newProductImage = new()
                        {
                            Image = fileName
                        };
                        productImages.Add(newProductImage);

                    }
                    productImages.FirstOrDefault().IsMain = true;
                     _context.ProductImages.AddRange(productImages);
                    dbProduct.Images = productImages;

                }
                else
                {
                    Product prod = new()
                    {
                        Images = dbProduct.Images
                    };
                }





                dbProduct.Name = product.Name;
                dbProduct.Price = product.Price;
                dbProduct.Count = product.Count;
                dbProduct.Description = product.Description;
                dbProduct.CategoryId = product.CategoryId;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));



            }
            catch (Exception)
            {

                throw;
            }
          
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Product dbProduct = await _productService.GetFullDataById((int)id);
            if (dbProduct == null) return NotFound();
            ViewBag.description = Regex.Replace(dbProduct.Description, "<.*?>", String.Empty);
            return View(dbProduct);
        }

    }
}
