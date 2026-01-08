using System;
using Functions.Manager;
namespace Functions.Script.Common
{
    [Serializable]
    public class LoadCommand : ICommand
    {
        public LoadCommand(Json.ProcessJson prc)
        {
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.Load(1);
            return true;
        }
    }
}
