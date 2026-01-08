using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Functions.Data.Maps;
using Functions.Enum;
using Functions.Json;
using Functions.Util;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = Functions.Data.Maps.TileData;

namespace Functions.Manager
{
    public class MapEditorManager : MonoBehaviour
    {
        [SerializeField] private GameObject gridGroup;
        [SerializeField] private TileBase tileGrid;
        [SerializeField] private TileBase tileCollision;
        [SerializeField] private TileData tileCursor;
        [SerializeField] private TileData tileGridCursor;
        [SerializeField] private TileData tileDark;
        [SerializeField] private Tilemap mapGrid;
        [SerializeField] private Tilemap mapDark;
        [SerializeField] private TileMapManager mngStage;
        [SerializeField] private Tilemap mapSelect;
        [SerializeField] private TileMapManager mngCursor;
        [SerializeField] private Tilemap mapUnit;
        [SerializeField] private AudioSource music;
        [SerializeField] private MapEditorWindowManager mngWindow;

        private Dictionary<string, TilesData> dictTiles;
        private Dictionary<string, MapData> dictMaps;
        private Dictionary<Vector3Int, TileData> dictMapTiles;
        private Dictionary<Vector2Int, int> dictHeight;
        private Dictionary<Vector2Int, bool> canCursor;

        private Camera camMain;
        private string nameFile = "default_map";
        /// <summary>マップ情報</summary>
        private MapData map;
        /// <summary>カーソル座標</summary>
        private Vector2Int posCursor;
        /// <summary>画面中心座標</summary>
        private Vector2Int posCenter;
        /// <summary>現在高度</summary>
        private int nowHeight;
        /// <summary>現在表示スケール</summary>
        private int nowScale = 4;
        /// <summary>配置タイル</summary>
        private TileData selectTile;

        private bool isPlayMusic;
        private bool isPlace;
        private bool isDrag;
        private bool isFreeLook;
        private bool isCtrl;
        private Vector3 posCameraMoveStart;
        private Vector3 posMoveStart;

        private BasicAction action;
        /// <summary>カーソル移動入力待機時間</summary>
        private float waitNavigate = 999;
        /// <summary>カーソル移動入力制限時間</summary>
        private const float WaitNavigateTime = 0.2f;

        async void Awake()
        {
            camMain = Camera.main;
            action = new BasicAction();
            action.Enable();
            mngStage.Initialize(camMain, action);
            var pathBase = DataUtil.PathBase;
            //var logWriter = new LogWriter(Path.Combine(pathBase, "logs", "debug.log"), this.GetCancellationTokenOnDestroy());

            dictMaps = new Dictionary<string, MapData>();
            dictMapTiles = new Dictionary<Vector3Int, TileData>();
            dictHeight = new Dictionary<Vector2Int, int>();
            canCursor = new Dictionary<Vector2Int, bool>();

            //// 各種定義の読込
            // マップチップ定義読込
            try
            {
                dictTiles = await DataUtil.LoadTiles(pathBase);
            }
            catch (Exception ex)
            {
                mngWindow.SetError(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace);
                return;
            }
            if (dictTiles.Count == 0)
            {
                mngWindow.SetError("タイルセット定義が存在しません。");
                return;
            }
            mngWindow.Initialize(this);
            InitializeMap(new MapData());
        }

        private void OnDisable()
        {
            action.Disable();
        }

