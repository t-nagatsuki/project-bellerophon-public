using System;
using Functions.Manager;
using Functions.Util;
using UnityEngine;

namespace Functions.Script.Effect
{
    [Serializable]
    public class BlastCommand : ICommand
    {
        private Vector2Int pos;
        
        public BlastCommand(Json.ProcessJson prc)
        {
            pos = new Vector2Int(prc.pos.x, prc.pos.y);
            if (pos.x < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.x"));
            if (pos.y < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.y"));
        }
        
        public bool Process(SlgSceneManager mng)
        {
            mng.BlastEffect(pos);
            return true;
        }
    }
}