using DFC.App.JobCategories.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DFC.App.JobCategories.UnitTests.ControllerTests.HomeControllerTests
{
    public class BaseHomeController
    {
        protected HomeController BuildHomeController()
        {
            var controller = new HomeController()
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            return controller;
        }
    }
}
