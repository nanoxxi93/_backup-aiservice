using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Watson.ToneAnalyzer.v3.Model;
using System;
using System.Dynamic;
using System.Threading.Tasks;
using AIService.Entities;
using Serilog;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AIService.Services
{
    public class ToneAnalyzerRequest
    {
        public string Apikey { get; set; }
        public string Endpoint { get; set; }
        public string Text { get; set; }
        public List<Dictionary<string, string>> Utterances { get; set; }
        public bool? Sentences { get; set; }
        public string ContentLanguage { get; set; }
        public string AcceptLanguage { get; set; }
        public long? Conversationid { get; set; }
        public string Ticketnum { get; set; }
    }

    public class ToneAnalyzerService
    {
        private static string label = "Services";
        private static string className = "ToneAnalyzerService";
        public static ToneAnalysis ToneAnalyzer(AppSettings appSettings, RequestInDTO requestInDTO, ToneAnalyzerRequest requestBody)
        {
            string methodName = "ToneAnalyzer";
            dynamic result = new ExpandoObject();
            try
            {
                ToneAnalyzer settings = appSettings.WatsonServices.ToneAnalyzer;
                IamAuthenticator authenticator = new IamAuthenticator(apikey: $"{requestBody.Apikey}");
                IBM.Watson.ToneAnalyzer.v3.ToneAnalyzerService toneAnalyzer = new IBM.Watson.ToneAnalyzer.v3.ToneAnalyzerService($"{settings.Version}", authenticator);
                toneAnalyzer.SetServiceUrl($"{requestBody.Endpoint}");
                ToneInput toneInput = new ToneInput()
                {
                    Text = requestBody.Text
                };
                result = toneAnalyzer.Tone(
                    toneInput: new MemoryStream(Encoding.UTF8.GetBytes(requestBody.Text)),
                    sentences: requestBody.Sentences,
                    contentLanguage: requestBody.ContentLanguage != null ? requestBody.ContentLanguage : "en",
                    acceptLanguage: requestBody.AcceptLanguage
                ).Result;
                return result;
            }
            catch (Exception e)
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"ERROR: {JsonConvert.SerializeObject(requestBody)}");
                throw e;
            }
        }

        public static UtteranceAnalyses ToneAnalyzerCustomer(AppSettings appSettings, RequestInDTO requestInDTO, ToneAnalyzerRequest requestBody)
        {
            string methodName = "ToneAnalyzer";
            dynamic result = new ExpandoObject();
            try
            {
                ToneAnalyzer settings = appSettings.WatsonServices.ToneAnalyzer;
                IamAuthenticator authenticator = new IamAuthenticator(apikey: $"{requestBody.Apikey}");
                IBM.Watson.ToneAnalyzer.v3.ToneAnalyzerService toneAnalyzer = new IBM.Watson.ToneAnalyzer.v3.ToneAnalyzerService($"{settings.Version}", authenticator);
                toneAnalyzer.SetServiceUrl($"{requestBody.Endpoint}");

                List<Utterance> utterances = new List<Utterance>();
                requestBody.Utterances.ForEach(u =>
                {
                    utterances.Add(new Utterance()
                    {
                        User = u["user"].ToString(),
                        Text = u["text"].ToString()
                    });
                });
                result = toneAnalyzer.ToneChat(
                    utterances: utterances,
                    contentLanguage: requestBody.ContentLanguage != null ? requestBody.ContentLanguage : "en",
                    acceptLanguage: requestBody.AcceptLanguage
                ).Result;
                return result;
            }
            catch (Exception e)
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"ERROR: {JsonConvert.SerializeObject(requestBody)}");
                throw e;
            }
        }
    }
}
