using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Our.Community.CustomForm.ViewComponents
{
    public class CaptchaControlViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CaptchaControlViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Captcha captcha = new Captcha(200, 80, 6);
            var img = captcha.GenerateAsB64(Captcha.CaptchaType.Circle);
            _httpContextAccessor?.HttpContext?.Session.SetString("Captcha", captcha.GetAnswer());

            return await Task.FromResult((IViewComponentResult)View("Captcha",Content("data:image/jpg;base64," + img)));
        }

    }
}