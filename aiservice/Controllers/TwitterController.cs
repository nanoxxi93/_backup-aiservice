using aiservice.Services;
using AIService.Entities;
using Serilog;
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
    public class TwitterController : ControllerBase
    {
        private static string label = "Controllers";
        private static string className = "TwitterController";
        private readonly AppSettings appSettings;
        public TwitterController(IOptionsSnapshot<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        [HttpGet("bearer")]
        public async Task<IActionResult> Bearer()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "TweetsSearch";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: ");
                response.Result = await TwitterService.Bearer(appSettings, requestInDTO);
                response.Success = true;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Debug($"RESULT: {JsonConvert.SerializeObject(response)} Execution Time: {watch.ElapsedMilliseconds} ms");
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Msg = e.Message;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"ERROR: -");
                return BadRequest(response);
            }
        }

        [HttpPost("tweets/search")]
        public async Task<IActionResult> TweetsSearchStandard([FromBody] JObject request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "TweetsSearchStandard";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(request)}");
                response.Result = await TwitterService.TweetsSearchStandard(appSettings, requestInDTO, request);
                response.Success = true;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Debug($"RESULT: {JsonConvert.SerializeObject(response)} Execution Time: {watch.ElapsedMilliseconds} ms");
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Msg = e.Message;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"ERROR: {JsonConvert.SerializeObject(request)}");
                return BadRequest(response);
            }
        }

        [HttpPost("tweets/search/premium")]
        public async Task<IActionResult> TweetsSearchPremium([FromBody] JObject request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "TweetsSearchPremium";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(request)}");
                response.Result = await TwitterService.TweetsSearchPremium(appSettings, requestInDTO, request);
                response.Success = true;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Debug($"RESULT: {JsonConvert.SerializeObject(response)} Execution Time: {watch.ElapsedMilliseconds} ms");
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Msg = e.Message;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"ERROR: {JsonConvert.SerializeObject(request)}");
                return BadRequest(response);
            }
        }
    }
}
