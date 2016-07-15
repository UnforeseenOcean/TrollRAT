using System;
using System.Runtime.InteropServices;

namespace TrollRAT
{
    public class PayloadEarthquake : LoopingPayload
    {
        [DllImport("TrollRATNative.dll")]
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
}