        // Update is called once per frame
        void Update()
        {
            mngCursor.ClearAllTiles();
            if (mngWindow.WindowProcess(this))
            { return; }
            if (action.UI.Ctrl.WasPressedThisFrame())
            { isCtrl = true; }
            else if (action.UI.Ctrl.WasReleasedThisFrame())
            { isCtrl = false; }
            var rightClick = action.UI.RightClick.WasPressedThisFrame();
            if (rightClick)
            {
                if (isCtrl)
                {
                    var pos = new Vector3Int(posCursor.x, posCursor.y, nowHeight * 2);
                    if (dictMapTiles.TryGetValue(pos, out var tile))
                    {
                        selectTile = tile;
                        mngWindow.SelectTile(selectTile);
                    }
                    else
                    {
                        mngWindow.UnselectTile();
                        selectTile = null;
                    }
                    return;
                }
                else if (selectTile != null)
                {
                    mngWindow.UnselectTile();
                    selectTile = null;
                    return;
                }
            }

            if (action.UI.Click.WasPressedThisFrame())
            { isPlace = true; }
            else if (!action.UI.Click.IsPressed())
            { isPlace = false; }
            if (action.UI.MiddleClick.WasPressedThisFrame())
            {
                isDrag = true;
                isFreeLook = true;
                posCameraMoveStart = camMain.transform.position;
                var mpos = action.UI.Point.ReadValue<Vector2>();
                posMoveStart = camMain.ScreenToWorldPoint(new Vector3(mpos.x, mpos.y, 0));
            }
            else if (!action.UI.MiddleClick.IsPressed())
            { isDrag = false; }
            if (action.UI.ScrollWheel.WasPerformedThisFrame())
            {
                if (isCtrl)
                {
                    // 高さ変更
                    if (action.UI.ScrollWheel.ReadValue<Vector2>().y > 0)
                    { mngWindow.MapEditorToolBar.SetHeight(mngWindow.MapEditorToolBar.NowHeight + 1); }
                    else if (action.UI.ScrollWheel.ReadValue<Vector2>().y < 0)
                    { mngWindow.MapEditorToolBar.SetHeight(mngWindow.MapEditorToolBar.NowHeight - 1); }
                }
                else
                {
                    // 拡大・縮小
                    if (action.UI.ScrollWheel.ReadValue<Vector2>().y > 0)
                    { nowScale = math.clamp(nowScale + 1, 3, 7); }
                    else if (action.UI.ScrollWheel.ReadValue<Vector2>().y < 0)
                    { nowScale = math.clamp(nowScale - 1, 3, 7); }
                    gridGroup.transform.localScale = new Vector3(nowScale * 0.25f, nowScale * 0.25f, nowScale * 0.25f);
                }
            }
            if (isDrag)
            {
                var mpos = action.UI.Point.ReadValue<Vector2>();
                var after = camMain.ScreenToWorldPoint(new Vector3(mpos.x, mpos.y, 0));
                var pos = camMain.transform.position;
                pos.x = posCameraMoveStart.x - (after.x - posMoveStart.x);
                pos.y = posCameraMoveStart.y - (after.y - posMoveStart.y);
                camMain.transform.position = Vector3.Lerp(camMain.transform.position, pos, 0.5f);
            }
            if (action.UI.Grid.WasPressedThisFrame())
            { mngWindow.MapEditorToolBar.Grid.value = !mngWindow.MapEditorToolBar.Grid.value; }
            if (action.UI.Collision.WasPressedThisFrame())
            { mngWindow.MapEditorToolBar.Collision.value = !mngWindow.MapEditorToolBar.Collision.value; }
            if (action.UI.Music.WasPressedThisFrame())
            { mngWindow.MapEditorToolBar.Music.value = !mngWindow.MapEditorToolBar.Music.value; }
            SetCursor(posCursor);
            ControlCursor();
        }

        private void ControlCursor()
        {
            if (waitNavigate >= WaitNavigateTime)
            {
                var navigate = action.UI.Navigate.ReadValue<Vector2>();
                var before = posCursor;
                if (navigate.sqrMagnitude != 0)
                {
                    isFreeLook = false;
                    waitNavigate = 0;
                    if (navigate.x < 0)
                    {
                        posCursor.y += 1;
                    }
                    else if (navigate.x > 0)
                    {
                        posCursor.y -= 1;
                    }
                    if (navigate.y < 0)
                    {
                        posCursor.x -= 1;
                    }
                    else if (navigate.y > 0)
                    {
                        posCursor.x += 1;
                    }
                    if (!GetCanMove(posCursor))
                    {
                        posCursor = before;
                    }
                    SetViewCenter(posCursor);
                }
                else
                {
                    waitNavigate = 0.15f;
                    posCursor = mngStage.GetMouseTile(nowHeight, nowScale);
                    if (!GetCanMove(posCursor))
                    {
                        posCursor = before;
                        return;
                    }
                    if (isPlace)
                    { SetTile(new Vector3Int(posCursor.x, posCursor.y, nowHeight * 2), selectTile); }
                }
            }
            else
            {
                waitNavigate += Time.deltaTime;
            }
        }

