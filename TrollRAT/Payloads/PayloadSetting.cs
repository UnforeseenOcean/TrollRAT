using System;
using System.Collections.Generic;
using System.Text;
using TrollRAT.Utils;

namespace TrollRAT.Payloads
{
    public abstract class PayloadSetting : IDBase<PayloadSetting>
    {
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
                decimal i = decimal.Parse(str);

                if (i <= max && i >= min)
                    value = i;
            }
            catch (Exception) { }
        }
    }

    public class PayloadSettingString : TitledPayloadSetting<string>
    {
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

    public class PayloadSettingSelect : TitledPayloadSetting<int>
    {
        protected string[] options;
        public string[] Options => options;

        public string ValueText => options[value];

        public PayloadSettingSelect(int defaultValue, string title, string[] options) : base(defaultValue, title)
        {
            this.options = options;
        }

        public override void writeHTML(StringBuilder builder)
        {
            builder.Append(String.Format("<div class=\"form-group\"><label for=\"id{1}\">{0}</label><select id=\"id{1}\" " +
                "class=\"form-control\" onchange=\"setSetting({1}, this.selectedIndex);\">",
                title, id, value));

            for (int i = 0; i < options.Length; i++)
            {
                builder.Append((i==value ? "<option selected=\"selected\">" : "<option>") + options[i] + "</option>");
            }

            builder.Append("</select></div>");
        }

        public override void readData(string str)
        {
            try
            {
                int i = int.Parse(str);

                if (i >= 0 && i < options.Length)
                    value = i;
            }
            catch (Exception) { }
        }
    }
}
