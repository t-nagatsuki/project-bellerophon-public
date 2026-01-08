using System;
using Functions.Manager;
namespace Functions.Script.Common
{
    [Serializable]
    public class CharaListCommand : ICommand
    {
        public CharaListCommand(Json.ProcessJson prc)
        {
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SetCharaList();
            return true;
        }
    }
}
