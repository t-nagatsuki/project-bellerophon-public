using System;
using Functions.Enum;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
namespace Functions.Script.Map
{
    [Serializable]
    public class ChgColorCommand : ICommand
    {
        private Vector3Int pos;
        private Color32 color;
        private LayerType layer = LayerType.All;

        public ChgColorCommand(Json.ProcessJson prc)
        {
            pos = prc.pos;
            color = prc.color;
            if (!String.IsNullOrWhiteSpace(prc.layer))
            {
                switch(prc.layer.ToLower())
                {
                    case "base":
                        layer = LayerType.Base; break;
                    case "overlay_back":
                        layer = LayerType.OverlayBack; break;
                    case "overlay_front":
                        layer = LayerType.OverlayFront; break;
                    case "all":
                        layer = LayerType.All; break;
                    default:
                        Debug.Log(LocaleUtil.GetMessage("E_S0003", $"layer {prc.layer}"));
                        layer = LayerType.All; break;
                }
            }
            if (pos.x < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.x"));
            if (pos.y < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.y"));
            if (pos.z < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.z"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.StageMap.SetColor(pos, color, layer);
            return true;
        }
    }
}

