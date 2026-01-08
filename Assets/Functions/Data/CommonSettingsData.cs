using System;
using System.Collections.Generic;
using Functions.Data.Maps;
using UnityEngine;
namespace Functions.Data
{
    [Serializable]
    public class CommonSettingsData
    {
        private bool debugMode;
        private bool toolMode;
        private string[] cursorId;
        private string[] highlightAreaId;
        private string[] arrangementAreaId;
        private string[] moveAreaId;
        private string[] attackAreaId;
        private string groupMarker;
        private string selfMarker;
        private string targetMarker;
        private TilesData datCursor;
        private List<MenuData> datMenus;

        public CommonSettingsData(string _id, string[] _cursorId, string[] _highlightAreaId, string[] _arrangementAreaId, string[] _moveAreaId, string[] _attackAreaId, string _groupMarker, string _selfMarker, string _targetMarker)
        {
            datCursor = new TilesData(_id);
            cursorId = _cursorId;
            highlightAreaId = _highlightAreaId;
            arrangementAreaId = _arrangementAreaId;
            moveAreaId = _moveAreaId;
            attackAreaId = _attackAreaId;
            groupMarker = _groupMarker;
            selfMarker = _selfMarker;
            targetMarker = _targetMarker;
            datMenus = new List<MenuData>();
        }

        public void AddCursor(TileData _tile)
        {
            datCursor.Tiles[_tile.TileId] = _tile;
        }

        public void AddMenu(Json.SystemMenuJson _json)
        {
            datMenus.Add(new MenuData(_json));
        }

        public bool DebugMode
        {
            get => debugMode;
            set => debugMode = value;
        }

        public bool ToolMode
        {
            get => toolMode;
            set => toolMode = value;
        }

        public TileData Cursor(int _idx)
        {
            if (cursorId.Length <= _idx)
            { _idx = 0; }
            if (!datCursor.Tiles.ContainsKey(cursorId[_idx]))
            { return null; }
            return datCursor.Tiles[cursorId[_idx]];
        }

        public TileData HighlightArea(int _idx)
        {
            if (highlightAreaId.Length <= _idx)
            { _idx = 0; }
            if (!datCursor.Tiles.ContainsKey(highlightAreaId[_idx]))
            { return null; }
            return datCursor.Tiles[highlightAreaId[_idx]];
        }

        public TileData ArrangementArea(int _idx)
        {
            if (arrangementAreaId.Length <= _idx)
            { _idx = 0; }
            if (!datCursor.Tiles.ContainsKey(arrangementAreaId[_idx]))
            { return null; }
            return datCursor.Tiles[arrangementAreaId[_idx]];
        }

        public TileData MoveArea(int _idx)
        {
            if (moveAreaId.Length <= _idx)
            { _idx = 0; }
            if (!datCursor.Tiles.ContainsKey(moveAreaId[_idx]))
            { return null; }
            return datCursor.Tiles[moveAreaId[_idx]];
        }

        public TileData AttackArea(int _idx)
        {
            if (attackAreaId.Length <= _idx)
            { _idx = 0; }
            if (!datCursor.Tiles.ContainsKey(attackAreaId[_idx]))
            { return null; }
            return datCursor.Tiles[attackAreaId[_idx]];
        }

        public TileData GroupMarker()
        {
            return datCursor.Tiles.GetValueOrDefault(groupMarker);
        }

        public TileData SelfMarker()
        {
            return datCursor.Tiles.GetValueOrDefault(selfMarker);
        }

        public TileData TargetMarker()
        {
            return datCursor.Tiles.GetValueOrDefault(targetMarker);
        }

        public MenuData[] Menus => datMenus.ToArray();
    }
}