        public void SetCursor(Vector2Int pos)
        {
            var cursor = new Vector3Int(pos.x, pos.y, nowHeight * 2);
            TileData dat = null;
            if (dictMapTiles.TryGetValue(cursor, out var tile))
            {
                dat = tile;
            }
            if (mapGrid.enabled)
            { mngCursor.SetTile(cursor, tileCursor); }
            else
            { mngCursor.SetTile(cursor, tileGridCursor); }
            mngWindow.SetDebug(cursor, dat);
        }

        public void SetTile(Vector3Int tilePos, TileData dat)
        {
            if (dat != null)
            { dictMapTiles[tilePos] = dat; }
            else
            { dictMapTiles.Remove(tilePos); }
            mngStage.SetColor(tilePos, Color.white, LayerType.All);
            mngStage.SetTile(tilePos, dat);
            var pos = new Vector2Int(tilePos.x, tilePos.y);
            SetTileHeight(pos);
            SetCollision(pos);
        }

        public void SetTile(Vector3Int tilePos, string chipSetId, string chipId)
        {
            if (!dictTiles.ContainsKey(chipSetId) || !dictTiles[chipSetId].Tiles.ContainsKey(chipId))
            {
                return;
            }
            dictMapTiles[tilePos] = dictTiles[chipSetId].Tiles[chipId];
            mngStage.SetColor(tilePos, Color.white, LayerType.All);
            mngStage.SetTile(tilePos, dictTiles[chipSetId].Tiles[chipId]);
            var pos = new Vector2Int(tilePos.x, tilePos.y);
            SetTileHeight(pos);
            SetCollision(pos);
        }

        private void SetTileHeight(Vector2Int pos)
        {
            var height = 0;
            var voidtile = true;
            for (var z = 0;z < 4; z++)
            {
                var p = new Vector3Int(pos.x, pos.y, z * 2);
                if (!dictMapTiles.TryGetValue(p, out var tile))
                { continue; }
                if (tile.Collision)
                { height = z * 2; }
                voidtile = dictMapTiles[p].VoidTile;
            }
            dictHeight[pos] = height;
            canCursor[pos] = !voidtile;
        }

        public void SetColor(Vector3Int pos, Color32 col, LayerType layer)
        {
            mngStage.SetColor(pos, col, layer);
        }

        public void InitializeMaps(MapData dat)
        {
            dictMaps.Clear();
            mngWindow.MapEditorToolBar.MapIdDropdown.choices.Clear();
            dictMapTiles.Clear();
            InitializeMap(dat);
        }

        public void InitializeMap(MapData dat)
        {
            TemporarilySavedMap();
            dictMaps.Remove(dat.MapId);
            dictMaps[dat.MapId] = dat;
            if (!mngWindow.MapEditorToolBar.MapIdDropdown.choices.Contains(dat.MapId))
            { mngWindow.MapEditorToolBar.MapIdDropdown.choices.Add(dat.MapId); }
            SetMap(dat.MapId);
        }

        private void TemporarilySavedMap()
        {
            if (map == null)
            { return; }
            map.Tiles.Clear();
            foreach (var pos in dictMapTiles.Keys)
            {
                if (pos.x < 0 || pos.x > map.Max.x || pos.y < 0 || pos.y > map.Max.y) continue;
                map.Tiles.Add(new MapTileData()
                {
                    Position = new Vector3Int(pos.x, pos.y, pos.z / 2),
                    TileSetId = dictMapTiles[pos].TileSetId,
                    TileId = dictMapTiles[pos].TileId
                });
            }
        }

