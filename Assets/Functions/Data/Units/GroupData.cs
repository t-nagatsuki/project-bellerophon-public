using System;
using UnityEngine;

namespace Functions.Data.Units
{
    [Serializable]
    public class GroupData
    {
        public int GroupId;
        public string GroupName;
        public Color32 GroupColor;
        public bool Player;
        public int[] Friendly;
        public string Music;

        public GroupData(int id, string name, Color32 col, bool player, int[] friend, string music)
        {
            GroupId = id;
            GroupName = name;
            GroupColor = col;
            Player = player;
            Friendly = friend ?? Array.Empty<int>();
            Music = music;
        }

        public void SetData(string name, Color32 col, bool player, int[] friend, string music)
        {
            GroupName = name;
            GroupColor = col;
            Player = player;
            Friendly = friend ?? Array.Empty<int>();
            Music = music;
        }
    }
}