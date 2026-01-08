using System;
using Functions.Manager;
namespace Functions.Script.Audio
{
    [Serializable]
    public class StopMusicCommand : ICommand
    {
        public StopMusicCommand(Json.ProcessJson prc)
        {
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.StopMusic();
            return false;
        }
    }
}

