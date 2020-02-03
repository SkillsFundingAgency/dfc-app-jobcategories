using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Net;
using System.Net.Mime;

namespace Dfc.App.JobCategories.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult NegotiateContentResult(this Controller controller, object viewModel, object dataModel = null)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            if (!controller.Request.Headers.Keys.Contains(HeaderNames.Accept))
            {
                return controller.StatusCode((int)HttpStatusCode.NotAcceptable);
            }

            var acceptHeaders = controller.Request.Headers[HeaderNames.Accept].ToString().ToUpperInvariant().Split(';');
            foreach (var acceptHeader in acceptHeaders)
            {
                var items = acceptHeader.Split(',');
                if (items.Contains(MediaTypeNames.Application.Json.ToUpperInvariant()))
                {
                    return controller.Ok(dataModel ?? viewModel);
                }

                if (items.Contains(MediaTypeNames.Text.Html) || items.Contains("*/*"))
                {
                    return controller.View(viewModel);
                }
            }

            return controller.StatusCode((int)HttpStatusCode.NotAcceptable);
        }
    }
}