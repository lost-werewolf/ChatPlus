using System;
using ChatPlus.Common.Configs;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;

namespace ChatPlus.Common.Configs.ConfigElements
{
    public class EnumStringOptionElement<TEnum> : RangeElement where TEnum : struct, Enum
    {
        private TEnum[] values;
        private string[] labels;

        public override int NumberTicks => values.Length;

        public override float TickIncrement
        {
            get
            {
                int denom = values.Length - 1;
                if (denom <= 0) denom = 1;
                return 1f / denom;
            }
        }

        protected override float Proportion
        {
            get
            {
                int denom = values.Length - 1;
                if (denom <= 0) denom = 1;
                return (float)GetIndex() / denom;
            }
            set
            {
                int denom = values.Length - 1;
                if (denom <= 0) denom = 1;
                int index = (int)Math.Round(value * denom);
                SetIndex(index);
            }
        }

        public override void OnBind()
        {
            base.OnBind();

            values = (TEnum[])Enum.GetValues(typeof(TEnum));

            TextDisplayFunction = () =>
            {
                string label = Label;
                if (string.IsNullOrEmpty(label)) label = MemberInfo.Name;

                int i = GetIndex();
                if (i < 0) i = 0;
                if (i >= values.Length) i = values.Length - 1;

                return label + "                      " + GetLiveLabel(values[i]);
            };
        }

        private int GetIndex()
        {
            object raw = MemberInfo.GetValue(Item);
            if (raw is TEnum current)
            {
                int idx = Array.IndexOf(values, current);
                if (idx >= 0) return idx;
            }
            return 0;
        }

        private void SetIndex(int index)
        {
            if (index < 0) index = 0;
            if (index >= values.Length) index = values.Length - 1;

            if (MemberInfo.CanWrite)
            {
                MemberInfo.SetValue(Item, values[index]);
                Interface.modConfig.SetPendingChanges();
            }
        }

        private string GetLiveLabel(TEnum value)
        {
            if (typeof(TEnum) == typeof(Config.TimestampSettings))
            {
                var v = (Config.TimestampSettings)(object)value;
                if (v == Config.TimestampSettings.Off) return "Off";

                string fmt = "[" + GetFormat(v) + "]";
                return DateTime.Now.ToString(fmt);
            }

            return value.ToString();
        }

        private static string GetFormat(Config.TimestampSettings value)
        {
            if (value == Config.TimestampSettings.HourAndMinute12Hours)
            {
                return "h:mm tt";
            }
            if (value == Config.TimestampSettings.HourAndMinuteAndSeconds12Hours)
            {
                return "h:mm:ss tt";
            }
            if (value == Config.TimestampSettings.HourAndMinute24Hours)
            {
                return "HH:mm";
            }
            if (value == Config.TimestampSettings.HourAndMinuteAndSeconds24Hours)
            {
                return "HH:mm:ss";
            }
            return "HH:mm";
        }
    }
}
