using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Cloud.SDK.Core.Http;
using System;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using AIService.Entities;
using Serilog;
using Newtonsoft.Json;

namespace AIService.Services
{
    public class TextToSpeechRequest
    {
        public string Apikey { get; set; }
        public string Endpoint { get; set; }
        public string Text { get; set; }
        public string Accept { get; set; }
        public string Voice { get; set; }
    }

    public class TextToSpeechService
    {
        private static string label = "Services";
        private static string className = "TextToSpeechService";
        public static dynamic TextToSpeech(AppSettings appSettings, RequestInDTO requestInDTO, TextToSpeechRequest requestBody)
        {
            string methodName = "TextToSpeech";
            dynamic result = new ExpandoObject();
            try
            {
                WatsonSettings settings = appSettings.WatsonServices.TextToSpeech;
                IamAuthenticator authenticator = new IamAuthenticator(apikey: $"{requestBody.Apikey}");
                IBM.Watson.TextToSpeech.v1.TextToSpeechService textToSpeech = new IBM.Watson.TextToSpeech.v1.TextToSpeechService(authenticator);
                textToSpeech.SetServiceUrl($"{requestBody.Endpoint}");
                DetailedResponse<MemoryStream> ms = new DetailedResponse<MemoryStream>();
                ms = textToSpeech.Synthesize(
                text: requestBody.Text,
                accept: requestBody.Accept != null ? requestBody.Accept : "audio/mp3",
                voice: requestBody.Voice != null ? requestBody.Voice : "es-ES_LauraV3Voice"
                );
                string filename = CommonService.GetExternalPlatforms(appSettings).STTAudioFilePath + Guid.NewGuid() + ".mp3";
                if (Directory.Exists(Path.Combine(CommonService.GetExternalPlatforms(appSettings).STTAudioFilePath)) == false)
                {
                    Directory.CreateDirectory(CommonService.GetExternalPlatforms(appSettings).STTAudioFilePath + Guid.NewGuid());
                }
                using (FileStream fs = File.Create(filename))
                {
                    ms.Result.WriteTo(fs);
                    fs.Close();
                    ms.Result.Close();
                }
                result = filename.Replace(CommonService.GetExternalPlatforms(appSettings).STTAudioFilePath, CommonService.GetExternalPlatforms(appSettings).STTAudioFileUrl);
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
