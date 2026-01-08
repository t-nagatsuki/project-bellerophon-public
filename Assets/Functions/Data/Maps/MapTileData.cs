using System;
using UnityEngine;

namespace Functions.Data.Maps
{
    [Serializable]
    public class MapTileData
    {
        public Vector3Int Position { get; set; }
        public string TileSetId { get; set; }
        public string TileId { get; set; }
    }
}