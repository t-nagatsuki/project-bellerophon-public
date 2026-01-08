using System;
using System.Collections.Generic;
using System.Linq;
using Functions.Enum;
using UnityEngine.Serialization;

namespace Functions.Data.Units
{
    [Serializable]
    public class SuitableData
    {
        [FormerlySerializedAs("SuitableName")] public string suitableName;
        [FormerlySerializedAs("Suitable")] public Suitable suitable;
        public Dictionary<int, Suitable> Growth = new Dictionary<int, Suitable>();

        public SuitableData(SuitableData dat, int lv)
        {
            suitableName = dat.suitableName;
            suitable = dat.suitable;
            Growth = dat.Growth;
            LevelUp(lv);
        }

        public SuitableData(string name, string value)
        {
            suitableName = name;
            if (String.IsNullOrWhiteSpace(value))
            {
                suitable = Suitable.E;
                return;
            }
            suitable = ConvertSuitable(value);
        }

        public void LevelUp(int lv)
        {
            var up = Growth.Where(v => v.Key <= lv).OrderByDescending(v => v.Key);
            if (up.Any())
            { suitable = up.First().Value; }
        }

        public Suitable ConvertSuitable(string value)
        {
            switch (value.ToUpper())
            {
                case "S":
                    return Suitable.S;
                case "A":
                    return Suitable.A;
                case "B":
                    return Suitable.B;
                case "C":
                    return Suitable.C;
                case "D":
                    return Suitable.D;
                default:
                    return Suitable.E;
            }
        }

        public string DisplayText
        {
            get
            {
                switch (suitable)
                {
                    case Suitable.S:
                        return "S";
                    case Suitable.A:
                        return "A";
                    case Suitable.B:
                        return "B";
                    case Suitable.C:
                        return "C";
                    case Suitable.D:
                        return "D";
                    default:
                        return "－";
                }
            }
        }
    }
}