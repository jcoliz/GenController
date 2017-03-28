using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHello.Uwp.WebPages
{
    public class Index
    {
        private Portable.ViewModels.MainViewModel VM = new Portable.ViewModels.MainViewModel();

        private Dictionary<string, Func<string>> Routes = new Dictionary<string, Func<string>>();

        public Index()
        {
            Routes["/index"] = Page;
            Routes["/stop"] = Stop;
            Routes["/start"] = Start;
        }

        public string Navigate(string address)
        {
            if (Routes.ContainsKey(address))
                return Routes[address].Invoke();
            else
                return string.Empty;
        }

        private string Page()
        {
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
                "$(\"button#start\").click(function(){ $.post(\"start\",{},function(data,status){ alert (data); }); });",
                "$(\"button#stop\").click(function(){ $.post(\"stop\",{},function(data,status){ alert (data); }); });",
                "});",
                "</script>",
                "<style>",
                "body { margin: 0; padding: 0; margin-left: 10px; background-color: rgb(197, 204, 211); -webkit-text-size-adjust:none; }",
                "h1 { margin:0; margin-left: auto; margin-right: auto; width: 350px; padding-top:10px; padding-right:10px; padding-bottom:10px; padding-left:10px; font-size:30px; font-family: Helvetica; font-weight:bold; color: rgb(76,86,108); }",
                "h1 span.big { float:right; }",
                ".button { display: block; line-height: 46px; width: 350px; font-size: 20px; font-weight: bold; font-family: Helvetica, sans-serif; color: #fff; text-decoration: none; text-align: center; margin: 10px auto; }",
                ".red { background-color: red }",
                ".green { background-color: green }",
                "</style>",
                "</head>",
                "<body>",
                $"<h1>{VM.CurrentTime.ToString("HH\\:mm\\:ss")} <span class=\"big\">{VM.Controller.FullStatus}</span></h1>",
            };

            html.AddRange(VM.Periods.Take(10).Select(p => $"<p>{p.Label}</p>"));
            html.AddRange(new List<string>() { 
                "<button class=\"green button\" id=\"start\">Start</button>",
                "<button class=\"red button\" id=\"stop\">Stop</button>",
                "</body></html>"
            });
            return string.Join("\r\n", html);
        }

        private string Start()
        {
            VM.StartCommand.Execute(this);
            return "Starting...";
        }
        private string Stop()
        {
            VM.StopCommand.Execute(this);
            return "Stopping...";
        }

    }
}
