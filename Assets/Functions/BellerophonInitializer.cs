using Functions.Util;
using UnityEngine;

namespace Functions
{
    public class BellerophonInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void OnRuntimeInitialize()
        {
            Debug.Log("BellerophonInitializer.OnRuntimeInitialize");
            DataUtil.InitCommandMap();
            DataUtil.InitSkillMap();
        }
    }
}