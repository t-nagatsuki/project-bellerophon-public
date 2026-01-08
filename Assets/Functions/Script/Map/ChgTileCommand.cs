using System;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
namespace Functions.Script.Map
{
    [Serializable]
    public class ChgTileCommand : ICommand
    {
        private Vector3Int pos;
        private string name;

        public ChgTileCommand(Json.ProcessJson prc)
        {
            pos = prc.pos;
            name = prc.name;
            
            if (pos.x < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.x"));
            if (pos.y < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.y"));
            if (pos.z < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.z"));
            if (string.IsNullOrWhiteSpace(name)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "name"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SetTile(pos, name);
            return true;
        }
    }
}

