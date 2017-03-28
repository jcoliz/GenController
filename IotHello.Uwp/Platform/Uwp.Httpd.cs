using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace IotHello.Uwp.Platform
{
    public class Httpd
    {
        int port = 8000;

        public async Task StartServer()
        {
            StreamSocketListener listener = new StreamSocketListener();
            await listener.BindServiceNameAsync(port.ToString());
            //listener.Control.KeepAlive = false;
            listener.ConnectionReceived += async (s, e) =>
            {
                try
                {
                    using (e.Socket)
                    {
                        var requests = new List<string>();
                        using (var input = e.Socket.InputStream)
                        {
                            using (var instream = input.AsStreamForRead())
                            {
                                using (var sr = new StreamReader(instream))
                                {
                                    while (!sr.EndOfStream)
                                    {
                                        var request = await sr.ReadLineAsync();
                                        if (string.IsNullOrEmpty(request))
                                            break;
                                        requests.Add(request);
                                    }
                                }
                            }
                        }

                        string content = "Unknown";
                        var page = requests[0]?.Split(' ')?[1];
                        if (page != null)
                        {
                            var indexpage = new WebPages.Index().Navigate(page);
                            if (!string.IsNullOrEmpty(indexpage))
                                content = indexpage;
                        }

                        var now = DateTime.Now.ToString("r");
                        var responses = new List<string>()
                        {
                            "HTTP/1.1 200 OK",
                            $"Date: {now}",
                            $"Last-Modified: {now}",
                            $"Content-Length: {content.Length}",
                            "Content-Type: text/html",
                            "Connection: Closed",
                            "",
                            content
                        };
                        string response = string.Join("\r\n", responses);

                        using (var output = e.Socket.OutputStream)
                        {
                            using (var outstream = output.AsStreamForWrite())
                            {
                                await outstream.WriteAsync(Encoding.ASCII.GetBytes(response), 0, Encoding.ASCII.GetByteCount(response));
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                }
            };
        }
    }
}
