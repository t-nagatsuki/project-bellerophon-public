using System;
using Functions.Manager;
namespace Functions.Script.Audio
{
    [Serializable]
    public class StopSoundCommand : ICommand
    {
        public StopSoundCommand(Json.ProcessJson prc)
        {
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.StopSound();
            return false;
        }
    }
}

