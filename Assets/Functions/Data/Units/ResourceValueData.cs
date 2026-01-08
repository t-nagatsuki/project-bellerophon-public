using System;
using Unity.Mathematics;

namespace Functions.Data.Units
{
    [Serializable]
    public class ResourceValueData
    {
        public string ResourceName;
        public string DisplayName;
        public int Now;
        public int Max;
        public bool IsLimit;

        public ResourceValueData(string _name, string _display, int _value, bool _limit=true)
        {
            ResourceName = _name;
            DisplayName = _display;
            Now = _value;
            Max = _value;
            IsLimit = _limit;
        }

        public void Calc(string _op, string _value)
        { Calc(_op, int.Parse(_value)); }

        public void Calc(string _op, int _value)
        {
            switch (_op)
            {
                case "=":
                    Now = _value;
                    break;
                case "+":
                    Now += _value;
                    break;
                case "-":
                    Now -= _value;
                    break;
                default:
                    Now = (int)math.floor(math.lerp(0, Max, _value / 100.0f));
                    break;
            }
            if (IsLimit)
            { Now = math.clamp(Now, 0, Max); }
            else
            { Max = Now; }
        }

        public string DisplayText
        {
            get
            {
                if (IsLimit) { return $"{Now:N0} / {Max:N0}"; }
                else { return $"{Max:N0}"; }
            }
        }

        public float Percent => (float)Now / Max;
    }
}
