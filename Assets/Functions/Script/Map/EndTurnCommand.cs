using System;
using Functions.Manager;
namespace Functions.Script.Map
{
    [Serializable]
    public class EndTurnCommand : ICommand
    {
        public EndTurnCommand(Json.ProcessJson prc)
        {
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.EndTurn();
            return true;
        }
    }
}

