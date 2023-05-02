using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.ViewComponents
{
    public class BlogViewComponent : ViewComponent  //seyfenin tekrarlan hisseleri ucun component yazib seyfenin Viewsunda cagiriq(yada seliqe ucun tekrarlanmayanlari yaziirq)
    {
        private readonly IBlogService _blogService;

        public BlogViewComponent(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public async Task<IViewComponentResult> InvokeAsync()  //comonentde bir nov Index action mentiqidi -InvokeAsync()
        {      
            return await Task.FromResult(View(new BlogVM { Blogs = await _blogService.GetAll(),BlogHeader = await _blogService.GetBlogHeader()}));       //Invoke bize bucur return edir Task.FromResult
        }



        //InokeAsync-na -- argument gondermek olur.Seyfede componenti cagiranda Invoke(icine arqument) gonderik
        //public async Task<IViewComponentResult> InvokeAsync(int skip)  //comonentde bir nov Index action mentiqidi -InvokeAsync()
        //{
        //    return await Task.FromResult(View(await _context.Blogs.Skip(skip).Take(3).ToListAsync()));       //Invoke bize bucur return edir Task.FromResult
        //}
    }
}
