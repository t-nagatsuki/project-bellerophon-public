using System;
using Functions.Manager;
namespace Functions.Script.Unit
{
    [Serializable]
    public class ClearUnitCommand : ICommand
    {
        private bool permanent;

        public ClearUnitCommand(Json.ProcessJson prc)
        {
            permanent = prc.permanent;
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.ClearUnit(permanent);
            return true;
        }
    }
}