        public void DeleteMap()
        {
            if (dictMaps.Count < 2)
            { return; }
            dictMaps.Remove(map.MapId);
            mngWindow.MapEditorToolBar.MapIdDropdown.choices.Remove(map.MapId);
            SetMap(dictMaps.Keys.ToArray()[0]);
        }

        public void SetGrid()
        {
            SetTileColor();
            mapGrid.ClearAllTiles();
            if (!tileGrid || !mapGrid.enabled)
            { return; }
            for (var x = 0; x <= map.Max.x; x++)
            {
                for (var y = 0; y <= map.Max.y; y++)
                {
                    mapGrid.SetTile(new Vector3Int(x, y, nowHeight *2), tileGrid);
                }
            }
            SetCursor(posCursor);
            if (!isFreeLook)
            { SetViewCenter(posCursor); }
        }

        public void SetCollision()
        {
            mapSelect.ClearAllTiles();
            if (!tileCollision || !mapSelect.enabled)
            { return; }
            for (var x = 0; x <= map.Max.x; x++)
            {
                for (var y = 0; y <= map.Max.y; y++)
                {
                    SetCollision(new Vector2Int(x, y));
                }
            }
        }

        public void SetCollision(Vector2Int pos)
        {
            if (!tileCollision || !mapSelect.enabled)
            { return; }
            for (var z = 0; z < 4; z++)
            { mapSelect.SetTile(new Vector3Int(pos.x, pos.y, z * 2), null); }
            if (canCursor.ContainsKey(pos) && canCursor[pos])
            { mapSelect.SetTile(new Vector3Int(pos.x, pos.y, dictHeight[pos]), tileCollision); }
        }

        public void SetMap(string newMap)
        {
            if (!dictMaps.ContainsKey(newMap))
            {
                return;
            }

            mngStage.ClearAllTiles();
            dictMapTiles.Clear();
            dictHeight.Clear();
            canCursor.Clear();
            map = dictMaps[newMap];
            mngWindow.MapEditorToolBar.MapIdDropdown.value = map.MapId;
            if (isPlayMusic)
            { SetMusic(map.Music, true); }

            SetTileMap();
            SetGrid();
            SetCollision();
        }

        private void SetTileMap()
        {
            for (var x = -10; x <= map.Max.x + 10; x++)
            {
                for (var y = -10; y <= map.Max.y + 10; y++)
                {
                    if (x < 0 || x > map.Max.x || y < 0 || y > map.Max.y)
                    {
                        SetTile(new Vector3Int(x, y, 0), map.TileSet, "void");
                    }
                    else
                    {
                        SetTile(new Vector3Int(x, y, 0), map.TileSet, map.InitTile);
                    }
                }
            }
            foreach (var tile in map.Tiles)
            {
                SetTile(new Vector3Int(tile.Position.x, tile.Position.y, tile.Position.z * 2), tile.TileSetId, tile.TileId);
            }
        }

        private void SetTileColor()
        {
            mapDark.ClearAllTiles();
            foreach (var pos in dictMapTiles.Keys)
            {
                var tile = dictMapTiles[pos];
                if (!dictTiles.ContainsKey(tile.TileSetId) || !dictTiles[tile.TileSetId].Tiles.ContainsKey(tile.TileId))
                {
                    return;
                }
                if (pos.z != nowHeight * 2)
                {
                    mapDark.SetTile(pos, tileDark.BaseLayer.Tile);
                }
            }
        }

