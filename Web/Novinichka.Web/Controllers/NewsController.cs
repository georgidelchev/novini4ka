using Microsoft.AspNetCore.Mvc;

namespace Novinichka.Web.Controllers
{
    public class NewsController : BaseController
    {
        public NewsController()
        {
        }

        [HttpGet]
        public IActionResult All()
        {
            return this.View();
        }
    }
}
