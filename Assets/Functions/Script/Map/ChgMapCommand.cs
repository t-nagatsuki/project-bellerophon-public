using System;
using Functions.Attributes;
using Functions.Manager;
using Functions.Util;
namespace Functions.Script.Map
{
    [Serializable]
    [Command("chg_map")]
    public class ChgMapCommand : ICommand
    {
        private string name;

        public ChgMapCommand(Json.ProcessJson prc)
        {
            name = prc.name;

            if (string.IsNullOrWhiteSpace(name)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "name"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SetMap(name);
            return true;
        }
    }
}

