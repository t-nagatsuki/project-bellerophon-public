
using System;
using Functions.Manager;
using Functions.Util;

namespace Functions.Script.Unit
{
    [Serializable]
    public class EditUnitCommand : ICommand
    {
        private string unitId;
        private string[] statuses;
        private string op;
        private string[] values;
        private string unitName;
        private bool hidden;

        public EditUnitCommand(Json.ProcessJson prc)
        {
            unitId = prc.unit_id;
            statuses = prc.statuses;
            op = prc.op;
            values = prc.values;
            unitName = prc.unit_name;
            hidden = prc.hidden;
            if (string.IsNullOrWhiteSpace(unitId)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "unit_id"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.EditUnit(
                unitId,
                statuses,
                op,
                values,
                unitName,
                hidden
            );
            return false;
        }
    }
}