        public void SaveMap()
        {
            TemporarilySavedMap();
            var jmaps = new List<MapJson>();
            foreach (var key in dictMaps.Keys)
            {
                var lst = new List<MapTileJson>();
                var data = dictMaps[key];
                foreach (var tile in data.Tiles)
                {
                    lst.Add(new MapTileJson()
                    {
                        pos = tile.Position,
                        tile_set = tile.TileSetId,
                        id = tile.TileId
                    });
                }
                jmaps.Add(new MapJson()
                {
                    id = data.MapId,
                    tile_set = data.TileSet,
                    music = data.Music,
                    max = data.Max,
                    init_tile = data.InitTile,
                    tiles = lst.ToArray()
                });
            }
            var maps = new MapsJson()
            {
                maps = jmaps.ToArray()
            };
            DataUtil.SaveData<MapsJson>(Path.Combine(DataUtil.PathBase, "data", "map", $"{nameFile}.json"), maps);
        }

        public async void LoadMap(string path)
        {
            // マップ定義読込
            nameFile = path;
            dictMaps.Clear();
            mngWindow.MapEditorToolBar.MapIdDropdown.choices.Clear();
            var pathBase = Path.Combine(DataUtil.PathBase, "data", "map");
            if (File.Exists(Path.Combine(pathBase, $"{path}.json"))) dictMaps = await DataUtil.LoadMapJson(Path.Combine(pathBase, $"{path}.json"), dictMaps);
            else dictMaps = await DataUtil.LoadMapYaml(Path.Combine(pathBase, $"{path}.yml"), dictMaps);
            map = null;
            foreach (var key in dictMaps.Keys)
            { mngWindow.MapEditorToolBar.MapIdDropdown.choices.Add(key); }
            if (dictMaps.Count > 0)
            { SetMap(dictMaps.Keys.First()); }
        }

        public void SetViewCenter(Vector2Int pos)
        {
            posCenter = pos;
            posCursor = pos;
            var tilePos = new Vector3Int(pos.x, pos.y, nowHeight);
            var cpos = mngStage.GetCellCenterWorld(tilePos);
            cpos.z = -10;
            camMain.transform.position = cpos;
        }

        public void SetHeight(int height)
        {
            nowHeight = height;
            SetGrid();
        }

        public void SetDisplayGrid(bool flg)
        {
            mapGrid.enabled = flg;
            SetGrid();
        }

        public void SetDisplayCollision(bool flg)
        {
            mapSelect.enabled = flg;
            SetCollision();
        }

        public void SetPlayMusic(bool flg)
        {
            isPlayMusic = flg;
            if (flg)
            {
                SetMusic(map.Music, true);
            }
            else
            {
                StopMusic();
            }
        }

        private async void SetMusic(string name, bool loop)
        {
            Debug.Log($"start_music {name}");
            if (String.IsNullOrWhiteSpace(name))
            {
                music.Stop();
                return;
            }
            music.clip = await DataUtil.GetAudio(name);
            music.loop = loop;
            music.Play();
        }

        private void StopMusic()
        {
            Debug.Log("stop_music");
            music.Stop();
        }

        public bool GetCanMove(Vector2Int pos)
        {
            if (map == null)
            { return false; }
            return pos.x >= 0 && pos.x <= map.Max.x && pos.y >= 0 && pos.y <= map.Max.y;
        }

        public int Distance(Vector2Int posBaase, Vector2Int posTarget)
        {
            return math.abs(posBaase.x - posTarget.x) + math.abs(posBaase.y - posTarget.y);
        }

        public Vector2Int CursorPosition => posCursor;

        public bool IsCancel => action.UI.Cancel.triggered;

        public BasicAction Action => action;

        public Dictionary<string, TilesData> TilesDatas => dictTiles;

        public TileData SelectTile
        {
            get => selectTile;
            set => selectTile = value;
        }
        
        public float WaitNavigate
        {
            set => waitNavigate = value;
        }

        public string FileName
        {
            get => nameFile;
            set => nameFile = value;
        }

        public MapEditorWindowManager MapEditorWindowManager => mngWindow;

        public Dictionary<string, MapData> Maps => dictMaps;
        public MapData MapData => map;
    }
}
