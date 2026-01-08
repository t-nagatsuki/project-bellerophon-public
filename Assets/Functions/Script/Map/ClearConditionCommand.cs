using System;
using Functions.Enum;
using Functions.Manager;
using Functions.Util;
namespace Functions.Script.Map
{
    [Serializable]
    public class ClearConditionCommand : ICommand
    {
        private ConditionType conditionType;
        
        public ClearConditionCommand(Json.ProcessJson prc)
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
                case "non_event":
                    conditionType = ConditionType.NonEvent;
                    break;
                case "all":
                    conditionType = ConditionType.All;
                    break;
                default:
                    throw new Exception(LocaleUtil.GetMessage("E_S0003", $"condition_type {prc.condition_type}"));
            }
        }

        public bool Process(SlgSceneManager mng)
        {
            switch (conditionType)
            {
                case ConditionType.Victory:
                    mng.VictoryConditionList.Clear();
                    break;
                case ConditionType.Defeat:
                    mng.DefeatConditionList.Clear();
                    break;
                case ConditionType.Event:
                    mng.EventConditionList.Clear();
                    break;
                case ConditionType.NonEvent:
                    mng.VictoryConditionList.Clear();
                    mng.DefeatConditionList.Clear();
                    break;
                case ConditionType.All:
                    mng.VictoryConditionList.Clear();
                    mng.DefeatConditionList.Clear();
                    mng.EventConditionList.Clear();
                    break;
            }
            return true;
        }
    }
}

