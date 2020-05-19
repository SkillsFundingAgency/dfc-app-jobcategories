using DFC.App.JobCategories.Controllers;
using DFC.App.JobCategories.PageService.EventProcessorServices;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace DFC.App.JobCategories.UnitTests.ControllerTests.WebhooksControllerTests
{
    public abstract class BaseWebhooksController
    {
        protected const string EventTypePublished = "Published";
        protected const string EventTypeDraft = "Draft";
        protected const string EventTypeDeleted = "Deleted";

        protected const string ContentTypeContactUs = "contact-us";

        protected BaseWebhooksController()
        {
            Logger = A.Fake<ILogger<WebhooksController>>();
            FakeEventMessageService = A.Fake<IEventProcessingService>();
        }

        protected ILogger<WebhooksController> Logger { get; }

        protected IEventProcessingService FakeEventMessageService { get; }

        protected static Stream BuildStreamFromModel<TModel>(TModel model)
        {
            var jsonData = JsonConvert.SerializeObject(model);
            byte[] byteArray = Encoding.ASCII.GetBytes(jsonData);
            MemoryStream stream = new MemoryStream(byteArray);

            return stream;
        }

        protected WebhooksController BuildWebhooksController(string mediaTypeName)
        {
            var objectValidator = A.Fake<IObjectModelValidator>();
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new WebhooksController(Logger, FakeEventMessageService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
                ObjectValidator = objectValidator,
            };

            A.CallTo(() => controller.ObjectValidator.Validate(A<ActionContext>.Ignored, A<ValidationStateDictionary>.Ignored, A<string>.Ignored, A<object>.Ignored));

            return controller;
        }
    }
}