using System;
using System.Collections.Generic;
using Functions.Manager;
using UnityEngine;

namespace Functions.Data.Scripts
{
    [Serializable]
    public class RunningScriptData
    {
        /// <summary>現在実行中スクリプトクラス</summary>
        private string NowClass { get; set; }

        /// <summary>現在実行中スクリプト関数</summary>
        private MethodData NowScripts { get; set; }
        /// <summary>現在実行中スクリプトライン</summary>
        private int NowScriptLine { get; set; }
        /// <summary>現在実行中スクリプトライン</summary>
        private RunningScriptData CallScript { get; set; }

        public RunningScriptData(string nowClass, MethodData nowScripts, int scriptLine = -1)
        {
            NowClass = nowClass;
            NowScripts = nowScripts;
            NowScriptLine = scriptLine;
        }

        public bool Run(SlgSceneManager mng)
        {
            if (CallScript != null)
            {
                if (!CallScript.Run(mng))
                { CallScript = null; }
                return true;
            }
            if (NowScripts == null) return false;
            for (; NowScriptLine < NowScripts.Commands.Count; NowScriptLine++)
            {
                if (NowScriptLine < 0 || !NowScripts.Commands[NowScriptLine].Process(mng)) continue;
                NowScriptLine++;
                break;
            }

            if (NowScriptLine != NowScripts.Commands.Count) return true;
            NowScripts = null;
            return false;
        }

        public bool CallMethod(Dictionary<string, ScriptsData> dictScripts, string callClass, string callMethod, int scriptLine = -1)
        {
            if (CallScript != null)
            {
                return CallScript.CallMethod(dictScripts, callClass, callMethod, scriptLine);
            }
            if (string.IsNullOrWhiteSpace(callClass))
            {
                callClass = NowClass;
            }
            if (!dictScripts.ContainsKey(callClass))
            {
                Debug.Log($"call class {callClass} not defined");
                return false;
            }
            Debug.Log($"call {callClass}, {callMethod}, {scriptLine}");
            if (NowScripts != null) CallScript = new RunningScriptData(callClass, dictScripts[callClass].GetMethod(callMethod), scriptLine);
            else
            {
                NowClass = callClass;
                NowScripts = dictScripts[callClass].GetMethod(callMethod);
                NowScriptLine = scriptLine;
            }
            return true;
        }

        public string RunningClass
        {
            get
            {
                if (CallScript != null) return CallScript.RunningClass;
                return NowClass;
            }
        }

        public string RunningScriptName
        {
            get
            {
                if (CallScript != null) return CallScript.RunningScriptName;
                return NowScripts == null ? string.Empty : NowScripts.Name;
            }
        }

        public int RunningScriptLine
        {
            get
            {
                if (CallScript != null) return CallScript.RunningScriptLine;
                return NowScriptLine;
            }
        }
    }
}