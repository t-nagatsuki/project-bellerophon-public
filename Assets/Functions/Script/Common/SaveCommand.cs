using System;
using Functions.Manager;
namespace Functions.Script.Common
{
    [Serializable]
    public class SaveCommand : ICommand
    {
        public SaveCommand(Json.ProcessJson prc)
        {
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.Save(1);
            return true;
        }
    }
}
