using System;
using System.Collections.Generic;
using Functions.Enum;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Functions.Data.Maps
{
    [Serializable]
    public class TileData
    {
        [field: SerializeField] 
        public string TileSetId { get; set; }
        [field: SerializeField] 
        public string TileId { get; set; }
        [field: SerializeField] 
        public string[] TileType { get; set; }
        [field: SerializeField] 
        public Dictionary<MoveType, MoveCostData> MoveCost { get; set; }
        [field: SerializeField] 
        public LayerData BaseLayer { get; set; }
        [field: SerializeField] 
        public LayerData OverlayBackLayer { get; set; }
        [field: SerializeField] 
        public LayerData OverlayFrontLayer { get; set; }
        [field: SerializeField] 
        public int CursorIndex { get; set; }
        [field: SerializeField] 
        public bool Collision { get; set; }
        [field: SerializeField] 
        public bool VoidTile { get; set; }
        [field: SerializeField] 
        public bool ToolHidden { get; set; }

        public TileData(string tileSetId, string tileId)
        {
            TileSetId = tileSetId;
            TileId = tileId;
            MoveCost = new Dictionary<MoveType, MoveCostData>();
            Collision = true;
            VoidTile = true;
            ToolHidden = true;
        }

        public void SetMoveCost(MoveType moveType, float moveCost)
        {
            if (moveCost <= 0) MoveCost.Remove(moveType);
            else
            {
                if (MoveCost.TryGetValue(moveType, out var value)) value.MoveCost = moveCost;
                else MoveCost[moveType] = new MoveCostData(moveType, moveCost);
            }
        }
    }
}