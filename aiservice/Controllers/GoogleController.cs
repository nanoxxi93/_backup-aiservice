using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using AIService.Entities;
using Serilog;
using AIService.Services;
using Newtonsoft.Json.Linq;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using aiservice.Services;

namespace AIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleController : ControllerBase
    {
        private static string label = "Controllers";
        private static string className = "GoogleController";
        private readonly AppSettings appSettings;

        public GoogleController(IOptionsSnapshot<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        [HttpPost("naturallanguageunderstanding")]
        public async Task<IActionResult> NaturalLanguageUnderstanding([FromBody] string request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "NaturalLanguageUnderstanding";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(request)}");
                response.Result = await NaturalLanguageUnderstandingService.GoogleNaturalLanguageUnderstanding(appSettings, requestInDTO, request);
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
                response.Result = await LanguageTranslatorService.GoogleLanguageTranslator(appSettings, requestInDTO, requestBody);
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

        [HttpGet("jwt")]
        public async Task<IActionResult> JWT()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "JWT";
            ResponseDTO response = new ResponseDTO();
            try
            {
                string googlejson = "GooglePlayStore.json";
                string googledata = "";
                using (StreamReader r = new StreamReader(googlejson))
                {
                    googledata = await r.ReadToEndAsync();
                }
                JObject googleCreds = JObject.Parse(googledata);
                string iss = googleCreds["client_email"].ToString();
                string aud = googleCreds["token_uri"].ToString();
                string scope = "https://www.googleapis.com/auth/androidpublisher";

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                using (var sr = new StringReader(googleCreds["private_key"].ToString()))
                {
                    PemReader pr = new PemReader(sr);
                    RsaPrivateCrtKeyParameters keyPair = (RsaPrivateCrtKeyParameters)pr.ReadObject();
                    RSAParameters rsaParams = DotNetUtilities.ToRSAParameters(keyPair);
                    rsa.ImportParameters(rsaParams);
                }

                Claim[] claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Iss, iss),
                    new Claim("scope", scope),
                    new Claim(JwtRegisteredClaimNames.Aud, aud),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                    new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.UtcNow.AddMinutes(20)).ToUnixTimeSeconds().ToString()),
                };

                RsaSecurityKey rsakey = new RsaSecurityKey(rsa);
                var creds = new SigningCredentials(rsakey, SecurityAlgorithms.RsaSha256);
                JwtSecurityToken jwt = new JwtSecurityToken(
                    claims: claims,
                    signingCredentials: creds
                );

                return Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
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

        [HttpPost("youtube")]
        public async Task<IActionResult> Youtube([FromBody] JObject request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            RequestInDTO requestInDTO = new RequestInDTO(Request.Host.ToString(), HttpContext.Connection.RemoteIpAddress.ToString(), Request.Path.ToString(), Request.Headers["guid"].ToString());
            string methodName = "Youtube";
            ResponseDTO response = new ResponseDTO();
            try
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(request)}");
                switch (request["operation"].ToString())
                {
                    case "search":
                        response.Result = await YoutubeService.Search(appSettings, requestInDTO, request);
                        break;
                    case "commentThreads":
                        response.Result = await YoutubeService.CommentThreads(appSettings, requestInDTO, request);
                        break;
                    default:
                        break;
                }
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
