using System;
using System.IO;
using System.Threading.Tasks;
using Functions.Data;
using Functions.Util;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Functions.Manager
{
    public class StartupManager : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static async void Initialize()
        {
            try
            {
                var pathBase = DataUtil.PathBase;
                _ = DataUtil.LoadSystemSettings(pathBase);
                if (DataUtil.SystemSettingsData == null)
                {
                    DataUtil.SystemSettingsData = new SystemSettingsData(
                        LocalizationSettings.AvailableLocales.Locales.FindIndex(x => x.Identifier.Code == "ja"),
                        1,
                        800,
                        600,
                        50,
                        50
                    );
                    DataUtil.SaveData(Path.Combine(DataUtil.PathBase, "system.json"), DataUtil.SystemSettingsData);
                }
                _ = DataUtil.LoadLocales(pathBase);
                var locale = LocalizationSettings.AvailableLocales.Locales[DataUtil.SystemSettingsData.SelectLocale];
                LocalizationSettings.SelectedLocale = locale;
                await LocalizationSettings.InitializationOperation.Task;
                switch (DataUtil.SystemSettingsData.WindowMode)
                {
                    case 0:
                        Screen.SetResolution(DataUtil.SystemSettingsData.WindowWidth, DataUtil.SystemSettingsData.WindowHeight, true);
                        break;
                    case 1:
                        Screen.SetResolution(DataUtil.SystemSettingsData.WindowWidth, DataUtil.SystemSettingsData.WindowHeight, false);
                        break;
                }
            }
            catch (Exception e)
            {
                DataUtil.ErrorMessage = e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace;
                Debug.LogException(e);
            }
        }
    }
}