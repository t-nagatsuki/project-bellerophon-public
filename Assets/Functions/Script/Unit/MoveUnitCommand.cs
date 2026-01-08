using System;
using Functions.Data;
using Functions.Enum;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
namespace Functions.Script.Unit
{
    [Serializable]
    public class MoveUnitCommand : ICommand
    {
        private string unitId;
        private Vector2Int position;
        private int direction;
        private bool animation;

        public MoveUnitCommand(Json.ProcessJson prc)
        {
            unitId = prc.unit_id;
            position = new Vector2Int(prc.pos.x, prc.pos.y);
            direction = prc.direction;
            animation = prc.animation;
            if (string.IsNullOrWhiteSpace(unitId)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "unit_id"));
            if (position.x < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.x"));
            if (position.y < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.y"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.MoveUnit(unitId, position, (DirectionType)direction, animation);
            return animation;
        }
    }
}

