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
        public PayloadActionClearScreen(Payload payload) : base(payload) { }

        [DllImport("user32.dll")]
        static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, int flags);

        public override string execute()
        {
            RedrawWindow(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 133);
            return "void(0);";
        }

        public override string Icon => null;
        public override string Title => "Clear Screen";
    }

    public class PayloadActionClearWindows : SimplePayloadAction
    {
        public PayloadActionClearWindows(Payload payload) : base(payload) { }

        [DllImport("Plugins\\TrollRATNative.dll")]
        static extern void clearWindows();

        public override string execute()
        {
            clearWindows();
            return "void(0);";
        }

        public override string Icon => null;
        public override string Title => "Close open Windows";
    }

}
