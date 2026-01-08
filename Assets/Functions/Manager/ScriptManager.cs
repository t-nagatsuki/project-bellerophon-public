using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using Functions.Data.Scripts;
using Functions.Util;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Functions.Manager
{
    public class ScriptManager : MonoBehaviour
    {
        /// <summary>スクリプトデータ</summary>
        private Dictionary<string, ScriptsData> dictScripts;
        private RunningScriptData runningScript = new("initialize", null);
        /// <summary>変数</summary>
        private Dictionary<string, ScriptsVariableData> dictVariable = new();
        
        private readonly Regex regexVariable = new Regex(@"\$\{(?<name>.+?)\}", RegexOptions.Compiled);
        private readonly Regex regexLocale = new Regex(@"\#\{(?<name>.+?)\}", RegexOptions.Compiled);

        public async UniTask<bool> InitializeScripts(string _path)
        {
            dictScripts = await DataUtil.LoadScripts(_path);
            CallMethod("initialize", "init", 0);
            return true;
        }

        public bool ScriptProcess(SlgSceneManager mng)
        {
            if (runningScript == null)
            { return false; }
            return runningScript.Run(mng);
        }

        public void SetVariable(string scope, string name, string value)
        {
            if (!dictVariable.ContainsKey(scope))
            { dictVariable[scope] = new ScriptsVariableData(scope); }
            dictVariable[scope].SetVariable(name, value);
        }

        public string GetVariable(string scope, string name)
        {
            if (!dictVariable.ContainsKey(scope))
            { return null; }
            return dictVariable[scope].GetVariable(name);
        }

        public string[] AnalysisEmbeddedvariable(string[] texts)
        {
            var result = new List<string>();
            foreach (var text in texts)
            {
                result.Add(AnalysisEmbeddedvariable(text));
            }
            return result.ToArray();
        }

        public string AnalysisEmbeddedvariable(string text)
        {
            var result = text;
            // 変数の置換
            foreach (Match m in regexVariable.Matches(text))
            {
                var splVariable = m.Groups["name"].Value.Split(".");
                var scope = "default";
                var name = m.Groups["name"].Value;
                if (splVariable.Length > 1)
                {
                    scope = splVariable[0];
                    name = splVariable[1];
                }
                var replace = string.Empty;
                if (dictVariable.ContainsKey(scope))
                { replace = dictVariable[scope].GetVariable(name); }
                result = result.Replace(m.Value, replace);
            }
            // 翻訳の置換
            foreach (Match m in regexLocale.Matches(result))
            {
                var variable = m.Groups["name"].Value;
                var replace = string.Empty;
                var t = DataUtil.LocalData.GetEntry(DataUtil.SystemSettingsData.SelectLocale, variable);
                if (t != null)
                {
                    replace = t;
                }
                else
                {
                    replace = LocaleUtil.GetEntry(variable);
                }
                result = result.Replace(m.Value, replace);
            }
            return result;
        }

        public bool CallMethod(string cls, string mtd, int next=-1)
        {
            return runningScript.CallMethod(dictScripts, cls, mtd, next);
        }

        public string NowClass => runningScript.RunningClass;
        public string NowScriptName => runningScript.RunningScriptName;
        public int NowScriptLine => runningScript.RunningScriptLine;

        public RunningScriptData RunningScript
        {
            get => runningScript;
            set => runningScript = value;
        }
        
        public Dictionary<string, ScriptsVariableData> Variable
        {
            get => dictVariable;
            set => dictVariable = value;
        }
    }
}