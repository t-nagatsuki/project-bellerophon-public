using System;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
namespace Functions.Script.Camera
{
    [Serializable]
    public class ViewCenterCommand : ICommand
    {
        private Vector2Int pos;
        private float time;

        public ViewCenterCommand(Json.ProcessJson prc)
        {
            pos = new Vector2Int(prc.pos.x, prc.pos.y);
            time = prc.time;
            if (pos.x < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.x"));
            if (pos.y < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.y"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SetViewCenter(pos, time);
            return true;
        }
    }
}

