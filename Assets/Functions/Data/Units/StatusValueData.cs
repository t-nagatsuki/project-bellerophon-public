using System;
using Unity.Mathematics;
using UnityEngine.Serialization;

namespace Functions.Data.Units
{
    [Serializable]
    public class StatusValueData
    {
        [FormerlySerializedAs("StatusName")] public string statusName;
        [FormerlySerializedAs("DisplayName")] public string displayName;
        [FormerlySerializedAs("Status")] public int status;

        public StatusValueData(string name, string display, int value)
        {
            statusName = name;
            displayName = display;
            status = value;
        }

        public void Calc(string op, string value)
        {
            switch (op)
            {
                case "=":
                    status = int.Parse(value);
                    break;
                case "+":
                    status += int.Parse(value);
                    break;
                case "-":
                    status -= int.Parse(value);
                    break;
                default:
                    status = (int)math.floor(math.lerp(0, status, float.Parse(value)));
                    break;
            }
            status = math.clamp(status, 0, status);
        }

        public string DisplayText => $"{status:N0}";
    }
}
