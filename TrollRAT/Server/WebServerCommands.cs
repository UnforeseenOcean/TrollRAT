using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using TrollRAT.Payloads;
using TrollRAT.Plugins;

namespace TrollRAT.Server
{
    public class RootCommand : WebServerCommandBase
    {
        public override Regex Path => new Regex("^/?(index(\\.html|\\.php)?)?$");

        private byte[] site;

        public RootCommand()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("TrollRAT.client.html"))
            {
                site = new byte[stream.Length];
                stream.Read(site, 0, (int)stream.Length);
            }
        }

        public override void execute(HttpListenerContext context)
        {
            respondBytes(site, context.Response, "text/html");
        }
    }

    public class PayloadsCommand : WebServerCommand
    {
        public PayloadsCommand(List<Payload> payloads) : base(payloads) { }
        public override Regex Path => new Regex("^/payloads$");

        public override void execute(HttpListenerContext context)
        {
            StringBuilder content = new StringBuilder();
            foreach (Payload payload in payloads)
            {
                content.Append("<a href=\"#\" onclick=\"onPayloadSelected(this);\" class=\"list-group-item clearfix\">");
                content.Append(payload.Name);
                content.Append("<span class=\"pull-right\">");

                foreach (PayloadAction action in payload.Actions)
                {
                    string btn = action.getListButton(payload);
                    if (btn != null)
                    {
                        content.Append(btn);
                    }
                }

                content.Append("</span></a>");
            }

            string response = content.ToString();
            if (response.Length < 1)
            {
                response = "<p>Nothing defined.</p>";
            }

            respondString(response, context.Response, "text/html");
        }
    }

    public class AboutCommand : WebServerCommandBase
    {
        public override Regex Path => new Regex("^/about$");

        public override void execute(HttpListenerContext context)
        {
            StringBuilder about = new StringBuilder();

            Version version = Assembly.GetCallingAssembly().GetName().Version;
            about.Append(String.Format("<p><strong>TrollRAT {0}.{1}</strong> - Remote Trolling Software by Leurak</p>", version.Major, version.Minor));
            about.Append("<p><strong>Source Code</strong> is <a href=\"http://github.com/Leurak/TrollRAT\">on GitHub</a>, Licensed under MIT License</p>");

            about.Append("<p><strong>Loaded Plugins: </strong>");
            foreach (ITrollRATPlugin plugin in TrollRAT.pluginManager.plugins)
            {
                about.Append(plugin.Name + ", ");
            }

            if (TrollRAT.pluginManager.plugins.Length == 0)
                about.Append("None");
            else
                about.Remove(about.Length - 2, 2);

            about.Append("</p>");

            respondString(about.ToString(), context.Response, "text/html");
        }
    }
}
