using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;

namespace DFC.App.JobCategories.Middleware
{
    public class EventGridTracingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TelemetryConfiguration telemetryConfiguration = TelemetryConfiguration.CreateDefault();
        private readonly TelemetryClient telemetryClient;
        private string traceId;
        private string parentId;

        public EventGridTracingMiddleware(RequestDelegate next)
        {
            _next = next;
            telemetryConfiguration.InstrumentationKey = "af3a60c6-4b76-4cb7-bc6f-b1b4b042bd29";
            telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //Check if the request is destined for webhooks and is of type application/json
            if (context.Request.Path.Value.ToLower().Contains("webhook") && context.Request.Headers.Any(x => x.Key == "Content-Type" && x.Value == "application/json"))
            {
                // IMPORTANT: Ensure the requestBody can be read multiple times.
                context.Request.EnableBuffering();

                using (StreamReader reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true))
                {
                    string strRequestBody = await reader.ReadToEndAsync();
                    var objRequestBody = JsonConvert.DeserializeObject<JArray>(strRequestBody);

                    foreach (var item in objRequestBody)
                    {
                        var jObj = (JObject)item;
                        var data = jObj.Properties().FirstOrDefault(x => x.Name == "data").Value;

                        var dataAsJobj = (JObject)data;

                        traceId = dataAsJobj.Properties().FirstOrDefault(x => x.Name == "traceId").Value.ToString();
                        parentId = dataAsJobj.Properties().FirstOrDefault(x => x.Name == "parentId").Value.ToString();
                    }

                    // IMPORTANT: Reset the request body stream position so the next middleware can read it
                    context.Request.Body.Position = 0;

                    // Let's create and start RequestTelemetry.
                    var requestTelemetry = new RequestTelemetry
                    {
                        Name = $"{context.Request.Method} {context.Request.GetUri().GetLeftPart(UriPartial.Path)}"
                    };

                    requestTelemetry.Context.Operation.Id = traceId;
                    requestTelemetry.Context.Operation.ParentId = parentId;

                    // StartOperation is a helper method that allows correlation of 
                    // current operations with nested operations/telemetry
                    // and initializes start time and duration on telemetry items.
                    var operation = telemetryClient.StartOperation(requestTelemetry);

                    // Process the request.
                    try
                    {
                        await _next.Invoke(context);
                    }
                    catch (Exception e)
                    {
                        requestTelemetry.Success = false;
                        telemetryClient.TrackException(e);
                        throw;
                    }
                    finally
                    {
                        // Update status code and success as appropriate.
                        if (context.Response != null)
                        {
                            requestTelemetry.ResponseCode = context.Response.StatusCode.ToString();
                            requestTelemetry.Success = context.Response.StatusCode >= 200 && context.Response.StatusCode <= 299;
                        }
                        else
                        {
                            requestTelemetry.Success = false;
                        }

                        // Now it's time to stop the operation (and track telemetry).
                        telemetryClient.StopOperation(operation);
                    }
                }
            }

            await _next.Invoke(context);
        }
    }
}
