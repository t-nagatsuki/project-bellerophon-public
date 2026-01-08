using Functions.Data;
using Functions.Data.Maps;
using Functions.Enum;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = Functions.Data.Maps.TileData;
namespace Functions.Manager
{
    public class TileMapManager : MonoBehaviour
    {
        [SerializeField] private Tilemap mapBase;
        [SerializeField] private Tilemap mapBack;
        [SerializeField] private Tilemap mapFront;
        [SerializeField] private int mouseHeight = 10;
        [SerializeField] private float mouseDelta = 0;
        [SerializeField] private Vector3 mouseOffset = new Vector3(0, -1, 0);
        [SerializeField] private float scaleOffset = 3.5f;

        private Camera camMain;
        private BasicAction action;

        private Vector2Int NONE_POSITION = new Vector2Int(-1, -1);
        private Matrix4x4 X_FLIP = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, 1f, 1f));
        private Matrix4x4 Y_FLIP = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, -1f, 1f));
        private Matrix4x4 BOTH_FLIP = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, -1f, 1f));

        public void Initialize(Camera _camera, BasicAction _action)
        {
            camMain = _camera;
            action = _action;
        }

        public void SetTile(Vector3Int _pos, TileData tileData)
        {
            SetTile(mapBase, _pos, tileData?.BaseLayer);
            SetTile(mapBack, _pos, tileData?.OverlayBackLayer);
            SetTile(mapFront, _pos, tileData?.OverlayFrontLayer);
        }

        public void SetTile(Vector3Int _pos, TileBase _base, TileBase _back, TileBase _front, MoveType move)
        {
            var upos = _pos;
            switch (move)
            {
                case MoveType.Air:
                    upos.z += 1;
                    break;
                case MoveType.Underwater:
                    upos.z -= 1;
                    break;
            }
            SetTile(mapBase, upos, _base);
            SetTile(mapBack, _pos, _back);
            SetTile(mapFront, upos, _front);
        }

        private void SetTile(Tilemap _map, Vector3Int _pos, LayerData _dat)
        {
            if (!_map)
            { return; }
            if (_dat != null)
            {
                _map.SetTile(_pos, _dat.Tile);
                SetFlip(_map, _pos, _dat);
            }
            else
            {
                _map.SetTile(_pos, null);
            }
        }

        private void SetTile(Tilemap _map, Vector3Int _pos, TileBase _dat)
        {
            if (!_map)
            { return; }
            if (_dat != null)
            {
                _map.SetTile(_pos, _dat);
            }
            else
            {
                _map.SetTile(_pos, null);
            }
        }

        public void SetFlip(Tilemap _map, Vector3Int _pos, LayerData _dat)
        {
            if (!_map)
            { return; }
            if (_dat.FlipX && !_dat.FlipY)
            { _map.SetTransformMatrix(_pos, X_FLIP); }
            else if (!_dat.FlipX && _dat.FlipY)
            { _map.SetTransformMatrix(_pos, Y_FLIP); }
            else if (_dat.FlipX && _dat.FlipY)
            { _map.SetTransformMatrix(_pos, BOTH_FLIP); }
            else
            { _map.SetTransformMatrix(_pos, Matrix4x4.identity); }
        }

        public void SetColor(Vector3Int _pos, Color32 _col, LayerType _layer = LayerType.Base)
        {
            switch (_layer)
            {
                case LayerType.Base:
                    if (mapBase)
                    { mapBase.SetColor(_pos, _col); }
                    break;
                case LayerType.OverlayBack:
                    if (mapBack)
                    { mapBack.SetColor(_pos, _col); }
                    break;
                case LayerType.OverlayFront:
                    if (mapFront)
                    { mapFront.SetColor(_pos, _col); }
                    break;
                default:
                    if (mapBase)
                    { mapBase.SetColor(_pos, _col); }
                    if (mapBack)
                    { mapBack.SetColor(_pos, _col); }
                    if (mapFront)
                    { mapFront.SetColor(_pos, _col); }
                    break;
            }
        }

        public void SetTileFlags(Vector3Int _pos, TileFlags _flag, MoveType move = MoveType.Space)
        {
            var upos = _pos;
            switch (move)
            {
                case MoveType.Air:
                    upos.z += 1;
                    break;
                case MoveType.Underwater:
                    upos.z -= 1;
                    break;
            }
            if (mapBase)
            { mapBase.SetTileFlags(upos, _flag); }
            if (mapBack)
            { mapBack.SetTileFlags(_pos, _flag); }
            if (mapFront)
            { mapFront.SetTileFlags(upos, _flag); }
        }
        
        public Vector2Int GetMouseTile(int _scale)
        {
            Vector3 mouse = action.UI.Point.ReadValue<Vector2>();
            mouse.z = mouseHeight;
            var mousePos = camMain.ScreenToWorldPoint(mouse);
            for (var i = mapBase.cellBounds.zMax; i >= 0; i--)
            {
                var v = new Vector3(mousePos.x, mousePos.y + (_scale - scaleOffset) * mouseDelta, i);
                v += mouseOffset;
                var pos = mapBase.WorldToCell(v);
                if (mapBase.HasTile(pos))
                {
                    pos = GetTopTile(pos);
                    return new Vector2Int(pos.x, pos.y);
                }
            }
            return NONE_POSITION;
        }

        public Vector2Int GetMouseTile(int _height, int _scale)
        {
            Vector3 mouse = action.UI.Point.ReadValue<Vector2>();
            mouse.z = mouseHeight;
            var mousePos = camMain.ScreenToWorldPoint(mouse);
            var v = new Vector3(mousePos.x, mousePos.y + (_scale - scaleOffset) * mouseDelta, _height);
            v += mouseOffset;
            var pos = mapBase.WorldToCell(v);
            var result = new Vector2Int(pos.x, pos.y);
            return result;
        }

        public Vector3Int GetTopTile(Vector3Int _pos)
        {
            var newPos = _pos;
            for (var i = _pos.z; i <= mapBase.cellBounds.zMax; i++)
            {
                var pos = new Vector3Int(_pos.x, _pos.y, i);
                if (mapBase.HasTile(pos))
                {
                    newPos = pos;
                }
                else
                {
                    break;
                }
            }
            return newPos;
        }

        public void ClearAllTiles()
        {
            if (mapBase)
            { mapBase.ClearAllTiles(); }
            if (mapBack)
            { mapBack.ClearAllTiles(); }
            if (mapFront)
            { mapFront.ClearAllTiles(); }
        }

        public Vector3Int WorldToCell(Vector3 _pos)
        { return mapBase.WorldToCell(_pos); }

        public Vector3 GetCellCenterWorld(Vector3Int _pos)
        { return mapBase.GetCellCenterWorld(_pos); }
    }
}
