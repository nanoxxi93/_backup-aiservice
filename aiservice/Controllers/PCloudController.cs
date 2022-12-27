using AIService.Entities;
using Serilog;
using AIService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace aiservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PCloudController : ControllerBase
    {
        private static string label = "Controllers";
        private static string className = "PCloudController";
        private readonly AppSettings appSettings;

        public PCloudController(IOptionsSnapshot<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        [HttpPost("{resource}/{operation?}")]
        public async Task<IActionResult> ResourceOperation(string resource, string operation, [FromBody] JObject data)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "ResourceOperation";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {resource}: {JsonConvert.SerializeObject(data)}");
                switch (resource)
                {
                    case "login":
                        response.Result = CommonService.StringToJObject(await PCloudService.Login(appSettings, CommonService.JObjectToDictionaryString(data)));
                        break;
                    case "folder":
                        response.Result = CommonService.StringToJObject(await PCloudService.Folder(appSettings, CommonService.JObjectToDictionaryString(data), operation));
                        break;
                    case "file":
                        response.Result = CommonService.StringToJObject(await PCloudService.File(appSettings, CommonService.JObjectToDictionaryString(data), operation));
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
