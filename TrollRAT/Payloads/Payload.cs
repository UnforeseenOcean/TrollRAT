using System;
using System.Collections.Generic;
using System.Threading;

namespace TrollRAT.Payloads
{
    public abstract class Payload
    {
        protected string name;
        public string Name => name;

        protected List<PayloadSetting> settings = new List<PayloadSetting>();
        public List<PayloadSetting> Settings => settings;

        protected List<PayloadAction> actions = new List<PayloadAction>();
        public List<PayloadAction> Actions => actions;
    }

    public abstract class ExecutablePayload : Payload
    {
        protected abstract void execute();

        public ExecutablePayload()
        {
            actions.Add(new PayloadActionExecute(this));
        }

        public void Execute()
        {
            var thread = new Thread(new ThreadStart(execute));
            thread.Start();
        }
    }

    public abstract class LoopingPayload : ExecutablePayload
    {
        protected bool running = false;
        public bool Running => running;

        private PayloadSettingNumber delay;
        public decimal Delay => delay.Value;

        protected int i;

        public LoopingPayload(int defaultDelay)
        {
            delay = new PayloadSettingNumber(defaultDelay, "Delay (in 1/100 seconds)", 1, 10000, 1);

            settings.Add(delay);
            actions.Add(new PayloadActionStartStop(this));

            var thread = new Thread(new ThreadStart(Loop));
            thread.Start();
        }

        public LoopingPayload() : this(100) { }

        public void Start()
        {
            running = true;
            i = 0;
        }

        public void Stop()
        {
            running = false;
        }

        private void Loop()
        {
            while (true)
            {
                if (running)
                {
                    execute();
                }

                for (i = (int)Delay; i >= 0; i--)
                    Thread.Sleep(10);
            }
        }
    }
}
