using System;
using System.Collections.Generic;
using Functions.Data;
using Functions.Data.Scripts;
using Functions.Enum;
using Functions.Manager;
using Functions.Util;
using UnityEngine;

namespace Functions.Script.Map
{
    [Serializable]
    public class AddConditionCommand : ICommand
    {
        private ConditionType conditionType;
        private ObjectiveType objectiveType;
        private string eventId;
        private int groupId;
        private string[] targets;
        private Vector2Int pos;
        private Vector2Int areaSize;
        private string name;
        private string method;

        public AddConditionCommand(Json.ProcessJson prc)
        {
            switch (prc.condition_type.ToLower())
            {
                case "victory":
                    conditionType = ConditionType.Victory;
                    break;
                case "defeat":
                    conditionType = ConditionType.Defeat;
                    break;
                case "event":
                    conditionType = ConditionType.Event;
                    break;
                default:
                    throw new Exception(LocaleUtil.GetMessage("E_S0003", $"condition_type {prc.condition_type}"));
            }

            switch (prc.objective_type.ToLower())
            {
                case "reach":
                    objectiveType = ObjectiveType.Reach;
                    break;
                case "reach_all":
                    objectiveType = ObjectiveType.ReachAll;
                    break;
                case "destroy":
                    objectiveType = ObjectiveType.Destroy;
                    break;
                case "destroy_all":
                    objectiveType = ObjectiveType.DestroyAll;
                    break;
                default:
                    throw new Exception(LocaleUtil.GetMessage("E_S0003", $"objective_type {prc.condition_type}"));
            }

            eventId = prc.event_id;
            groupId = prc.group_id;
            targets = prc.values;
            pos = new Vector2Int(prc.pos.x, prc.pos.y);
            areaSize = prc.area_size.x + prc.area_size.y > 1 ? new Vector2Int(prc.area_size.x, prc.area_size.y) : Vector2Int.one;
            name = prc.name;
            method = prc.method;

            if (string.IsNullOrWhiteSpace(eventId)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "event_id"));

            if (objectiveType is ObjectiveType.Reach or ObjectiveType.ReachAll)
            {
                if (pos.x < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.x"));
                if (pos.y < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.y"));
                if (areaSize.x < 1) throw new Exception(LocaleUtil.GetMessage("E_S0004", "area_size.x", "1"));
                if (areaSize.y < 1) throw new Exception(LocaleUtil.GetMessage("E_S0004", "area_size.y", "1"));
            }
            if (string.IsNullOrWhiteSpace(method)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "method"));
        }

        public bool Process(SlgSceneManager mng)
        {
            switch (conditionType)
            {
                case ConditionType.Victory:
                    SetCondition(mng.VictoryConditionList);
                    break;
                case ConditionType.Defeat:
                    SetCondition(mng.DefeatConditionList);
                    break;
            }
            return true;
        }

        private void SetCondition(List<ConditionData> lst)
        {
            lst.Add(new ConditionData()
            {
                EventId = eventId,
                ConditionType = conditionType,
                ObjectiveType = objectiveType,
                GroupId = groupId,
                Targets = targets,
                Position = pos,
                AreaSize = areaSize,
                Name = name,
                Method = method
            });
        }
    }
}

