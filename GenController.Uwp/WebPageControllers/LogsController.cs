using Catnap.Server;
using Commonality;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace GenController.Uwp.Controllers
{
    /// <summary>
    /// Web controller for "logs" page
    /// </summary>
    /// <remarks>
    /// Service Dependencies:
    ///     * ILogger
    /// </remarks>
    [RoutePrefix("logs")]
    class LogsController : Controller
    {
        [HttpGet]
        [Route]
        public async Task<HttpResponse> Get()
        {
            await Logger?.LogEventAsync("Web.Logs");

            var htmlBuilder = new StringBuilder();
            htmlBuilder.AppendLine("<html>");
            htmlBuilder.AppendLine("<head>");
            htmlBuilder.AppendLine("<meta name=\"viewport\" content=\"width=device-width, user-scalable=no\"/>");
            htmlBuilder.AppendLine("<style>");
            htmlBuilder.AppendLine("body { margin: 0; padding: 0; margin-left: 10px; background-color: rgb(197, 204, 211); -webkit-text-size-adjust:none; }");
            htmlBuilder.AppendLine("h1 { margin:0; margin-left: auto; margin-right: auto; width: 350px; padding-top:10px; padding-right:10px; padding-bottom:10px; padding-left:10px; font-size:30px; font-family: Helvetica; font-weight:bold; color: rgb(76,86,108); }");
            htmlBuilder.AppendLine("h1 span.big { float:right; }");
            htmlBuilder.AppendLine(".button { display: block; line-height: 46px; width: 350px; font-size: 20px; font-weight: bold; font-family: Helvetica, sans-serif; color: #fff; text-decoration: none; text-align: center; margin: 10px auto; }");
            htmlBuilder.AppendLine(".red { background-color: red }");
            htmlBuilder.AppendLine(".green { background-color: green }");
            htmlBuilder.AppendLine("ul { padding: 0; margin-top:0; margin-left: auto; margin-right: auto; margin-bottom:17px; font-size:17px; font-family: Helvetica; font-weight:bold; color:black; width: 350px; background-color: white; border-width: 1px; border-style:solid ; border-color:rgb(217,217,217); -webkit-border-radius: 8px; }");
            htmlBuilder.AppendLine("li { list-style-type: none; border-top-width:1px; border-top-style:solid; border-top-color:rgb(217,217,217); padding:10px; }");
            htmlBuilder.AppendLine("</style>");
            htmlBuilder.AppendLine("</head>");
            htmlBuilder.AppendLine("<body>");
            htmlBuilder.AppendLine("<h1>Logs</h1>");
            htmlBuilder.AppendLine("<ul>");
            var logs = FileSystemLogger.GetLogs();
            var sorted = logs.ToList();
            sorted.Sort((x, y) => y.CompareTo(x));

            foreach (var log in sorted)
            {
                htmlBuilder.AppendLine($"<li><a href=\"logs/log/{log.ToBinary().ToString()}\">{log}</a>");
            }

            htmlBuilder.AppendLine("</ul>");
            htmlBuilder.AppendLine("</body>");
            htmlBuilder.AppendLine("</html>");
            htmlBuilder.AppendLine();

            return new HttpResponse(HttpStatusCode.Ok, htmlBuilder.ToString());
        }

        [HttpGet]
        [Route("log/{param1}")]
        public async Task<HttpResponse> WithParam(string param1)
        {
            long binary = Convert.ToInt64(param1, 16);
            DateTime dt = DateTime.FromBinary(binary);

            await Logger?.LogEventAsync("Web.GetLog", $"Log={dt}");

            var htmlBuilder = new StringBuilder();
            htmlBuilder.AppendLine("<html>");
            htmlBuilder.AppendLine("<head>");
            htmlBuilder.AppendLine("</head>");
            htmlBuilder.AppendLine("<body>");
            htmlBuilder.AppendLine($"<h1>{dt}</h1>");

            var lines = await FileSystemLogger.ReadContents(dt);
            foreach(var line in lines)
                htmlBuilder.AppendLine($"<p>{line}</p>");

            htmlBuilder.AppendLine("</body>");
            htmlBuilder.AppendLine("</html>");
            htmlBuilder.AppendLine();

            return new HttpResponse(HttpStatusCode.Ok, htmlBuilder.ToString());
        }

        #region Service Locator services
        private ILogger Logger => Service.TryGet<ILogger>();
        #endregion
    }
}
