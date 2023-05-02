using EntityFramework_Slider.Services;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EntityFramework_Slider.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly ILayoutService _layoutService;   //layout service hazir var deye onu istifade edirik header componenti ucun

        public HeaderViewComponent(ILayoutService layoutService)
        {
            _layoutService = layoutService;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult(View( _layoutService.GetSettingDatas()));       //Invoke bize bucur return edir Task.FromResult
        }

    }
}
