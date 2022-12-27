using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AIService.Entities;
using AIService.Services;
using Serilog;
using aiservice.Services;
using System.Collections.Generic;
using IBM.Watson.ToneAnalyzer.v3.Model;
using System.Threading;

namespace AIService.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class WatsonController : ControllerBase
    {
        private static string label = "Controllers";
        private static string className = "WatsonController";
        private readonly AppSettings appSettings;

        public WatsonController(IOptionsSnapshot<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        [HttpPost("iamtoken")]
        public async Task<IActionResult> IAMToken([FromBody] JObject request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "IAMToken";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(request)}");
                response.Result = await ObjectStorageService.IamToken(appSettings, requestInDTO, request["apikey"].ToString());
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

        [HttpPost("discovery")]
        public async Task<IActionResult> Discovery([FromBody] JObject request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "Discovery";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(request)}");
                DiscoveryRequest requestBody = request.ToObject<DiscoveryRequest>();
                switch (requestBody.Operation)
                {
                    case "environment":
                        response.Result = await Task.Run(() => DiscoveryService.Environments(appSettings, requestInDTO, requestBody));
                        break;
                    case "collection":
                        response.Result = await Task.Run(() => DiscoveryService.Collections(appSettings, requestInDTO, requestBody));
                        break;
                    case "field":
                        response.Result = await Task.Run(() => DiscoveryService.CollectionFields(appSettings, requestInDTO, requestBody));
                        break;
                    case "query":
                        response.Result = await Task.Run(() => DiscoveryService.QueryCollection(appSettings, requestInDTO, requestBody));
                        break;
                    default:
                        break;
                }
                response.Success = true;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"RESULT: {JsonConvert.SerializeObject(response)} Execution Time: {watch.ElapsedMilliseconds} ms");
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

        [HttpPost("objectstorage")]
        public async Task<IActionResult> ObjectStorage([FromBody] JObject request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "ObjectStorage";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(request)}");
                response.Result = await ObjectStorageService.Options(appSettings, requestInDTO, CommonService.JObjectToDictionary(request));
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

        [HttpPost("objectstorage/upload")]
        public async Task<IActionResult> ObjectStorageUpload()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "ObjectStorageUpload";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(Request.Form)}");
                response.Result = await ObjectStorageService.Upload(appSettings, requestInDTO, Request.Form);
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
                    .Error(e, $"ERROR: {JsonConvert.SerializeObject(Request.Form)}");
                return BadRequest(response);
            }
        }

        [DisableRequestSizeLimit]
        [HttpPost("objectstorage/multipartupload")]
        public async Task<IActionResult> ObjectStorageMultipartUpload()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "ObjectStorageMultipartUpload";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(Request.Form)}");
                response.Result = await ObjectStorageService.MultipartUpload(appSettings, requestInDTO, Request.Form);
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
                    .Error(e, $"ERROR: {JsonConvert.SerializeObject(Request.Form)}");
                return BadRequest(response);
            }
        }

        [HttpPost("objectstorage/download")]
        public async Task<IActionResult> ObjectStorageDownload([FromBody] JObject request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "ObjectStorageDownload";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(request)}");
                return File(await ObjectStorageService.Download(appSettings, requestInDTO, CommonService.JObjectToDictionary(request)), "application/octet-stream");
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

        [HttpPost("languagetranslator")]
        public async Task<IActionResult> LanguageTranslator([FromBody] JObject request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "LanguageTranslator";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(request)}");
                LanguageTranslatorRequest requestBody = request.ToObject<LanguageTranslatorRequest>();
                response.Result = await Task.Run(() => LanguageTranslatorService.LanguageTranslator(appSettings, requestInDTO, requestBody));
                response.Success = true;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"RESULT: {JsonConvert.SerializeObject(response)} Execution Time: {watch.ElapsedMilliseconds} ms");
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

        [HttpPost("naturallanguageclassifier/training")]
        public async Task<IActionResult> NaturalLanguageClassifierTraining([FromForm] IFormCollection collection)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "NaturalLanguageClassifierTraining";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(collection)}");
                var file = collection.Files[0];
                response.Result = await Task.Run(() => NaturalLanguageClassifierService.NaturalLanguageClassifierTraining(appSettings, requestInDTO, file, collection));
                response.Success = true;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"RESULT: {JsonConvert.SerializeObject(response)} Execution Time: {watch.ElapsedMilliseconds} ms");
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

        [HttpPost("naturallanguageclassifier")]
        public async Task<IActionResult> NaturalLanguageClassifier([FromBody] JObject request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "NaturalLanguageClassifier";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(request)}");
                NaturalLanguageClassifierRequest requestBody = request.ToObject<NaturalLanguageClassifierRequest>();
                switch (requestBody.Method.ToLower())
                {
                    case "classify":
                        response.Result = await Task.Run(() => NaturalLanguageClassifierService.NaturalLanguageClassifier(appSettings, requestInDTO, requestBody));
                        break;
                    case "list":
                        response.Result = await Task.Run(() => NaturalLanguageClassifierService.NaturalLanguageClassifierList(appSettings, requestInDTO, requestBody));
                        break;
                    case "detail":
                        response.Result = await Task.Run(() => NaturalLanguageClassifierService.NaturalLanguageClassifierDetail(appSettings, requestInDTO, requestBody));
                        break;
                    case "delete":
                        response.Result = await Task.Run(() => NaturalLanguageClassifierService.NaturalLanguageClassifierDelete(appSettings, requestInDTO, requestBody));
                        break;
                    default:
                        break;
                }
                response.Success = true;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"RESULT: {JsonConvert.SerializeObject(response)} Execution Time: {watch.ElapsedMilliseconds} ms");
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

        [HttpPost("naturallanguageunderstanding")]
        public async Task<IActionResult> NaturalLanguageUnderstanding([FromBody] JObject request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "NaturalLanguageUnderstanding";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(request)}");
                NaturalLanguageUnderstandingRequest requestBody = request.ToObject<NaturalLanguageUnderstandingRequest>();
                response.Result = await Task.Run(() => NaturalLanguageUnderstandingService.NaturalLanguageUnderstanding(appSettings, requestInDTO, requestBody));
                response.Success = true;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"RESULT: {JsonConvert.SerializeObject(response)} Execution Time: {watch.ElapsedMilliseconds} ms");
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

        [HttpPost("speechtotext")]
        public async Task<IActionResult> SpeechToText([FromBody] JObject request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "SpeechToText";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(request)}");
                SpeechToTextRequest requestBody = request.ToObject<SpeechToTextRequest>();
                response.Result = await SpeechToTextService.SpeechToText(appSettings, requestInDTO, requestBody);
                response.Success = true;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"RESULT: {JsonConvert.SerializeObject(response)} Execution Time: {watch.ElapsedMilliseconds} ms");
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

        [HttpPost("texttospeech")]
        public async Task<IActionResult> TextToSpeech([FromBody] JObject request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "TextToSpeech";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(request)}");
                TextToSpeechRequest requestBody = request.ToObject<TextToSpeechRequest>();
                response.Result = await Task.Run(() => TextToSpeechService.TextToSpeech(appSettings, requestInDTO, requestBody));
                response.Success = true;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"RESULT: {JsonConvert.SerializeObject(response)} Execution Time: {watch.ElapsedMilliseconds} ms");
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

        [HttpPost("toneanalyzer")]
        public async Task<IActionResult> ToneAnalyzer([FromBody] JObject request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "ToneAnalyzer";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(request)}");
                ToneAnalyzerRequest requestBody = request.ToObject<ToneAnalyzerRequest>();
                response.Result = await Task.Run(() => ToneAnalyzerService.ToneAnalyzer(appSettings, requestInDTO, requestBody));
                response.Success = true;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"RESULT: {JsonConvert.SerializeObject(response)} Execution Time: {watch.ElapsedMilliseconds} ms");
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

        [HttpPost("toneanalyzercustomer")]
        public async Task<IActionResult> ToneAnalyzerCustomer([FromBody] JObject request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "ToneAnalyzerCustomer";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(request)}");
                ToneAnalyzerRequest requestBody = request.ToObject<ToneAnalyzerRequest>();
                response.Result = await Task.Run(() => ToneAnalyzerService.ToneAnalyzerCustomer(appSettings, requestInDTO, requestBody));
                response.Success = true;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"RESULT: {JsonConvert.SerializeObject(response)} Execution Time: {watch.ElapsedMilliseconds} ms");
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

        [HttpPost("watsonassistant")]
        public async Task<IActionResult> WatsonAssistant([FromBody] JObject request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "WatsonAssistant";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(request)}");
                response.Result = await Task.Run(() => WatsonAssistantService.MessageRequestV1(appSettings, requestInDTO, request));
                response.Success = true;
                watch.Stop();
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