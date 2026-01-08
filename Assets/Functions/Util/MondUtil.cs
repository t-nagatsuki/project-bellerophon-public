using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Functions.Data;
using Functions.Manager;
using Mond;
using Mond.Libraries;

namespace Functions.Util
{
    public class MondUtil
    {
        private string pathBase;
        private Dictionary<string, string> cachePrograms;
        private MondState state;

        public MondUtil(string pathBase, ScriptManager mngScript)
        {
            this.pathBase = pathBase;
            cachePrograms = new Dictionary<string, string>();
            state = new MondState();
            state["get_variable"] = MondValue.Function((_, args) => mngScript.GetVariable(args[0], args[1]));
            state["set_variable"] = MondValue.Function((_, args) =>
            {
                mngScript.SetVariable(args[0], args[1], args[2]);
                return MondValue.Undefined;
            });
        }

        public void SetState(string key, MondValue value)
        {
            state[key] = value;
        }

        public async UniTask<MondValue> LoadScript(string path)
        {
            if (!cachePrograms.ContainsKey(path))
            {
                var prg = string.Empty;
                var pathLocal = Path.Combine(pathBase, "script", "calc", $"{path}.mond");
                if (File.Exists(pathLocal))
                { prg = await DataUtil.ReadText(pathLocal); }
                cachePrograms[path] = prg;
            }
            return state.Run(cachePrograms[path]);
        }
    }
}