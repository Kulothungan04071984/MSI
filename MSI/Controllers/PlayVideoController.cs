using Microsoft.AspNetCore.Mvc;

namespace MSI.Controllers
{
    public class PlayVideoController : Controller
    {
        public IActionResult VideoPlaying()
        {
            return View();
        }
    }
}
