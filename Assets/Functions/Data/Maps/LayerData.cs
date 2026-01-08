using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Functions.Data.Maps
{
    [Serializable]
    public class LayerData
    {
        [field: SerializeField] 
        public string Resource { get; set; }
        [field: SerializeField] 
        public TileBase Tile { get; set; }
        [field: SerializeField] 
        public Color32 TileColor { get; set; }
        [field: SerializeField] 
        public bool FlipX { get; set; }
        [field: SerializeField] 
        public bool FlipY { get; set; }

        public LayerData(string resource, Color32 tileColor, TileBase tile = null, bool flipX = false, bool flipY = false)
        {
            Resource = resource;
            Tile = tile;
            TileColor = tileColor;
            FlipX = flipX;
            FlipY = flipY;
        }

        public void SetColor(int? alpha, int? red, int? green, int? blue)
        {
            var color = TileColor;
            if (alpha != null)
            { color.a = Convert.ToByte(alpha); }
            if (red != null)
            { color.r = Convert.ToByte(red); }
            if (green != null)
            { color.g = Convert.ToByte(green); }
            if (blue != null)
            { color.b = Convert.ToByte(blue); }
            TileColor = color;
        }
    }
}