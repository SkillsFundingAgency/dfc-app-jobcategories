using Dfc.App.JobCategories.ViewModels.Error;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Dfc.App.JobCategories.Controllers
{
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}