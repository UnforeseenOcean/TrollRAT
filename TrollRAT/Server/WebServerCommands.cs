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

    public class PluginsCommand : WebServerCommandBase
    {
        public override Regex Path => new Regex("^/plugins$");

        public override void execute(HttpListenerContext context)
        {
            StringBuilder plugins = new StringBuilder();
            foreach (ITrollRATPlugin plugin in TrollRAT.pluginManager.plugins)
            {
                plugins.Append(plugin.Name + ", ");
            }
            respondString(plugins.ToString(0, plugins.Length - 2), context.Response, "text/html");
        }
    }
}
