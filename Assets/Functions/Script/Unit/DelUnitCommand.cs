using System;
using Functions.Manager;
using Functions.Util;

namespace Functions.Script.Unit
{
    [Serializable]
    public class DelUnitCommand : ICommand
    {
        private string unitId;
        private bool animation;
        private bool permanent;

        public DelUnitCommand(Json.ProcessJson prc)
        {
            unitId = prc.unit_id;
            animation = prc.animation;
            permanent = prc.permanent;
            if (string.IsNullOrWhiteSpace(unitId)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "unit_id"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.DeleteUnit(unitId, animation, permanent);
            return animation;
        }
    }
}

