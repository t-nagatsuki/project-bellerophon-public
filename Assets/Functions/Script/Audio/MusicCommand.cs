using System;
using Functions.Attributes;
using Functions.Manager;
namespace Functions.Script.Audio
{
    [Serializable]
    [Command("music")]
    public class MusicCommand : ICommand
    {
        private string name;
        private bool loop;

        public MusicCommand(Json.ProcessJson prc)
        {
            name = prc.name;
            loop = prc.loop;
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SetMusic(name, loop);
            return false;
        }
    }
}

