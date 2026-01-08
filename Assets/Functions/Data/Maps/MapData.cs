using System;
using System.Collections.Generic;
using UnityEngine;

namespace Functions.Data.Maps
{
    [Serializable]
    public class MapData
    {
        public string MapId { get; set; }
        public string TileSet { get; set; }
        public string Music { get; set; }
        public Vector2Int Max { get; set; }
        public string InitTile { get; set; }
        public List<MapTileData> Tiles { get; set; }

        public MapData()
        {
            MapId = "default_map";
            Max = new Vector2Int(10, 10);
            TileSet = string.Empty;
            InitTile = string.Empty;
            Music = string.Empty;
            Tiles = new List<MapTileData>();
        }

        public MapData(Json.MapJson json)
        {
            MapId = json.id;
            TileSet = json.tile_set;
            Music = json.music;
            Max = json.max;
            InitTile = json.init_tile;
            Tiles = new List<MapTileData>();
            foreach (var t in json.tiles)
            {
                if (string.IsNullOrWhiteSpace(t.tile_set))
                { Tiles.Add(new MapTileData { Position = t.pos, TileSetId = TileSet, TileId = t.id }); }
                else
                { Tiles.Add(new MapTileData { Position = t.pos, TileSetId = t.tile_set, TileId = t.id }); }
            }
        }
    }
}