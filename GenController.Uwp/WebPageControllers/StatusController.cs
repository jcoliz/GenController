using Catnap.Server;
using Commonality;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Windows.Web.Http;

namespace GenController.Uwp.Controllers
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
            Service.Get<ILogger>().LogEvent("Web.Status");

            var html = new List<string>()
            {
                "<html>",
                "<head>",
                //"<meta http-equiv=\"refresh\" content=\"5\"/>",
                "<meta http-equiv=\"expires\" content=\"-1\"/>",
                "<meta name=\"viewport\" content=\"width=device-width, user-scalable=no\"/>",
                "<script src=\"https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.1.1.min.js\"></script>",
                "<script>",
                "$(document).ready(function(){",
                "$(\"button#start\").click(function(){ $.post(\"status/start\",{},function(data,status){ alert (data); }); });",
                "$(\"button#stop\").click(function(){ $.post(\"status/stop\",{},function(data,status){ alert (data); }); });",
                "$(\"button#disable\").click(function(){ $.post(\"status/disable\",{},function(data,status){ alert (data); }); });",
                "});",
                "</script>",
                "<style>",
                "body { margin: 0; padding: 0; margin-left: 10px; background-color: rgb(197, 204, 211); -webkit-text-size-adjust:none; }",
                "h1 { margin:0; margin-left: auto; margin-right: auto; width: 350px; padding-top:10px; padding-right:10px; padding-bottom:10px; padding-left:10px; font-size:30px; font-family: Helvetica; font-weight:bold; color: rgb(76,86,108); }",
                "h1 span.big { float:right; }",
                ".button { display: block; line-height: 46px; width: 350px; font-size: 20px; font-weight: bold; font-family: Helvetica, sans-serif; color: #fff; text-decoration: none; text-align: center; margin: 10px auto; }",
                ".red { background-color: red }",
                ".green { background-color: green }",
                ".gray { background-color: gray }",
                "ul { padding: 0; margin-top:0; margin-left: auto; margin-right: auto; margin-bottom:17px; font-size:17px; font-family: Helvetica; font-weight:bold; color:black; width: 350px; background-color: white; border-width: 1px; border-style:solid ; border-color:rgb(217,217,217); -webkit-border-radius: 8px; }",
                "li { list-style-type: none; border-top-width:1px; border-top-style:solid; border-top-color:rgb(217,217,217); padding:10px; }",
                "</style>",
                "</head>",
                "<body>",
                $"<h1>{VM.CurrentTime.ToString("HH\\:mm\\:ss")} <span class=\"big\">{VM.Controller.Status}</span></h1>",
                "<ul>"
            };

            html.AddRange(VM.Periods.Take(6).Select(p => $"<li>{p.Label}</li>"));
            html.AddRange(new List<string>() {
                "</ul>",
                "<button class=\"green button\" id=\"start\">Start</button>",
                "<button class=\"red button\" id=\"stop\">Stop</button>"
            });

            if (VM.Controller.Enabled)
                html.Add("<button class=\"gray button\" id=\"disable\">Disable</button>");
            else
                html.Add("<button class=\"gray button\" id=\"enable\">Enable</button>");

            html.AddRange(new List<string>() {
                $"<ul><li>{App.Current.Title} {App.Current.Version}</li>",
                $"<li>E: {(VM.Controller.Enabled?'Y':'N')} 1:{(VM.Controller.StartLine?'Y':'N')} 0:{(VM.Controller.StopLine?'Y':'N')} R:{(VM.Controller.RunSignal?'Y':'N')} P:{(VM.Controller.PanelLightSignal?'Y':'N')} </li>",
                "<li><a href=\"/logs\">View Logs</a></li></ul>",
                "</body></html>"
            });
            var content = string.Join("\r\n", html);

            Service.Get<ILogger>().LogEvent("Web.StatusOK",$"Status={VM.Controller.Status}");

            return new HttpResponse(HttpStatusCode.Ok, content);
        }

        [HttpPost]
        [Route("start")]
        public HttpResponse Start([Body] string postContent)
        {
            Service.Get<ILogger>().LogEvent("Web.Start");
            VM.StartCommand.Execute(this);
            Service.Get<ILogger>().LogEvent("Web.StartOK");
            return new HttpResponse(HttpStatusCode.Ok, $"Starting...");
        }

        [HttpPost]
        [Route("stop")]
        public HttpResponse Stop([Body] string postContent)
        {
            Service.Get<ILogger>().LogEvent("Web.Stop");
            VM.StopCommand.Execute(this);
            Service.Get<ILogger>().LogEvent("Web.StopOK");
            return new HttpResponse(HttpStatusCode.Ok, $"Stopping...");
        }
        [HttpPost]
        [Route("disable")]
        public HttpResponse Disable([Body] string postContent)
        {
            Service.Get<ILogger>().LogEvent("Web.Disable");
            VM.DisableCommand.Execute(this);
            Service.Get<ILogger>().LogEvent("Web.DisableOK");
            return new HttpResponse(HttpStatusCode.Ok, $"Disabled.");
        }
        [HttpPost]
        [Route("enable")]
        public HttpResponse Enable([Body] string postContent)
        {
            Service.Get<ILogger>().LogEvent("Web.Enable");
            VM.EnableCommand.Execute(this);
            Service.Get<ILogger>().LogEvent("Web.EnableOK");
            return new HttpResponse(HttpStatusCode.Ok, $"Enabled.");
        }
    }
}
