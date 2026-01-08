using System;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
namespace Functions.Script.Map
{
    [Serializable]
    public class SetArrangementAreaCommand : ICommand
    {
        private Vector2Int pos;
        private Vector2Int areaSize;

        public SetArrangementAreaCommand(Json.ProcessJson prc)
        {
            pos = new Vector2Int(prc.pos.x, prc.pos.y);
            areaSize = new Vector2Int(prc.area_size.x, prc.area_size.y);
            
            if (pos.x < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.x"));
            if (pos.y < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.y"));
            if (areaSize.x < 1) throw new Exception(LocaleUtil.GetMessage("E_S0004", "area_size.x", "1"));
            if (areaSize.y < 1) throw new Exception(LocaleUtil.GetMessage("E_S0004", "area_size.y", "1"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SetArrangementArea(pos, areaSize);
            return true;
        }
    }
}

