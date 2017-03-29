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
        private Portable.ViewModels.MainViewModel VM = new Portable.ViewModels.MainViewModel();

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

            var html = new List<string>()
            {
                "<html>",
                "<head>",
                "<meta http-equiv=\"refresh\" content=\"5\"/>",
                "<meta http-equiv=\"expires\" content=\"-1\"/>",
                "<meta name=\"viewport\" content=\"width=device-width, user-scalable=no\"/>",
                "<script src=\"https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.1.1.min.js\"></script>",
                "<script>",
                "$(document).ready(function(){",
                "$(\"button#start\").click(function(){ $.post(\"status/start\",{},function(data,status){ alert (data); }); });",
                "$(\"button#stop\").click(function(){ $.post(\"status/stop\",{},function(data,status){ alert (data); }); });",
                "});",
                "</script>",
                "<style>",
                "body { margin: 0; padding: 0; margin-left: 10px; background-color: rgb(197, 204, 211); -webkit-text-size-adjust:none; }",
                "h1 { margin:0; margin-left: auto; margin-right: auto; width: 350px; padding-top:10px; padding-right:10px; padding-bottom:10px; padding-left:10px; font-size:30px; font-family: Helvetica; font-weight:bold; color: rgb(76,86,108); }",
                "h1 span.big { float:right; }",
                ".button { display: block; line-height: 46px; width: 350px; font-size: 20px; font-weight: bold; font-family: Helvetica, sans-serif; color: #fff; text-decoration: none; text-align: center; margin: 10px auto; }",
                ".red { background-color: red }",
                ".green { background-color: green }",
                "ul { padding: 0; margin-top:0; margin-left: auto; margin-right: auto; margin-bottom:17px; font-size:17px; font-family: Helvetica; font-weight:bold; color:black; width: 350px; background-color: white; border-width: 1px; border-style:solid ; border-color:rgb(217,217,217); -webkit-border-radius: 8px; }",
                "li { list-style-type: none; border-top-width:1px; border-top-style:solid; border-top-color:rgb(217,217,217); padding:10px; }",
                "</style>",
                "</head>",
                "<body>",
                $"<h1>{VM.CurrentTime.ToString("HH\\:mm\\:ss")} <span class=\"big\">{VM.Controller.FullStatus}</span></h1>",
                "<ul>"
            };

            html.AddRange(VM.Periods.Take(6).Select(p => $"<li>{p.Label}</li>"));
            html.AddRange(new List<string>() {
                "</ul>",
                "<button class=\"green button\" id=\"start\">Start</button>",
                "<button class=\"red button\" id=\"stop\">Stop</button>",
                "</body></html>"
            });
            var content = string.Join("\r\n", html);

            return new HttpResponse(HttpStatusCode.Ok, content);
        }

        [HttpPost]
        [Route("start")]
        public HttpResponse Start([Body] string postContent)
        {
            VM.StartCommand.Execute(this);
            return new HttpResponse(HttpStatusCode.Ok, $"Starting...");
        }

        [HttpPost]
        [Route("stop")]
        public HttpResponse Stop([Body] string postContent)
        {
            VM.StopCommand.Execute(this);
            return new HttpResponse(HttpStatusCode.Ok, $"Stopping...");
        }
    }
}
