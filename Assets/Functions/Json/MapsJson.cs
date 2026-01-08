using System;
using UnityEngine;
using VYaml.Annotations;
namespace Functions.Json
{
    [Serializable]
    [YamlObject]
    public partial class MapsJson
    {
        public MapJson[] maps;
    }

    [Serializable]
    [YamlObject]
    public partial class MapJson
    {
        public string id;
        public string tile_set;
        public string music;
        public Vector2Int max;
        public string init_tile;
        public MapTileJson[] tiles;
    }

    [Serializable]
    [YamlObject]
    public partial class MapTileJson
    {
        public Vector3Int pos;
        public string tile_set;
        public string id;
    }
}