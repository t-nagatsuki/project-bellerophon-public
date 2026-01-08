using System;
using UnityEngine.Localization.Settings;
namespace Functions.Util
{
    public static class LocaleUtil
    {
        public static string GetMessage(string cd, params object[] args)
        {
            var entry = LocalizationSettings.StringDatabase.GetTableEntry("Message", cd).Entry;
            if (entry == null) return string.Empty;
            return String.Format(entry.Value, args);
        }

        public static string GetEntry(string key)
        {
            var entry = LocalizationSettings.StringDatabase.GetTableEntry("UIText", key).Entry;
            return entry == null ? string.Empty : entry.Value;
        }
    }
}
