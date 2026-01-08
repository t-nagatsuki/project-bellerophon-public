using System;
using Functions.Manager;
using Functions.Util;

namespace Functions.Script.Map
{
    [Serializable]
    public class DelConditionCommand : ICommand
    {
        private string eventId;
        
        public DelConditionCommand(Json.ProcessJson prc)
        {
            eventId = prc.event_id;
            if (String.IsNullOrWhiteSpace(eventId)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "event_id"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.VictoryConditionList.RemoveAll(x => x.EventId == eventId);
            mng.DefeatConditionList.RemoveAll(x => x.EventId == eventId);
            mng.EventConditionList.RemoveAll(x => x.EventId == eventId);
            return true;
        }
    }
}

