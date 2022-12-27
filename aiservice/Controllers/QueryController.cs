using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using AIService.Entities;
using Serilog;
using AIService.Services;

namespace AIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class QueryController : ControllerBase
    {
        private static string label = "Controllers";
        private static string className = "QueryController";
        private readonly AppSettings appSettings;

        public QueryController(IOptionsSnapshot<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        [HttpPost]
        [Route("GetCollection")]
        public async Task<IActionResult> GetCollection([FromBody] QueryBody queryBody)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "GetCollection";
            ResponseDTO response = new ResponseDTO();
            Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                .Information($"REQUEST {Request.Headers["guid"].ToString()}: {JsonConvert.SerializeObject(queryBody)}");
            try
            {
                response.Result = await QueryService.GetCollection_SP_Dic(appSettings, requestInDTO, queryBody);
                response.Success = true;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Debug($"RESULT {Request.Headers["guid"].ToString()}: {JsonConvert.SerializeObject(response)} Execution Time: {watch.ElapsedMilliseconds} ms");
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                if (((Npgsql.PostgresException)e).MessageText != "")
                    response.Msg = ((Npgsql.PostgresException)e).MessageText;
                else
                    response.Msg = "Hubo un problema vuelva a intentarlo.";
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"ERROR: {JsonConvert.SerializeObject(queryBody)} Execution Time: {watch.ElapsedMilliseconds} ms");
                return BadRequest(response);
            }
        }

        [HttpPost]
        [Route("Get")]
        public async Task<IActionResult> Get([FromBody] QueryBody queryBody)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "Get";
            ResponseDTO response = new ResponseDTO();
            Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                .Information($"REQUEST {Request.Headers["guid"].ToString()}: {JsonConvert.SerializeObject(queryBody)}");
            try
            {
                response.Result = await QueryService.Get_SP_Dic(appSettings, requestInDTO, queryBody);
                response.Success = true;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Debug($"RESULT {Request.Headers["guid"].ToString()}: {JsonConvert.SerializeObject(response)} Execution Time: {watch.ElapsedMilliseconds} ms");
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                if (((Npgsql.PostgresException)e).MessageText != "")
                    response.Msg = ((Npgsql.PostgresException)e).MessageText;
                else
                    response.Msg = "Hubo un problema vuelva a intentarlo.";
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"ERROR: {JsonConvert.SerializeObject(queryBody)} Execution Time: {watch.ElapsedMilliseconds} ms");
                return BadRequest(response);
            }
        }
    }
}
