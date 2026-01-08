using System;
using Functions.Enum;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
namespace Functions.Script.Unit
{
    [Serializable]
    public class AddUnitCommand : ICommand
    {
        private int groupId;
        private string unitId;
        private string charaId;
        private Vector2Int position;
        private int direction;
        private string unit;
        private string chara;
        private int lv;
        private string unitName;
        private string charaName;
        private bool nonArrangement;
        private bool animation;
        private bool hidden;
        private MoveType moveType;
        private ActionType actionType;
        private Vector2Int targetPosition;

        public AddUnitCommand(Json.ProcessJson prc)
        {
            groupId = prc.group_id;
            unitId = prc.unit_id;
            charaId = prc.chara_id;
            position = new Vector2Int(prc.pos.x, prc.pos.y);
            direction = prc.direction;
            unit = prc.unit;
            chara = prc.chara;
            lv = Math.Clamp(prc.lv, 1, 99);
            unitName = prc.unit_name;
            charaName = prc.chara_name;
            nonArrangement = prc.non_arrangement;
            animation = prc.animation;
            hidden = prc.hidden;
            if (!String.IsNullOrWhiteSpace(prc.move_type))
            {
                switch (prc.move_type.ToLower())
                {
                    case "space":
                        moveType = MoveType.Space;
                        break;
                    case "air":
                        moveType = MoveType.Air;
                        break;
                    case "ground":
                        moveType = MoveType.Ground;
                        break;
                    case "underwater":
                        moveType = MoveType.Underwater;
                        break;
                    default:
                        moveType = MoveType.Space;
                        break;
                }
            }
            
            if (groupId == 0) throw new Exception(LocaleUtil.GetMessage("E_S0007", "group_id", "0"));
            if (position.x < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.x"));
            if (position.y < 0) throw new Exception(LocaleUtil.GetMessage("E_S0002", "pos.y"));

            if (String.IsNullOrWhiteSpace(prc.action_type))
            { return; }
            switch (prc.action_type.ToLower())
            {
                case "destination":
                    actionType = ActionType.Destination;
                    break;
                case "guard":
                    actionType = ActionType.Guard;
                    break;
                case "peace":
                    actionType = ActionType.Peace;
                    break;
                default:
                    actionType = ActionType.Normal;
                    break;
            }
            targetPosition = prc.target_position;
            if (actionType == ActionType.Destination && targetPosition == null) throw new Exception(LocaleUtil.GetMessage("E_S0003", "target_position"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.AddUnit(
                groupId, 
                unitId, 
                charaId, 
                position, 
                direction,
                moveType,
                unit, 
                chara, 
                lv,
                nonArrangement,
                animation,
                hidden,
                unitName,
                charaName,
                actionType,
                targetPosition);
            return animation;
        }
    }
}

