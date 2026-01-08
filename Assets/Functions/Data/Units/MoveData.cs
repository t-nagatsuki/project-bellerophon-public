using System;
using System.Collections.Generic;
using UnityEngine;

namespace Functions.Data.Units
{
    [Serializable]
    public class MoveData
    {
        public Vector2Int Position;
        public bool IsStop;
        public float Cost;
        public List<Vector2Int> Route;
    }
}
