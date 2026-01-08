using System;
using Functions.Manager;
namespace Functions.Script.Common
{
    [Serializable]
    public class ConditionListCommand : ICommand
    {
        public ConditionListCommand(Json.ProcessJson prc)
        {
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SlgWindowManager.SetCondition(mng);
            return true;
        }
    }
}
