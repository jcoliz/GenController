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
                "<meta http-equiv=\"refresh\" content=\"10\">",
                "<meta http-equiv=\"expires\" content=\"-1\">",
                "<script src=\"https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.1.1.min.js\"></script>",
                "<script>",
                "$(document).ready(function(){",
                "$(\"button#start\").click(function(){ $.post(\"start\",{},function(data,status){ alert (data); }); });",
                "$(\"button#stop\").click(function(){ $.post(\"stop\",{},function(data,status){ alert (data); }); });",
                "});",
                "</script>",
                "</head>",
                "<body>",
                $"<h1>{VM.CurrentTime.ToString("HH\\:mm\\:ss")}</h1>",
                $"<h2>{VM.Controller.FullStatus}</h2>"
            };

            html.AddRange(VM.Periods.Take(10).Select(p => $"<p>{p.Label}</p>"));
            html.AddRange(new List<string>() { 
                "<button id=\"start\">Start</button><br/>",
                "<button id=\"stop\">Stop</button>",
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
