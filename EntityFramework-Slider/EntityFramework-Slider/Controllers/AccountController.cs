using EntityFramework_Slider.Models;
using EntityFramework_Slider.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EntityFramework_Slider.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;  //UserManager-Identity tablarla islemek ucun -databazaynan(Create elemey user uzerinde isler gormek)
        private readonly SignInManager<AppUser> _signInManager; //SignInManager-sayta giris elemey ucun (sayta giris etmek log out olmaq)
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }





        //----------SAYTA REGISTER VIEW----------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }




        //----------SAYTA REGISTER----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)  //bos gelen input olarsa viewa qayit dolduruqun datalarla
            {
                return View(model);
            }

            AppUser newUser = new()  //bizim databazadaki user modelmize assayn edirik Viwnodelmizden gelenlere
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.FullName,
                //Paswordumuz heslenecek deye onu ayrica gonderik
            };
            IdentityResult result = await _userManager.CreateAsync(newUser, model.Password);     //CreateAsync -user yaratmaq ucun (databazaya save edir)

            if (!result.Succeeded)  //eyer giris ugursuz olarsa(qoyduqumuz sertlere uyqun olmazsa)
            {
                foreach (var item in result.Errors)  //errorlar List seklindedi
                {
                    ModelState.AddModelError(string.Empty, item.Description);   //string.Empty-her hansisa filt altinda yazilmasin deye,item.Description-errorlari icliyi
                }
                return View(model);
            }


            return RedirectToAction(nameof(Login));
        }



        //----------SAYTA LOGIN OLUB DAXIL OLMAQ----------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            AppUser user = await _userManager.FindByEmailAsync(model.EmaiOrUsername);  //tapaq bize gelen email varmi databazada(saytdan ya usernam gelir ya email)
            
            if(user is null)  //eyer o adda email yoxdusa
            {
                user = await _userManager.FindByNameAsync(model.EmaiOrUsername); //tapaq bize gelen adda varmi databazada(saytdan ya usernam gelir ya email)
            }
            if(user is null) //o adda hem email hem username yoxdursa
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (!result.Succeeded) //eyer pasword sehvdirse
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong");
                return View(model);
            }
            return RedirectToAction("Index","Home");
        }



        //----------SAYTDAN CIXIS----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();  //kim girmis vezyetdedise cixsin ordan(sessiondan tokeni silir)
            return RedirectToAction("Index","Home");
        }
    }
}
