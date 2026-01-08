using System;
using Functions.Enum;

namespace Functions.Data.Maps
{
    [Serializable]
    public class MoveCostData
    {
        public MoveType MoveType { get; set; }
        public float MoveCost { get; set; }

        public MoveCostData(MoveType moveType, float moveCost)
        {
            MoveType = moveType;
            MoveCost = moveCost;
        }
    }
}