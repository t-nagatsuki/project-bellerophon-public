using System;
using Functions.Manager;
namespace Functions.Script.Unit
{
    [Serializable]
    public class ClearGroupCommand : ICommand
    {
        public ClearGroupCommand(Json.ProcessJson prc)
        {
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.ClearGroup();
            return true;
        }
    }
}

