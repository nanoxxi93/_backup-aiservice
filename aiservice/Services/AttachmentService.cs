using aiservice.api;
using AIService.Entities;
using Serilog;
using FluentFTP;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AIService.Services
{
    public class AttachmentService
    {
        private static string label = "Services";
        private static string className = "AttachmentService";
        public static async Task UploadFtp(AppSettings appSettings, RequestInDTO requestInDTO, dynamic watch, string traceIdentifier, Dictionary<string, object> form, IFormFile file, byte[] bytes)
        {
            try
            {
                string methodName = "UploadFtp";
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Information($"REQUEST: {JsonConvert.SerializeObject(form)}");
                Uri ftpuri = new Uri(form["url"].ToString());

                var token = new CancellationToken();
                using (var ftp = new FtpClient(ftpuri, form["username"].ToString(), form["password"].ToString()))
                {
                    await ftp.ConnectAsync(token);
                    bool fileExists = await ftp.FileExistsAsync($"{form["folder"]}/{file.FileName}");

                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    string fileName = fileExists ? $"{fileNameWithoutExtension}_{DateTime.UtcNow.ToString("yyyy-MM-dd_HHmmss")}{extension}" : file.FileName;

                    // define the progress tracking callback
                    Progress<FtpProgress> progress = new Progress<FtpProgress>(p =>
                    {
                        if (p.Progress == 1)
                        {
                            // all done!
                            Startup.Progress[traceIdentifier] = p.Progress;
                            System.Diagnostics.Debug.WriteLine("The operation completed - {0}", p.Progress);
                        }
                        else
                        {
                            // percent done = (p.Progress * 100)
                            Startup.Progress[traceIdentifier] = p.Progress;
                            System.Diagnostics.Debug.WriteLine("Progress - {0}", p.Progress);
                        }
                    });

                    // upload a file with progress tracking
                    var result = await ftp.UploadAsync(bytes,
                        $"{form["folder"]}/{fileName}",
                        FtpRemoteExists.Overwrite,
                        true, progress, token);
                    Startup.Progress.Remove(traceIdentifier);
                    watch.Stop();
                    Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                        .Debug($"RESULT: {JsonConvert.SerializeObject(result)} Execution Time: {watch.ElapsedMilliseconds} ms");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                Startup.Progress.Remove(traceIdentifier);
            }
        }
    }
}
