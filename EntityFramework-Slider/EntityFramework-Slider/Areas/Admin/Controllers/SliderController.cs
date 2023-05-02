using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Heplers;
using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env; //IWebHostEnvironment-bu sistem terefden verilir #root -a catmaq ucun
        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


       // -------ALL SLIDER-------
        public async Task<IActionResult> Index()
        {
            IEnumerable<Slider> sliders = await _context.Sliders.Where(m => !m.SoftDelete).ToListAsync();
            return View(sliders);
        }



        

        //-----------PHOTO DETAIL--------   //Info butona basanda bu actiona yonlenir
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            //eyer kimse urlden UI-dan yani  id--ni silse seyfe baglansin
            //BadRequest=Exception cixaririq
            if (id == null) return BadRequest();
            Slider? slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);
            //eyer kimse sehv regem yazibsa Url-e
            //NotFound-tapilmadi deye Exception 
            if(slider == null) return NotFound();
            return View(slider);
        }




        //------PHOTO CREATE VIEW--------
        [HttpGet]
        public IActionResult Create()    /*async-elemirik cunku data gelmir databazadan*/
        {
           
            return View();
        }




        //-----PHOTO UPLOAD MULTIPLY------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderCreateVM slider)  //bize multiple-inputdan bir nece sekil gelir List sekilinde
        {
            try
            {
                if (!ModelState.IsValid) 
                {
                    return View();
                }

                foreach (var photo in slider.Photos)  //bize egelen List sekilde fayllari foreachde bir bir yoxlayiriq
                {
                    if (!photo.CheckFileType("image/"))   //sekildimi?
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        return View();
                    }
                    //if (!photo.CheckFileSize(200))  //Length/1024 -Length seklin Size(olcusun) boluruk 1204-kilobayta cevirmek ucun ve yoxlayiriq 200kb boyukudurse yukleme
                    //{
                    //    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    //    return View();
                    //}
         

                }
                foreach (var photo in slider.Photos)   //ayri foreachde yazmaqa sebeb yuxardaki foreachde olan datanin biri serti qarsilamirsa biri save olacaq biri yox yani qarsiqliq duser deye
                {
                    //yeni gelen adlar ucun yeni pathlar yaradiriq
                    string fileName = Guid.NewGuid().ToString() + " " + photo.FileName;
                    string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);
                    await FileHelper.SaveFileAsync(newPath, photo);

                    Slider newSlider = new()  //databazamiz Slider tipinden gebul edir deye ondan instans aliriq
                    {
                        Image = fileName  //elmizdeki adi assign edirik ona
                    };
                    await _context.Sliders.AddAsync(newSlider);
                   
                }
                await _context.SaveChangesAsync(); //forachden colde yaziriq sonda hamsin  save elesin deye
                return RedirectToAction(nameof(Index));

            }
            catch (Exception)
            {

                throw;
            }
          
        }




        //-------PHOTO UPLOAD------
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Slider slider)
        //{
        //    try
        //    {

        //        if (!slider.Photo.CheckFileType("image/"))   //bize gelen faylin tipi.icinde image formati varmi yoxlayiriq
        //        {
        //            ModelState.AddModelError("Photo", "File type must be image");  //erroe gonderik viewa (spana)
        //            return View();
        //        }

        //        if (!ModelState.IsValid) //eyer input bos olanda submit olarsa hemin seyfede qalsin
        //        {
        //            return View();
        //        }

        //        //if(!slider.Photo.CheckFileSize(200))  //Length/1024 -Length seklin Size(olcusun) boluruk 1204-kilobayta cevirmek ucun ve yoxlayiriq 200kb boyukudurse yukleme
        //        //{
        //        //    ModelState.AddModelError("Photo", "Image size must be max 200kb");
        //        //    return View();
        //        //}
        //        string fileName = Guid.NewGuid().ToString() + " " + slider.Photo.FileName; // Guid.NewGuid().ToString()--bize gelen seklin qabaqina herdefe ferlqli string qoymaq ucun,yani eyni adda sekil yuklense lahiyede problem olmasin deye ve + sekilin adi

        //        string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);  //Path.Combine-birlesdirmeye komek edir-/root/img + bize gelen       //   (_env.WebRootPath)-lahiyedeki rootu verir bize. //Path.Combinle birlesdiririk

        //        using (FileStream stream = new FileStream(path, FileMode.Create))  //FileStream-odurki bir fayli harasa save etmek isteyirikse bir axin yaradiriq ve onun vasitesi ile save edirik yaratdiqimiz patha
        //        {
        //            await slider.Photo.CopyToAsync(stream);   // CopyTo-fotonu yaratdiqimiz streama copy etmek ucundu.yani bu sekli fiziki olaraq kopyuterde saxlamaq ucun  ve fayllarnan ve sqlnen isleyende await yaziriq async
        //        }


        //        slider.Image = fileName;   //seklin adi databazada olacaq beraber olsun bize gelen seklin adina

        //        await _context.Sliders.AddAsync(slider);  // seklin adini save edirik databazaya 
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));  //qayitsin butun slider sekiller olan yere

        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //}














        //-------PHOTO DELETE FROM PROJECT AND DATABASE-------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)  //sekli silmek hem lahiyeden hemde databazadan
        {
            try
            {
                   if(id == null) return BadRequest();

                   Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);   //datani idle tapiriq

                  if (slider == null) return NotFound();

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", slider.Image);    //static method cagirib -data path tapiriq hansi sekil fiziki olaraq lahiyeden silinsin



                //if(System.IO.File.Exists(path))  //System.IO.File.Exist()-sistemden gelir yoxlayir sistemde bele bir root varmi 
                //{
                //    System.IO.File.Delete(path);   //sistemden bu rootu sil

                //}

                FileHelper.DeleteFile(path);  //static method fayla aid pathi yoxlayib silmek ucun cagiririq


                _context.Sliders.Remove(slider);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            catch (Exception)
            {

                throw;
            }
            
          
        }




        //-----------PHOTO UPDATE-----------
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);
           
            if (slider == null) return NotFound();
            return View(slider);
         
        }



        //-----------PHOTO UPDATE----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Slider slider)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(slider);
                }

                if (id == null) return BadRequest();

                Slider dbSlider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);

                if (dbSlider == null) return NotFound();

             

                if (!slider.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View(dbSlider);
                }


                if (!slider.Photo.CheckFileSize(200))
                {
                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View(dbSlider);
                }



                //update olanda kohne pathi silirik
                string deletePath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbSlider.Image);

                FileHelper.DeleteFile(deletePath);


                //yeni gelen adla yeni path yaradiriq
                string fileName = Guid.NewGuid().ToString() + " " + slider.Photo.FileName;
                string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                await FileHelper.SaveFileAsync(newPath,slider.Photo);

                //databazaya yeni slider datalarini add edirik
                dbSlider.Image = fileName;               
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }

        }




        //---------ACTIVE-DEACTIVE METHOD--------------


        [HttpPost]
        public async Task<IActionResult> SetStatus(int? id)
        {
            if (id is null) return BadRequest();
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);
           
            if (slider == null) return NotFound();

            slider.SoftDelete = !slider.SoftDelete; //eyer true-dusa false olsun,false-dusa true olsun(tersine beraber olsun)
            //if (slider.SoftDelete)  //eyer true-dusa false olsun
            //{
            //    slider.SoftDelete = false;
            //}
            //else  //false-dusa true olsun
            //{
            //    slider.SoftDelete = true;
            //}
            await _context.SaveChangesAsync();
            return Ok(slider.SoftDelete);
        }

    }
}
