using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AIService.Entities;
using System.Collections.Generic;
using Serilog;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using aiservice.api;
using AIService.Services;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private static string label = "Controllers";
        private static string className = "ValuesController";
        private readonly AppSettings appSettings;

        public ValuesController(IOptionsSnapshot<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            watch.Stop();
            return Ok();
        }

        // GET api/values
        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            watch.Stop();
            return Ok(new { result = $"API v2.5.25 .Net Core 3.1 is working Execution Time: {watch.ElapsedMilliseconds} ms" });
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "Post";
            try
            {
                var form = new Dictionary<string, object>();
                foreach (var key in Request.Form.Keys)
                {
                    var value = Request.Form[key][0];
                    form.Add(key, value);
                }
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Debug($"RESULT: {JsonConvert.SerializeObject(form)} Execution Time: {watch.ElapsedMilliseconds} ms");
                watch.Stop();
            }
            catch (Exception)
            {

            }
            return Ok();
        }

        [HttpPost("uploadftp")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFTP()
        {
            var formdata = await Request.ReadFormAsync();
            var form = new Dictionary<string, object>();
            foreach (var key in formdata.Keys)
            {
                var value = Request.Form[key][0];
                form.Add(key, value);
            }
            IFormFile file = formdata.Files[0];
            using var fileStream = file.OpenReadStream();
            byte[] bytes = new byte[file.Length];
            fileStream.Read(bytes, 0, (int)file.Length);


            Stream stream = file.OpenReadStream();
            string traceIdentifier = HttpContext.TraceIdentifier;
            Startup.Progress.Add(traceIdentifier, 0);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "UploadFTP";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Task[] tasks = new[]
                {
                    Task.Run(() => AttachmentService.UploadFtp(appSettings, requestInDTO, watch, traceIdentifier, form, file, bytes))
                };

                return Ok($"Request submitted successfully: {traceIdentifier}");
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Msg = e.Message;
                watch.Stop();
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"ERROR: {JsonConvert.SerializeObject(form)}");
                return BadRequest(response);
            }
        }

        [HttpGet("uploadftp/progress/{traceIdentifier}")]
        public ActionResult Progress(string traceIdentifier)
        {
            if (Startup.Progress.ContainsKey(traceIdentifier))
            {
                return Ok(Startup.Progress[traceIdentifier].ToString());
            }
            else
            {
                return Ok();
            }
        }

        [HttpPost("zipfile")]
        public async Task<ActionResult> ZipFile()
        {
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            var formdata = await Request.ReadFormAsync();
            var form = new Dictionary<string, object>();
            foreach (var key in formdata.Keys)
            {
                var value = Request.Form[key][0];
                form.Add(key, value);
            }
            
            List<Dictionary<string, object>> data = CommonService.JArrayToListDictionary(JArray.Parse(form["data"].ToString()));
            DataTable table = new DataTable();
            var columnNames = data.SelectMany(dict => dict.Keys).Distinct();
            table.Columns.AddRange(columnNames.Select(c => new DataColumn(c)).ToArray());
            data.ForEach(x =>
            {
                var row = table.NewRow();
                foreach (var k in x.Keys)
                {
                    row[k] = x[k];
                }
                table.Rows.Add(row);
            });
            string worksheetname = form["worksheetname"]?.ToString() ?? "Hoja1";
            byte[] b1 = CommonService.CreateExcel(table, worksheetname);
            string workbookname = form["workbookname"]?.ToString() ?? "Libro1.xlsx";
            byte[] b2 = CommonService.ZipData(b1, workbookname);
            var result = await ObjectStorageService.UploadBytes(appSettings, requestInDTO, form, b2);
            return Ok(result);
        }

        [HttpGet("linkedin/callback")]
        public ActionResult Linkedin()
        {
            return Ok();
        }
    }
}
