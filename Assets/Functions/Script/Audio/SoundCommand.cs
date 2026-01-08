using System;
using Functions.Manager;
using Functions.Util;

namespace Functions.Script.Audio
{
    [Serializable]
    public class SoundCommand : ICommand
    {
        private string name;

        public SoundCommand(Json.ProcessJson prc)
        {
            name = prc.name;
            if (String.IsNullOrWhiteSpace(name)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "name"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SetSound(name);
            return false;
        }
    }
}

