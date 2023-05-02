using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Services
{
    public class ProductService : IProductService
    {

        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Product>> GetAll() =>  await _context.Products.Include(m => m.Images).Where(m => !m.SoftDelete).ToListAsync();
        public async Task<Product> GetById(int id) => await _context.Products.FindAsync(id);

        public async Task<int> GetCountAsync()  //method for count all product
        {
            return await _context.Products.CountAsync();
        }

        public async Task<Product> GetFullDataById(int id) => await _context.Products.Include(m => m.Images).Include(m => m.Category).FirstOrDefaultAsync(m => m.Id == id);

        public async Task<List<Product>> GetPaginatedDatas(int page,int take)   //categoride lazim olacaq deye ayri servise cixardiriq
        {
           return  await _context.Products.Include(m => m.Images).Include(m=>m.Category).Skip((page*take)-take).Take(take).ToListAsync();
        }
                                                                                           //Skip((page* take)-take)-seyfeni vuruq goturdyumuz product gederine ve cixiriq yene goturduyumuz productun gedere.Meselen(1*5-5)=birinci seyfede hecne skip elemirik=Skip(0).
                                                                                           //Bize contollerden asp taglarnen page gonderecek ona uyqun hesablanacaq ve skip olunacaq ve take hemseki reqemde gonderilceke
    }
}
