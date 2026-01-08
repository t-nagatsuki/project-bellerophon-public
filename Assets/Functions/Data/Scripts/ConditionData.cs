using System;
using Functions.Enum;
using UnityEngine;

namespace Functions.Data.Scripts
{
    [Serializable]
    public class ConditionData
    {
        public string EventId;
        public ConditionType ConditionType;
        public ObjectiveType ObjectiveType;
        public int GroupId;
        public string[] Targets;
        public Vector2Int Position;
        public Vector2Int AreaSize;
        public string Name;
        public string Method;
    }
}