using EntityFramework_Slider.Data;
using EntityFramework_Slider.Heplers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public BlogController(IBlogService blogService,
                                  AppDbContext context, IWebHostEnvironment env)
        {
            _blogService = blogService;
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {

            return View(await _blogService.GetAll());
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {
            try
            {

                if (!blog.Photo.CheckFileType("image/"))  
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View();
                }
                if (!ModelState.IsValid)
                {
                    return View();
                }
                //if (!blog.Photo.CheckFileSize(200))  
                //{
                //    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                //    return View();
                //}

                string fileName = Guid.NewGuid().ToString() + " " + blog.Photo.FileName;
                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))  
                {
                    await blog.Photo.CopyToAsync(stream);  
                }

                blog.Image = fileName;  

                await _context.Blogs.AddAsync(blog); 
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
                 
        
                catch (Exception ex)
                {
                ViewBag.error = ex.Message;
                return View();
            }


        }


        public IActionResult Error(string msj)
        {
            ViewBag.error = msj;  
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
         
            if (id == null) return BadRequest();
            Blog blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null) return NotFound();
            return View(blog);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null) return BadRequest();

                Blog blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);

                if (blog == null) return NotFound();

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", blog.Image);


                FileHelper.DeleteFile(path);


                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            catch (Exception)
            {

                return RedirectToAction("Error");
            }


        }



        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();
            Blog blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);

            if (blog == null) return NotFound();

            return View(blog);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,Blog blog)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View(blog);
                }

                if (id == null) return BadRequest();

                Blog dbBlog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);

                if (dbBlog == null) return NotFound();

              

                    if (!blog.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View(dbBlog);
                }

                if (!blog.Photo.CheckFileSize(200))
                {
                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View(dbBlog);
                }


                //update olanda kohne pathi silirik
                string deletePath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbBlog.Image);

                FileHelper.DeleteFile(deletePath);


                //yeni gelen adla yeni path yaradiriq
                string fileName = Guid.NewGuid().ToString() + " " + blog.Photo.FileName;
                string newPath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                using (FileStream stream = new FileStream(newPath, FileMode.Create))
                {
                    await blog.Photo.CopyToAsync(stream);
                }

                //databazaya add edirik
                dbBlog.Image = fileName;
                dbBlog.Header = blog.Header;
                dbBlog.Description = blog.Description;
                dbBlog.Date = blog.Date;

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }
         
        }
    }



  





}
