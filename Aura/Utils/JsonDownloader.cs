using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Aura.Utils
{
    public class DownloadProgressEventArgs : EventArgs
    {
        public int ProgressPercentage { get; }

        public DownloadProgressEventArgs(int percentage)
        {
            ProgressPercentage = percentage;
        }
    }

    public class JsonDownloader
    {
        public HttpClient Client { get; private set; }

        public event EventHandler<DownloadProgressEventArgs> DownloadProgressChanged;

        public JsonDownloader()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("User-Agent", Assembly.GetExecutingAssembly().FullName ?? "Aura");
        }

        public async Task<T> GetObject<T>(string url)
        {
            string json = await Client.GetStringAsync(url);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<string> GetTempFile(string url, string filename)
        {
            string extension = Path.GetExtension(filename);
            string path = Path.ChangeExtension(Path.GetTempFileName(), extension);

            using (var response = await Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                long? totalBytes = response.Content.Headers.ContentLength;

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    var buffer = new byte[8192];
                    long totalRead = 0;
                    int read;
                    int lastPercentage = -1;

                    while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, read);
                        totalRead += read;

                        if (totalBytes.HasValue)
                        {
                            int percentage = (int)((double)totalRead / totalBytes.Value * 100.0);
                            if (percentage != lastPercentage)
                            {
                                lastPercentage = percentage;
                                DownloadProgressChanged?.Invoke(this, new DownloadProgressEventArgs(percentage));
                            }
                        }
                    }
                }
            }

            return path;
        }
    }
}

