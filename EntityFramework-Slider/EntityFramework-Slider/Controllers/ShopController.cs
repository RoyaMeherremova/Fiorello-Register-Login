using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EntityFramework_Slider.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {


            //count- bize databazada olan datalarin sayini verir
            int count = await _context.Products.Where(m => !m.SoftDelete).CountAsync();
            ViewBag.Count = count;
                                                   
            IEnumerable<Product> products =await _context.Products
                                                  .Include(m =>m.Images)
                                                   .Where(m => !m.SoftDelete)
                                                   //.OrderByDescending(m => m.Id)  -Idiye gore coxdan aza siralayir 
                                                   .Take(4)  /*Datanin sayin bildirik negeder getirsin data bazadan deye*/
                                                   //.Skip(8)    say bildirik negeder datanin ustunden kecib qalanini getirsin deye

                                                   .ToListAsync();
            return View(products);
        }

        //method -LoadMore-basqa yani  yerde qalan sekileri getirmek ucun 
        public async Task<IActionResult> LoadMore(int skip)
        {
            IEnumerable<Product> products = await _context.Products
                                                  .Include(m => m.Category)
                                                  .Include(m => m.Images)
                                                   .Where(m => !m.SoftDelete)
                                                   .Skip(skip)
                                                    .Take(4)
                                                   .ToListAsync();
            //PartialView-vasitesi ile elmizde olan partiola produktlari gonderirik
            //Json-vasitesi ile gondere bilmirik cunku Icinde Include var-basqa table 
            return PartialView("_ProductsPartial",products);
        }

        //method -Search
        public IActionResult Search(string searchText)
        {
            var products = _context.Products.Include(m=>m.Images)
                                            .Include(m => m.Category)
                                            .Where(m=>m.Name.ToLower().Contains(searchText.ToLower())) //ajax vasitesi ile bize value gelir ve biz ona uyqun databazadan o Adla olan productlari tapib yiqiriq liste gonderirik Partiala
                                            .Take(5)                      
                                            .ToList();  

            return PartialView("_SearchPartial",products);
        }


  


    }
}


