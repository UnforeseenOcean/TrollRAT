using System;
using System.Runtime.InteropServices;

using TrollRAT.Payloads;
using TrollRATActions;

namespace TrollRATPayloads.Payloads
{
    public class PayloadEarthquake : LoopingPayload
    {
        [DllImport("Plugins\\TrollRATNative.dll")]
        public static extern void payloadEarthquake(int delay, int power);

        private PayloadSettingNumber power = new PayloadSettingNumber(20, "Movement Factor", 2, 60, 1);

        public PayloadEarthquake() : base(4)
        {
            actions.Add(new PayloadActionClearScreen());
            settings.Add(power);

            name = "Earthquake (Shake Screen)";
        }

        protected override void execute()
        {
            payloadEarthquake((int)Delay, (int)power.Value);
        }
    }

    public class PayloadMeltingScreen : LoopingPayload
    {
        [DllImport("Plugins\\TrollRATNative.dll")]
        public static extern void payloadMeltingScreen(int size, int power);

        private PayloadSettingNumber size = new PayloadSettingNumber(30, "Bar Size", 4, 200, 1);
        private PayloadSettingNumber power = new PayloadSettingNumber(10, "Power", 1, 40, 1);

        public PayloadMeltingScreen() : base(4)
        {
            actions.Add(new PayloadActionClearScreen());
            settings.Add(size);
            settings.Add(power);

            name = "Melting Screen";
        }

        protected override void execute()
        {
            payloadMeltingScreen((int)size.Value, (int)power.Value);
        }
    }

    public class PayloadTrain : LoopingPayload
    {
        [DllImport("Plugins\\TrollRATNative.dll")]
        public static extern void payloadTrain(int xPower, int yPower);

        private PayloadSettingNumber xPower = new PayloadSettingNumber(10, "X Movement", -100, 100, 1);
        private PayloadSettingNumber yPower = new PayloadSettingNumber(0, "Y Movement", -100, 100, 1);

        public PayloadTrain() : base(5)
        {
            actions.Add(new PayloadActionClearScreen());
            settings.Add(xPower);
            settings.Add(yPower);

            name = "Train/Elevator Effect";
        }

        protected override void execute()
        {
            payloadTrain((int)xPower.Value, (int)yPower.Value);
        }
    }

    public class PayloadDrawPixels : LoopingPayload
    {
        [DllImport("Plugins\\TrollRATNative.dll")]
        public static extern void payloadDrawPixels(uint color, int power);

        private PayloadSettingNumber power = new PayloadSettingNumber(500, "Changed Pixels per Iteration", 1, 10000, 1);
        protected PayloadSettingSelect color = new PayloadSettingSelect(0, "Color",
            new string[] { "Black", "White", "Red", "Green", "Blue", "Random (Black/White)", "Random (RGB)" });

        private static readonly uint[] colors = new uint[] { 0x000000, 0xFFFFFF, 0x0000FF, 0x00FF00, 0xFF0000 };

        private Random rng = new Random();

        public PayloadDrawPixels() : base(1)
        {
            actions.Add(new PayloadActionClearScreen());

            settings.Add(power);
            settings.Add(color);

            name = "Draw Pixels on Screen";
        }

        protected override void execute()
        {
            uint c;

            if (color.Value == colors.Length)
                c = rng.NextDouble() > 0.5 ? colors[0] : colors[1];
            else if (color.Value == colors.Length + 1)
                c = (uint)rng.Next();
            else
                c = colors[color.Value];

            payloadDrawPixels(c, (int)power.Value);
        }
    }
}
