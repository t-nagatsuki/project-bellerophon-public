using System;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
namespace Functions.Script.Unit
{
    [Serializable]
    public class AddGroupCommand : ICommand
    {
        private int groupId;
        private string name;
        private Color32 color;
        private bool player;
        private int[] friendly;
        private string music;

        public AddGroupCommand(Json.ProcessJson prc)
        {
            groupId = prc.group_id;
            name = prc.name;
            color = prc.color;
            player = prc.player;
            friendly = prc.friendly;
            if (friendly == null) friendly = Array.Empty<int>();
            music = prc.music;
            if (groupId == 0) throw new Exception(LocaleUtil.GetMessage("E_S0007", "group_id", "0"));
            if (String.IsNullOrWhiteSpace(name)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "name"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SetGroup(groupId, name, color, player, friendly, music);
            return true;
        }
    }
}

