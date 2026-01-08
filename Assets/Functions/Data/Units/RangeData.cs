using System;
using UnityEngine;

namespace Functions.Data.Units
{
    [Serializable]
    public class RangeData
    {
        public Vector2Int Position;
        public bool IsAttack;
        public bool IsTarget;
        public int Cost;
        public RangeData Range;
    }
}
