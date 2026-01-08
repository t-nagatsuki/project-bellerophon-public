using System.Collections.Generic;
using System.IO;
using System.Linq;
using Functions.Data;
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
    public class TileEditorManager : MonoBehaviour
    {
        [SerializeField] private GameObject gridGroup;
        [SerializeField] private TileMapManager mngStage;
        [SerializeField] private TileEditorWindowManager mngWindow;

        private Dictionary<string, TilesData> dictTiles;

        private Camera camMain;
        private string nameFile = "default_tiles";
        /// <summary>現在表示スケール</summary>
        // private int nowScale = 4;
        /// <summary>タイル情報</summary>
        private TilesData selectTiles;
        /// <summary>タイル情報</summary>
        private TileData selectTile;

        private string selectBaseResource;
        private string selectBackResource;
        private string selectFrontResource;

        private List<Sprite> lstBaseSprite;
        private List<Sprite> lstBackSprite;
        private List<Sprite> lstFrontSprite;

        private int selectBaseSprite;
        private int selectBackSprite;
        private int selectFrontSprite;

        private bool isCtrl;

        private BasicAction action;
        /// <summary>カーソル移動入力待機時間</summary>
        private float waitNavigate = 999;
        /// <summary>カーソル移動入力制限時間</summary>
        private const float WaitNavigateTime = 0.2f;

        void Awake()
        {
            camMain = Camera.main;
            action = new BasicAction();
            action.Enable();
            mngStage.Initialize(camMain, action);
            //var logWriter = new LogWriter(Path.Combine(pathBase, "logs", "debug.log"), this.GetCancellationTokenOnDestroy());

            dictTiles = new Dictionary<string, TilesData>();
            lstBaseSprite = new List<Sprite>();
            lstBackSprite = new List<Sprite>();
            lstFrontSprite = new List<Sprite>();
            nameFile = "default_tiles";
            dictTiles["default_tiles"] = new TilesData("default_tiles");
            selectTiles = dictTiles["default_tiles"];
            selectTiles.Tiles["default_tile"] = new TileData("default_tiles", "default_tile");
            selectTile = selectTiles.Tiles["default_tile"];
            mngWindow.TileEditorToolBar.InitializeTileSet(this, selectTiles.TileSetId, selectTile.TileId);

            mngWindow.Initialize(this);
        }

        private void OnDestroy()
        {
            action.Disable();
        }

        // Update is called once per frame
        void Update()
        {
            if (mngWindow.WindowProcess(this))
            { return; }
            if (action.UI.Ctrl.WasPressedThisFrame())
            { isCtrl = true; }
            else if (action.UI.Ctrl.WasReleasedThisFrame())
            { isCtrl = false; }
            var rightClick = action.UI.RightClick.WasPressedThisFrame();
            if (action.UI.ScrollWheel.WasPerformedThisFrame())
            {
                if (isCtrl)
                {
                    // レイヤー変更
                    if (action.UI.ScrollWheel.ReadValue<Vector2>().y > 0)
                    {  }
                    else if (action.UI.ScrollWheel.ReadValue<Vector2>().y < 0)
                    {  }
                }
                else
                {
                    // 拡大・縮小
                    //if (action.UI.ScrollWheel.ReadValue<Vector2>().y > 0)
                    //{ nowScale = math.clamp(nowScale + 1, 3, 7); }
                    //else if (action.UI.ScrollWheel.ReadValue<Vector2>().y < 0)
                    //{ nowScale = math.clamp(nowScale - 1, 3, 7); }
                    //gridGroup.transform.localScale = new Vector3(nowScale * 0.25f, nowScale * 0.25f, nowScale * 0.25f);
                }
            }
        }

        public void SetSprite(int layer, string path, Sprite sprite)
        {
            switch (layer)
            {
                case 1:
                    if (selectBaseResource != path)
                    {
                        lstBaseSprite.Clear();
                        selectBaseSprite = 0;
                        selectBaseResource = path;
                    }
                    if (lstBaseSprite.Count <= selectBaseSprite)
                    {
                        lstBaseSprite.Add(sprite);
                    }
                    else
                    {
                        lstBaseSprite[selectBaseSprite] = sprite;
                    }
                    break;
                case 2:
                    if (selectBackResource != path)
                    {
                        lstBackSprite.Clear();
                        selectBackSprite = 0;
                        selectBackResource = path;
                    }
                    if (lstBackSprite.Count <= selectBackSprite)
                    {
                        lstBackSprite.Add(sprite);
                    }
                    else
                    {
                        lstBackSprite[selectBackSprite] = sprite;
                    }
                    break;
                case 3:
                    if (selectFrontResource != path)
                    {
                        lstFrontSprite.Clear();
                        selectFrontSprite = 0;
                        selectFrontResource = path;
                    }
                    if (lstFrontSprite.Count <= selectFrontSprite)
                    {
                        lstFrontSprite.Add(sprite);
                    }
                    else
                    {
                        lstFrontSprite[selectFrontSprite] = sprite;
                    }
                    break;
            }
            mngWindow.TileEditorToolBar.DisplaySelectSprite(this);
            DisplayTileSample();
        }

        public void RemoveSprite(int layer)
        {
            switch (layer)
            {
                case 1:
                    if (lstBaseSprite.Count <= selectBaseSprite)
                    { return; }
                    lstBaseSprite.RemoveAt(selectBaseSprite);
                    selectBaseSprite = math.clamp(selectBaseSprite - 1, 0, lstBaseSprite.Count - 1);
                    break;
                case 2:
                    if (lstBackSprite.Count <= selectBackSprite)
                    { return; }
                    lstBackSprite.RemoveAt(selectBackSprite);
                    selectBackSprite = math.clamp(selectBackSprite - 1, 0, lstBackSprite.Count - 1);
                    break;
                case 3:
                    if (lstFrontSprite.Count <= selectFrontSprite)
                    { return; }
                    lstFrontSprite.RemoveAt(selectFrontSprite);
                    selectFrontSprite = math.clamp(selectFrontSprite - 1, 0, lstFrontSprite.Count - 1);
                    break;
            }
            mngWindow.TileEditorToolBar.DisplaySelectSprite(this);
            DisplayTileSample();
        }

        public void DisplayTileSample()
        {
            selectTile.BaseLayer = SetTileLayer(selectTile.BaseLayer, lstBaseSprite, selectBaseResource, mngWindow.TileEditorToolBar.BaseLayerColor, mngWindow.TileEditorToolBar.IsBaseFlipX, mngWindow.TileEditorToolBar.BaseAnimationSpeed);
            selectTile.OverlayBackLayer = SetTileLayer(selectTile.OverlayBackLayer, lstBackSprite, selectBackResource, mngWindow.TileEditorToolBar.BackLayerColor, mngWindow.TileEditorToolBar.IsBackFlipX, mngWindow.TileEditorToolBar.BackAnimationSpeed);
            selectTile.OverlayFrontLayer = SetTileLayer(selectTile.OverlayFrontLayer, lstFrontSprite, selectFrontResource, mngWindow.TileEditorToolBar.FrontLayerColor, mngWindow.TileEditorToolBar.IsFrontFlipX, mngWindow.TileEditorToolBar.FrontAnimationSpeed);
            mngStage.SetTile(Vector3Int.zero, selectTile);
        }

        public void SetTile(string tileKey)
        {
            if (!selectTiles.Tiles.TryGetValue(tileKey, out var tile))
            { return; }
            selectTile = tile;
            SetTileSetting(selectTile.BaseLayer, lstBaseSprite);
            SetTileSetting(selectTile.OverlayBackLayer, lstBackSprite);
            SetTileSetting(selectTile.OverlayFrontLayer, lstFrontSprite);
            mngStage.SetTile(Vector3Int.zero, selectTile);
        }
        
        private void SetTileSetting(LayerData dat, List<Sprite> lst)
        {
            lst.Clear();
            if (dat == null)
            { return; }
            switch (dat.Tile)
            {
                case AnimatedTile tile:
                    {
                        foreach (var sprite in tile.m_AnimatedSprites)
                        {
                            lst.Add(sprite);
                        }
                        break;
                    }
                case IsometricRuleTile ruleTile:
                    lst.Add(ruleTile.m_DefaultSprite);
                    break;
                case RuleOverrideTile overrideTile:
                    lst.Add(overrideTile.m_Tile.m_DefaultSprite);
                    break;
                default:
                    lst.Add(((Tile)dat.Tile).sprite);
                    break;
            }
        }

        private LayerData SetTileLayer(LayerData dat, List<Sprite> lst, string path, Color32 color, bool flipX, float anim)
        {
            if (lst.Count == 0)
            {
                return null;
            }
            if (dat == null)
            {
                dat = new LayerData(path, color, flipX: flipX);
            }
            else
            {
                dat.Resource = path;
            }

            if (lst.Count > 1)
            {
                dat.Tile = DataUtil.CreateTile(SelectTile.TileId, lst.ToArray(), anim, Tile.ColliderType.None);
            }
            else
            {
                dat.Tile = DataUtil.CreateTile(SelectTile.TileId, lst[0], color, Tile.ColliderType.None);
            }

            return dat;
        }

        public void ChangeTile(TileData datTile)
        {
            selectTile = datTile;
            selectBaseSprite = 0;
            selectBackSprite = 0;
            selectFrontSprite = 0;
            lstBaseSprite.Clear();
            lstBackSprite.Clear();
            lstFrontSprite.Clear();
            if (selectTile.BaseLayer != null)
            {
                selectBaseResource = selectTile.BaseLayer.Resource;
                if (selectTile.BaseLayer.Tile is IsometricRuleTile tile)
                {
                    lstBaseSprite.Add(tile.m_DefaultSprite);
                }
                /*
                else if (selectTile.BaseLayer.Tile is RuleOverrideTile)
                {
                    foreach (var sprite in ((RuleOverrideTile)selectTile.BaseLayer.Tile).m_Sprites)
                    { lstBaseSprite.Add(sprite); }
                }
                */
                else if (selectTile.BaseLayer.Tile is AnimatedTile animatedTile)
                {
                    foreach (var sprite in animatedTile.m_AnimatedSprites)
                    { lstBaseSprite.Add(sprite); }
                }
                else
                {
                    lstBaseSprite.Add(((Tile)selectTile.BaseLayer.Tile).sprite);
                }
            }
            else
            { selectBaseResource = string.Empty; }

            if (selectTile.OverlayBackLayer != null)
            {
                selectBackResource = selectTile.OverlayBackLayer.Resource;
                if (selectTile.OverlayBackLayer.Tile is IsometricRuleTile tile)
                {
                    lstBackSprite.Add(tile.m_DefaultSprite);
                }
                /*
                else if (selectTile.OverlayBackLayer.Tile is RuleOverrideTile)
                {
                    foreach (var sprite in ((RuleOverrideTile)selectTile.OverlayBackLayer.Tile).m_Sprites)
                    { lstBackSprite.Add(sprite); }
                }
                */
                else if (selectTile.OverlayBackLayer.Tile is AnimatedTile animatedTile)
                {
                    foreach (var sprite in animatedTile.m_AnimatedSprites)
                    { lstBackSprite.Add(sprite); }
                }
                else
                {
                    lstBackSprite.Add(((Tile)selectTile.OverlayBackLayer.Tile).sprite);
                }
            }
            else
            { selectBackResource = string.Empty; }

            if (selectTile.OverlayFrontLayer != null)
            {
                selectFrontResource = selectTile.OverlayFrontLayer.Resource;
                if (selectTile.OverlayFrontLayer.Tile is IsometricRuleTile tile)
                {
                    lstFrontSprite.Add(tile.m_DefaultSprite);
                }
                /*
                else if (selectTile.OverlayFrontLayer.Tile is RuleOverrideTile)
                {
                    foreach (var sprite in ((RuleOverrideTile)selectTile.OverlayFrontLayer.Tile).m_Sprites)
                    { lstFrontSprite.Add(sprite); }
                }
                */
                else if (selectTile.OverlayFrontLayer.Tile is AnimatedTile animatedTile)
                {
                    foreach (var sprite in animatedTile.m_AnimatedSprites)
                    { lstFrontSprite.Add(sprite); }
                }
                else
                {
                    lstFrontSprite.Add(((Tile)selectTile.OverlayFrontLayer.Tile).sprite);
                }
            }
            else
            { selectFrontResource = string.Empty; }
        }

        public void TemporarilySavedTileSet()
        {
            if (selectTiles == null)
            { return; }
            TemporarilySavedTile();
            dictTiles[selectTiles.TileSetId] = selectTiles;
        }

        public void TemporarilySavedTile()
        {
            if (selectTiles == null || selectTile == null)
            { return; }
            selectTiles.Tiles[selectTile.TileId] = selectTile;
        }

        public async void LoadTiles(string path)
        {
            // タイル定義読込
            nameFile = path;
            dictTiles.Clear();
            mngWindow.TileEditorToolBar.TileSetDropdown.choices.Clear();
            mngWindow.TileEditorToolBar.TileDropdown.choices.Clear();
            dictTiles = await DataUtil.LoadTiles(DataUtil.PathBase, Path.Combine(DataUtil.PathBase, "data", "tiles"), path);
            selectTile = null;
            selectTiles = null;
            foreach (var key in dictTiles.Keys)
            { mngWindow.TileEditorToolBar.TileSetDropdown.choices.Add(key); }

            if (dictTiles.Count > 0)
            {
                selectTiles = dictTiles.Values.ToArray()[0];
                mngWindow.TileEditorToolBar.InitializeTile(this, selectTiles.TileSetId, string.Empty);
                if (selectTiles.Tiles.Count > 0)
                {
                    selectTile = selectTiles.Tiles.Values.ToArray()[0];
                    ChangeTile(SelectTiles.Tiles[selectTile.TileId]);
                    if (mngWindow.TileEditorToolBar.Pixel != SelectTiles.Pixels)
                    {
                        mngWindow.TileEditorToolBar.SetPixel(this, SelectTiles.Pixels);
                    }
                    mngWindow.TileEditorToolBar.InitializeTileSetting(this);
                    DisplayTileSample();
                }
            }
        }

        public void SaveTiles()
        {
            var lst = new List<TileJson>();
            TemporarilySavedTileSet();
            foreach (var tiles in dictTiles.Values)
            {
                var chips = new List<TileChipJson>();
                foreach (var t in tiles.Tiles.Values)
                {
                    chips.Add(new TileChipJson()
                    {
                        name = t.TileId,
                        tool_hidden = t.ToolHidden,
                        collision = t.Collision,
                        void_tile = t.VoidTile,
                        suitable = ConvertMoveJson(t.MoveCost),
                        base_layer = ConvertLayerJson(t.BaseLayer),
                        overlay_back = ConvertLayerJson(t.OverlayBackLayer),
                        overlay_front = ConvertLayerJson(t.OverlayFrontLayer)
                    });
                }
                lst.Add(new TileJson()
                {
                    id = tiles.TileSetId,
                    pixels = tiles.Pixels,
                    tile = chips.ToArray()
                });
            }
            DataUtil.SaveData(Path.Combine(DataUtil.PathBase, "data", "tiles", $"{nameFile}.json"), new TilesJson()
            {
                tiles = lst.ToArray()
            });
        }

        private TileMoveTypeJson[] ConvertMoveJson(Dictionary<MoveType, MoveCostData> costs)
        {
            if (costs == null || costs.Count == 0)
            {
                return null;
            }
            var results = new List<TileMoveTypeJson>();
            foreach (var cost in costs.Values)
            {
                results.Add(new TileMoveTypeJson()
                {
                    move_type = CastMoveType(cost.MoveType),
                    cost = cost.MoveCost,
                });
            }
            return results.ToArray();
        }

        private string CastMoveType(MoveType typ)
        {
            switch (typ)
            {
                case MoveType.Space:
                    return "space";
                case MoveType.Air:
                    return "air";
                case MoveType.Ground:
                    return "ground";
                case MoveType.Underwater:
                    return "underwater";
                default:
                    return "space";
            }
        }

        private TileChipLayerJson ConvertLayerJson(LayerData dat)
        {
            if (dat == null)
            {
                return null;
            }

            var pos = new List<Vector2Int>();
            var animSpeed = 0.0f;
            switch (dat.Tile)
            {
                case AnimatedTile datTile:
                    {
                        foreach (var sprite in datTile.m_AnimatedSprites)
                        {
                            pos.Add(Vector2Int.RoundToInt(sprite.rect.position));
                        }
                        animSpeed = datTile.m_MaxSpeed;
                        break;
                    }
                case IsometricRuleTile ruleTile:
                    pos.Add(Vector2Int.RoundToInt(ruleTile.m_DefaultSprite.rect.position));
                    break;
                case RuleOverrideTile overrideTile:
                    pos.Add(Vector2Int.RoundToInt(overrideTile.m_Tile.m_DefaultSprite.rect.position));
                    break;
                default:
                    {
                        var tile = dat.Tile as Tile;
                        if (tile != null)
                        { pos.Add(Vector2Int.RoundToInt(tile.sprite.rect.position)); }
                        break;
                    }
            }
            return new TileChipLayerJson()
            {
                resource = $"{dat.Resource}.png",
                pos = pos.ToArray(),
                color = dat.TileColor,
                anim_speed = animSpeed,
                flip_x = dat.FlipX,
                flip_y = dat.FlipY
            };
        }

        public void SetViewCenter(Vector2Int pos)
        {
            var tilePos = new Vector3Int(pos.x, pos.y, 0);
            var cpos = mngStage.GetCellCenterWorld(tilePos);
            cpos.z = -10;
            camMain.transform.position = cpos;
        }
        
        public int Distance(Vector2Int posBase, Vector2Int posTarget)
        {
            return math.abs(posBase.x - posTarget.x) + math.abs(posBase.y - posTarget.y);
        }

        public bool IsCancel => action.UI.Cancel.triggered;

        public BasicAction Action => action;

        public Dictionary<string, TilesData> TilesDatas => dictTiles;

        public float WaitNavigate
        {
            set => waitNavigate = value;
        }

        public string FileName
        {
            get => nameFile;
            set => nameFile = value;
        }

        public TilesData SelectTiles
        {
            get => selectTiles;
            set => selectTiles = value;
        }
        
        public TileData SelectTile
        {
            get => selectTile;
            set => selectTile = value;
        }

        public List<Sprite> BaseSpriteList
        {
            get => lstBaseSprite;
        }
        public List<Sprite> BackSpriteList
        {
            get => lstBackSprite;
        }
        public List<Sprite> FrontSpriteList
        {
            get => lstFrontSprite;
        }
        public int SelectBaseSprite
        {
            get => selectBaseSprite;
            set => selectBaseSprite = value;
        }

        public int SelectBackSprite
        {
            get => selectBackSprite;
            set => selectBackSprite = value;
        }

        public int SelectFrontSprite
        {
            get => selectFrontSprite;
            set => selectFrontSprite = value;
        }

        public TileEditorWindowManager TileEditorWindowManager
        {
            get => mngWindow;
        }
    }
}
