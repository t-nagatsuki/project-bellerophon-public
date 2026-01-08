using System;
using Functions.Manager;
namespace Functions.Script.Map
{
    [Serializable]
    public class ClearArrangementAreaCommand : ICommand
    {
        public ClearArrangementAreaCommand(Json.ProcessJson prc)
        {
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.ClearArrangementArea();
            return true;
        }
    }
}

