using System;
using Functions.Manager;
namespace Functions.Script.Map
{
    [Serializable]
    public class ClearHighlightAreaCommand : ICommand
    {
        public ClearHighlightAreaCommand(Json.ProcessJson prc)
        {
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.ClearHighlightArea();
            return true;
        }
    }
}

