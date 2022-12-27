using AIService.Entities;
using Serilog;
using AIService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace AIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrestashopController : ControllerBase
    {
        private static string label = "Controllers";
        private static string className = "PrestashopController";
        private readonly AppSettings appSettings;

        public PrestashopController(IOptionsSnapshot<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        [HttpPost("{resource}/{operation}")]
        public async Task<IActionResult> ResourceOperation(string resource, string operation, [FromBody] JObject data)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "ResourceOperation";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {resource} - {operation}: {JsonConvert.SerializeObject(data)}");
                switch (operation)
                {
                    case "get":
                        response.Result = await PrestashopService.GetResource(appSettings, requestInDTO, resource, CommonService.JObjectToDictionary(data));
                        break;
                    case "post":
                        response.Result = await PrestashopService.PostResource(appSettings, requestInDTO, resource, CommonService.JObjectToDictionary(data));
                        break;
                    case "put":
                        response.Result = await PrestashopService.PutResource(appSettings, requestInDTO, resource, CommonService.JObjectToDictionary(data));
                        break;
                    default:
                        break;
                }
                
                response.Success = true;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Debug($"RESULT: {resource} - {operation}: {JsonConvert.SerializeObject(response)} Execution Time: {watch.ElapsedMilliseconds} ms");
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Msg = e.Message;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"ERROR: {resource} - {operation}: {JsonConvert.SerializeObject(data)}");
                return BadRequest(response);
            }
        }
    }
}
