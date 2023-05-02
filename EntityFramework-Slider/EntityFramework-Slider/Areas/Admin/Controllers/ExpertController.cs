using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Heplers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class ExpertController : Controller
    {
        private readonly IExpertService _expertService;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ExpertController(IExpertService expertService, 
                                      AppDbContext context,
                                      IWebHostEnvironment env)
        {
            _expertService = expertService;
            _context = context;
            _env = env;
        }


        public async Task<IActionResult> Index()
        {

            return View( await _expertService.GetAll());
        }



        //-------DETAIL------
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Experts expert = await _context.Experts.FirstOrDefaultAsync(m => m.Id == id); 
            if (expert == null) return NotFound();
            return View(expert);
        }

        //-------CREATE------
        [HttpGet]
        public IActionResult Create() 
        {

            return View();
        }


        //-------CREATE------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Experts expert)
        {
            try
            {

                if (!expert.Photo.CheckFileType("image/"))   
                {
                    ModelState.AddModelError("Photo", "File type must be image"); 
                    return View();
                }

                if (!ModelState.IsValid) 
                {
                    return View();
                }

                if (!expert.Photo.CheckFileSize(200))
                {
                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View();
                }
                string fileName = Guid.NewGuid().ToString() + " " + expert.Photo.FileName;

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);  
                using (FileStream stream = new FileStream(path, FileMode.Create)) { 
                    await expert.Photo.CopyToAsync(stream);   
                }


                expert.Image = fileName;  

                await _context.Experts.AddAsync(expert);  
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));  

            }
            catch (Exception ex)
            {

                ViewBag.error = ex.Message;
                return View();
            }

        }




        //-------DELETE-------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id) 
        {
            try
            {
                if (id == null) return BadRequest();

                Experts expert = await _context.Experts.FirstOrDefaultAsync(m => m.Id == id);   

                if (expert == null) return NotFound();

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", expert.Image);    

                FileHelper.DeleteFile(path);


                _context.Experts.Remove(expert);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                ViewBag.error = ex.Message;
                return View();
            }


        }



        //-----------UPDATE-----------
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();
            Experts expert = await _context.Experts.FirstOrDefaultAsync(m => m.Id == id);

            if (expert == null) return NotFound();

            ExpertUpdateVM model = new()
            {
                Image = expert.Image,
                Name = expert.Name,
                Position = expert.Position,
            };
            return View(model);
            
        }



        //-----------UPDATE----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ExpertUpdateVM expert)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(expert);
                }

                if (id == null) return BadRequest();

                Experts dbExpert = await _context.Experts.FirstOrDefaultAsync(m => m.Id == id);

                if (dbExpert == null) return NotFound();

                ExpertUpdateVM model = new() //kohneynen beraberlesdiririk eyer sekilin size ve tipinde problem olsa viewda kohne datalar qalsin
                {
                    Image = dbExpert.Image,
                    Name = dbExpert.Name,
                    Position = dbExpert.Position,
                };
                if (expert.Photo != null)
                {
                    if (!expert.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        return View(model);
                    }


                    if (!expert.Photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        return View(model);
                    }



                    //update olanda kohne pathi silirik
                    string deletePath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbExpert.Image);

                    FileHelper.DeleteFile(deletePath);


                    //yeni gelen adla yeni path yaradiriq
                    string fileName = Guid.NewGuid().ToString() + " " + expert.Photo.FileName;
                    string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                    await FileHelper.SaveFileAsync(newPath, expert.Photo);

                    //databazaya yeni slider datalarini add edirik
                    dbExpert.Image = fileName;
                }
                else
                {
                    Experts newExpert = new()    //eyer sekil update olmursa yani sekil fayl gelmirse kohnesi qalsin
                    {
                        Image = dbExpert.Image,
                    };
                }
                dbExpert.Name = expert.Name;
                dbExpert.Position = expert.Position;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }

        }





    }
}
