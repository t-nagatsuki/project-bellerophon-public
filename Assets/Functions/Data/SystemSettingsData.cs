using System;
using System.Collections.Generic;
namespace Functions.Data
{
    [Serializable]
    public class SystemSettingsData
    {
        public int SelectLocale;
        public int WindowMode;
        public int WindowWidth;
        public int WindowHeight;
        public float BgmVolume;
        public float SoundVolume;

        public SystemSettingsData(int selectLocale, int windowMode, int windowWidth, int windowHeight, float bgmVolume, float soundVolume)
        {
            this.SelectLocale = selectLocale;
            this.WindowMode = windowMode;
            this.WindowWidth = windowWidth;
            this.WindowHeight = windowHeight;
            this.BgmVolume = bgmVolume;
            this.SoundVolume = soundVolume;
        }
    }
}
