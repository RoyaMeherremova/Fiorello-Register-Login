using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExpertHeaderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IExpertService _expertService;

        public ExpertHeaderController(IExpertService expertService, AppDbContext context)
        {
            _expertService = expertService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            

            return View(await _expertService.GetExpertsHeader());
        }

      [HttpGet]
       public IActionResult Create()   
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpertsHeader expertsHeader)
        {

            try 
            {
                if (!ModelState.IsValid)  
                {
                    return View();
                }
                await _context.ExpertsHeaders.AddAsync(expertsHeader);  
                await _context.SaveChangesAsync();  
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                return RedirectToAction("Error", new { msj = "hi there!" });  
            }


        }
        public IActionResult Error(string msj)
        {
            ViewBag.error = msj;  //oz yazdqimiz texti gonderik error Viewa
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();  

           ExpertsHeader expertsHeader = await _context.ExpertsHeaders.FindAsync(id); 

            if (expertsHeader is null) return NotFound(); 

            _context.ExpertsHeaders.Remove(expertsHeader); 

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int? id)
        {
            if (id is null) return BadRequest();  //eyer Id(null) gelmirse exseption cixsin

            ExpertsHeader expertsHeader = await _context.ExpertsHeaders.FindAsync(id);  //eyni id-li datani tap

            if (expertsHeader is null) return NotFound();  //eyer bucur Id yoxdursa exseption cixsin

            expertsHeader.SoftDelete = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }






        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();

            ExpertsHeader expertsHeader = await _context.ExpertsHeaders.FindAsync(id);

            if (expertsHeader is null) return NotFound();

            return View(expertsHeader); 

        }





        [HttpPost] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ExpertsHeader expertsHeader)  
        {
            if (id is null) return BadRequest();  

            if (!ModelState.IsValid)
            {
                return View();
            }

            ExpertsHeader dbExpertsHeader = await _context.ExpertsHeaders.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

            if (dbExpertsHeader is null) return NotFound(); 

            if (dbExpertsHeader.Title.Trim().ToLower() == expertsHeader.Title.Trim().ToLower())
            {
                return RedirectToAction("Index");
            }

            _context.ExpertsHeaders.Update(expertsHeader);  

            await _context.SaveChangesAsync();  

            return RedirectToAction("Index");

        }



        [HttpGet]  
        public async Task<IActionResult> Detail(int? id)  
        {
            if (id is null) return BadRequest();

            ExpertsHeader expertsHeader = await _context.ExpertsHeaders.FindAsync(id);

            if (expertsHeader is null) return NotFound();

            return View(expertsHeader);

        }
    }
}

