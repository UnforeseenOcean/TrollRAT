using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TrollRAT
{
    public abstract class PayloadSetting
    {
        private static int idCounter = 0;

        protected int id;
        public int ID => id;

        public PayloadSetting()
        {
            id = idCounter++;
        }

        public abstract void writeHTML(StringBuilder builder);
        public abstract void readData(string str);
    }

    public abstract class PayloadSetting<t> : PayloadSetting
    {
        protected t value;
        public t Value => value;

        public PayloadSetting(t defaultValue) : base()
        {
            value = defaultValue;
        }
    }

    public abstract class TitledPayloadSetting<t> : PayloadSetting<t>
    {
        protected string title;
        public string Title => title;

        public TitledPayloadSetting(t defaultValue, string title) : base(defaultValue)
        {
            this.title = title;
        }
    }

    public class PayloadSettingNumber : TitledPayloadSetting<decimal>
    {
        protected decimal min, max, step;
        public decimal Min => min;
        public decimal Max => max;
        public decimal Step => step;

        public PayloadSettingNumber(decimal defaultValue, string title, decimal min, decimal max, decimal step) : base(defaultValue, title)
        {
            this.min = min;
            this.max = max;
            this.step = step;
        }

        public override void writeHTML(StringBuilder builder)
        {
            builder.Append(String.Format("<div class=\"form-group\"><label for=\"id{5}\">{0}</label><input id=\"id{5}\" " + 
                "class=\"form-control\" type=\"number\"min=\"{1}\" max=\"{2}\" step=\"{3}\" value=\"{4}\" " + 
                "oninput=\"setSetting({5}, this.value);\"></input></div>",
                title, min, max, step, value, id));
        }

        public override void readData(string str)
        {
            try
            {
                value = decimal.Parse(str);
            } catch (Exception) { }
        }
    }

    public class PayloadSettingString : TitledPayloadSetting<string>
    {
        protected decimal min, max, step;
        public decimal Min => min;
        public decimal Max => max;
        public decimal Step => step;

        public PayloadSettingString(string defaultValue, string title) : base(defaultValue, title) { }

        public override void writeHTML(StringBuilder builder)
        {
            builder.Append(String.Format("<div class=\"form-group\"><label for=\"id{1}\">{0}</label><input id=\"id{1}\" " +
                "class=\"form-control\" type=\"text\" value=\"{2}\" " +
                "oninput=\"setSetting({1}, this.value);\"></input></div>",
                title, id, value));
        }

        public override void readData(string str)
        {
            value = str;
        }
    }
}
