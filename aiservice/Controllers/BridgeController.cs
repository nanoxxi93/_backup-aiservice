﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AIService.Entities;
using Serilog;
using AIService.Services;

namespace AIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BridgeController : ControllerBase
    {
        private static string label = "Controllers";
        private static string className = "BridgeController";
        private static HttpClient httpClient = new HttpClient();
        private static HttpRequestBody httpRequestBody = new HttpRequestBody();
        private static HttpResponseMessage responseString = new HttpResponseMessage();
        private string bridgeurl = $"";
        private readonly AppSettings appSettings;

        public BridgeController(IOptionsSnapshot<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        [HttpPost("naturallanguageclassifier")]
        public async Task<IActionResult> NaturalLanguageClassifier([FromForm] IFormCollection collection)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "NaturalLanguageClassifier";
            ResponseDTO response = new ResponseDTO();
            List<string> textList = new List<string>();
            textList = await CommonService.FormDataToListString(collection);
            JObject keys = new JObject();
            keys = await CommonService.FormDataToJObject(collection);
            try
            {
                List<JObject> result = new List<JObject>();
                foreach(string row in textList)
                {
                    Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                        .Information($"REQUEST: {JsonConvert.SerializeObject(row)}");
                    JObject result0 = new JObject();
                    string rowtemp = null;
                    foreach (var k in keys)
                    {
                        string _value = "";
                        string _output = "";
                        rowtemp = rowtemp != null ? rowtemp : row;
                        if (k.Value.ToString().Contains("{{text}}", StringComparison.InvariantCultureIgnoreCase))
                            _value = k.Value.ToString().Replace("{{text}}", rowtemp, StringComparison.InvariantCultureIgnoreCase);
                        if (k.Value.ToString().Contains("_output", StringComparison.InvariantCultureIgnoreCase))
                            _output = k.Value["_output"].ToString();
                        JObject httpbody = JObject.Parse(_value);
                        httpRequestBody.Body = new StringContent(httpbody.ToString(), Encoding.UTF8, "application/json");
                        responseString = await httpClient.PostAsync(k.Key, httpRequestBody.Body);
                        if (responseString.IsSuccessStatusCode)
                        {
                            var content = await responseString.Content.ReadAsStringAsync();
                            var httpresponse = JObject.Parse(content);
                            rowtemp = httpresponse[_output].ToString();
                            result0[k.Key] = httpresponse;
                        }
                    }
                    result.Add(JObject.FromObject(new { text = row, result = result0 }));
                }
                response.Result = result;
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
                    .Error(e, $"ERROR: {JsonConvert.SerializeObject(collection)}");
                return BadRequest(response);
            }
        }
    }
}
