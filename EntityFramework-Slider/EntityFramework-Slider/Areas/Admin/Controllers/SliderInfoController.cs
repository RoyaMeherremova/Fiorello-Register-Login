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
    public class SliderInfoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISliderService _sliderService;
        private readonly IWebHostEnvironment _env;
        public SliderInfoController(AppDbContext context,
                                    ISliderService sliderService,
                                    IWebHostEnvironment env)
        {
            _context = context;
            _sliderService = sliderService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<SliderInfo> sliderInfos = await _context.SliderInfos.ToListAsync();
            return View(sliderInfos);
        }



        //-------DETAIL------
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);
            if (sliderInfo == null) return NotFound();
            return View(sliderInfo);
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
        public async Task<IActionResult> Create(SliderInfo sliderInfo)
        {
            try
            {

                if (!sliderInfo.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View();
                }

                if (!ModelState.IsValid)
                {
                    return View();
                }

                if (!sliderInfo.Photo.CheckFileSize(200))
                {
                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View();
                }
                string fileName = Guid.NewGuid().ToString() + " " + sliderInfo.Photo.FileName;

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await sliderInfo.Photo.CopyToAsync(stream);
                }


                sliderInfo.SignatureImage = fileName;

                await _context.SliderInfos.AddAsync(sliderInfo);
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

               SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);

                if (sliderInfo == null) return NotFound();

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", sliderInfo.SignatureImage);

                FileHelper.DeleteFile(path);


                _context.SliderInfos.Remove(sliderInfo);
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
            SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);

            if (sliderInfo == null) return NotFound();

            SliderInfoUpdateVM model = new()
            {
                SignatureImage = sliderInfo.SignatureImage,
                Title = sliderInfo.Title,
                Description = sliderInfo.Description
            };
            return View(model);


        }

        //-----------UPDATE-----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, SliderInfoUpdateVM sliderInfo)
        { 

            try
            {
                if (id == null) return BadRequest();
                SliderInfo dbSliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);

                if (dbSliderInfo == null) return NotFound();


                SliderInfoUpdateVM model = new()
                {
                    SignatureImage = dbSliderInfo.SignatureImage,
                    Title = dbSliderInfo.Title,
                    Description = dbSliderInfo.Description
                };
                if (sliderInfo.Photo != null)
                {
                    if (!sliderInfo.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        return View(model);
                    }


                    if (!sliderInfo.Photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        return View(model);
                    }



                    //update olanda kohne pathi silirik
                    string deletePath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbSliderInfo.SignatureImage);

                    FileHelper.DeleteFile(deletePath);


                    //yeni gelen adla yeni path yaradiriq
                    string fileName = Guid.NewGuid().ToString() + " " + sliderInfo.Photo.FileName;
                    string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                    await FileHelper.SaveFileAsync(newPath, sliderInfo.Photo);

                    //databazaya yeni slider datalarini add edirik
                    dbSliderInfo.SignatureImage = fileName;
                }
                else
                {
                    SliderInfo newSliderInfo = new()    //eyer sekil update olmursa yani sekil fayl gelmirse kohnesi qalsin
                    {
                        SignatureImage = dbSliderInfo.SignatureImage
                    };
                }

                dbSliderInfo.Title = sliderInfo.Title;
                dbSliderInfo.Description = sliderInfo.Description;
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




        
    

