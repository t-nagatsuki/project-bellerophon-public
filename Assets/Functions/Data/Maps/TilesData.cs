using System;
using System.Collections.Generic;

namespace Functions.Data.Maps
{
    [Serializable]
    public class TilesData
    {
        public string TileSetId { get; set; }
        public int Pixels { get; set; }
        public Dictionary<string, TileData> Tiles { get; set; }

        public TilesData(string tileSetId, int pixels = 32)
        {
            TileSetId = tileSetId;
            Pixels = pixels;
            Tiles = new Dictionary<string, TileData>();
        }
    }
}