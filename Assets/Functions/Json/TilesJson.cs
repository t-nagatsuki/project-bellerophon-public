using System;
using UnityEngine;
using VYaml.Annotations;
namespace Functions.Json
{
    [Serializable]
    [YamlObject]
    public partial class TilesJson
    {
        public TileJson[] tiles;
    }

    [Serializable]
    [YamlObject]
    public partial class TileJson
    {
        public string id;
        public int pixels;
        public TileChipJson[] tile;
    }

    [Serializable]
    [YamlObject]
    public partial class TileChipJson
    {
        public string name;
        public string[] tile_type;
        public int cursor_index;
        public bool collision;
        public bool void_tile;
        public bool tool_hidden;
        public TileMoveTypeJson[] suitable;
        public TileChipLayerJson base_layer;
        public TileChipLayerJson overlay_back;
        public TileChipLayerJson overlay_front;
    }

    [Serializable]
    [YamlObject]
    public partial class TileMoveTypeJson
    {
        public string move_type;
        public float cost;
    }

    [Serializable]
    [YamlObject]
    public partial class TileChipLayerJson
    {
        public string resource;
        public Vector2Int[] pos;
        public Color32 color;
        public bool flip_x;
        public bool flip_y;
        public float anim_speed;
        public TileOverrideRuleJson override_rule;
        public TileRuleJson[] rules;
    }

    [Serializable]
    [YamlObject]
    public partial class TileRuleJson
    {
        public string output;
        public string resource;
        public Vector2Int[] pos;
        public float perlin;
        public float min_speed;
        public float max_speed;
        public TileRuleNeighborJson[] neighbors;
    }

    [Serializable]
    [YamlObject]
    public partial class TileRuleNeighborJson
    {
        public Vector2Int pos;
        public int flg;
    }

    [Serializable]
    [YamlObject]
    public partial class TileOverrideRuleJson
    {
        public string tileset;
        public string tile;
        public int[] original_sprite;
        public int[] original_sprite_index;
        public string override_resource;
        public Vector2Int[] override_pos;
    }
}