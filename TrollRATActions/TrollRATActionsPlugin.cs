using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

using TrollRAT;
using TrollRAT.Plugins;
using TrollRAT.Payloads;

namespace TrollRATActions
{
    [Export(typeof(ITrollRATPlugin))]
    public class TrollRATPayloadsPlugin : ITrollRATPlugin
    {
        public string Name => "TrollRAT Actions";

        public void onLoad()
        {
            
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
        [DllImport("Plugins\\TrollRATNative.dll")]
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
