using System;
using System.Collections.Generic;
using Functions.Enum;
using UnityEngine;

namespace Functions.Data.Units
{
    [Serializable]
    public class ArrangementData
    {
        public DirectionType Direction;
        public int GroupId;
        public ActionType ActionType;
        public MoveType MoveType;
        public string UnitId;
        public string CharacterId;
        public bool IsClone;
        public bool IsTemporaryArrangement;
        public bool IsArrangement;
        public bool IsEndMove;
        public Dictionary<Vector2Int, MoveData> RouteData;
        public Dictionary<Vector2Int, RangeData> RangeData;
        public Vector2Int BeforePosition; 
        public DirectionType BeforeDirection; 
        public Vector2Int TargetPosition; 
        public bool IsEndAction;

        public ArrangementData(int group, string uid, string cid)
        {
            GroupId = group;
            UnitId = uid;
            CharacterId = cid ?? string.Empty;
        }
    }
}