
using System;
using Functions.Manager;
using Functions.Util;

namespace Functions.Script.Unit
{
    [Serializable]
    public class EditCharacterCommand : ICommand
    {
        private string charaId;
        private string[] statuses;
        private string op;
        private string[] values;
        private string charaName;
        private bool hidden;

        public EditCharacterCommand(Json.ProcessJson prc)
        {
            charaId = prc.chara_id;
            statuses = prc.statuses;
            op = prc.op;
            values = prc.values;
            charaName = prc.chara_name;
            hidden = prc.hidden;
            if (string.IsNullOrWhiteSpace(charaId)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "chara_id"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.EditCharacter(
                charaId,
                statuses,
                op,
                values,
                charaName,
                hidden
            );
            return false;
        }
    }
}

