using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddSession(); //Sesion istifade edecymizi bildiririk


builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<AppUser,IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); //(AddIdentity-oz model idenity olacaq birde rollari,AddEntityFrameworkStores-saxlanma yeri olacaq,AddDefaultTokenProviders-sessionda haslanmis datalar saxlamaq ucun )

builder.Services.Configure<IdentityOptions>(opt =>  //sertler geydiyat ucun
{
    opt.Password.RequiredLength = 8;  //mutleq 8 sayda olsun paswordd
    opt.Password.RequireDigit = true;  //mutleq regem olsun paswordda
    opt.Password.RequireLowercase = true; //pasword icinde kicik herf olsun
    opt.Password.RequireUppercase = true; //pasword icinde boyuk herf olsun
    opt.Password.RequireNonAlphanumeric= true; //herf ve regem olmayanda olsun(isareler)

    opt.User.RequireUniqueEmail=true;  //her userin oz emaili olmalidi(unique)

    opt.Lockout.MaxFailedAccessAttempts= 3; //3 defe tekrar tekrar sehv girse user block olsun
    opt.Lockout.DefaultLockoutTimeSpan= TimeSpan.FromMinutes(30); //sehv giribse nece defe nece deqeden sonra cehd etsin
    opt.Lockout.AllowedForNewUsers= true; //teze geydiydan kecenler bir nece defe sehv ede bilsin block olmasin tezeler


});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //bu servislerde Coockideki datalara catmaq ucundu(istifade edeceymizi burda bildirik)
builder.Services.AddScoped<ILayoutService, LayoutService>(); //istifade edeceymiz servisin adin bildirik  AddScopped-birdefe request atir onu istifade edir ama yeni insatnsda tezesin yaradir,ama varsa(request) kohnesin istifade edir
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISliderService, SliderService>();
builder.Services.AddScoped<IExpertService, ExpertService>();
builder.Services.AddScoped<IFooterService, FooterService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();   //Sesion istifade edecymizi bildiririk
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


//for-adminPanel
app.MapControllerRoute(
     name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
