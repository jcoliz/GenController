using Catnap.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace IotHello.Uwp.Controllers
{
    [RoutePrefix("status")]
    class StatusController : Controller
    {
        private SynchronizationContext Context = SynchronizationContext.Current;

        [HttpGet]
        [Route]
        public HttpResponse Get()
        {
            var htmlBuilder = new StringBuilder();
            htmlBuilder.AppendLine("<html>");
            htmlBuilder.AppendLine("<head>");
            htmlBuilder.AppendLine("<meta http-equiv=\"refresh\" content=\"10\">");
            htmlBuilder.AppendLine("<meta http-equiv=\"expires\" content=\"-1\">");
            htmlBuilder.AppendLine("<script src=\"https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.1.1.min.js\"></script>");
            htmlBuilder.AppendLine("<script>");
            htmlBuilder.AppendLine("$(document).ready(function(){");
            htmlBuilder.AppendLine("$(\"button#start\").click(function(){ $.post(\"status/start\",{},function(data,status){ alert (data); }); });");
            htmlBuilder.AppendLine("$(\"button#stop\").click(function(){ $.post(\"status/stop\",{},function(data,status){ alert (data); }); });");
            htmlBuilder.AppendLine("});");
            htmlBuilder.AppendLine("</script>");
            htmlBuilder.AppendLine("</head>");
            htmlBuilder.AppendLine("<body>");
            htmlBuilder.AppendLine($"<h1>{DateTime.Now}</h1>");
            htmlBuilder.AppendLine("<button id=\"start\">Start</button>");
            htmlBuilder.AppendLine("<button id=\"stop\">Stop</button>");
            htmlBuilder.AppendLine("</body>");
            htmlBuilder.AppendLine("</html>");
            htmlBuilder.AppendLine();

            return new HttpResponse(HttpStatusCode.Ok, htmlBuilder.ToString());
        }

        [HttpPost]
        [Route("start")]
        public HttpResponse Start([Body] string postContent)
        {
            return new HttpResponse(HttpStatusCode.Ok, $"Starting...");
        }

        [HttpPost]
        [Route("stop")]
        public HttpResponse Stop([Body] string postContent)
        {
            return new HttpResponse(HttpStatusCode.Ok, $"Stopping...");
        }
    }
}
