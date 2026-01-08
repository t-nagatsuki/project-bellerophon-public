using System;
using Functions.Manager;
using Functions.Util;

namespace Functions.Script.Unit
{
    [Serializable]
    public class DelGroupCommand : ICommand
    {
        private int groupId;

        public DelGroupCommand(Json.ProcessJson prc)
        {
            groupId = prc.group_id;
            if (groupId == 0) throw new Exception(LocaleUtil.GetMessage("E_S0007", "group_id", "0"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.DeleteGroup(groupId);
            return true;
        }
    }
}

