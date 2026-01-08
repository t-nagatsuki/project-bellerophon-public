using System;
using Functions.Manager;
namespace Functions.Script.Common
{
    [Serializable]
    public class UnitListCommand : ICommand
    {
        public UnitListCommand(Json.ProcessJson prc)
        {
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SetUnitList();
            return true;
        }
    }
}
