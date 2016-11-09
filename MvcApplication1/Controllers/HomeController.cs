using System.Web.Mvc;

namespace DotyAppServer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {            
            return View();
        }
    }
}
