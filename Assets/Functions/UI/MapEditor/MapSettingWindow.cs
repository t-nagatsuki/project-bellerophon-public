using System.Collections.Generic;
using System.IO;
using System.Linq;
using Functions.Data;
using Functions.Data.Maps;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using TileData = Functions.Data.Maps.TileData;
namespace Functions.UI.MapEditor
{
    public class MapSettingWindow : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset templateTile;

        private UIDocument document;
        private Button btnAccept;
        private Button btnCancel;
        private TextField txtFileName;
        private TextField txtMapId;
        private DropdownField drpMusic;
        private UnsignedIntegerField intMapSizeX;
        private UnsignedIntegerField intMapSizeY;
        private DropdownField drpTileSet;
        private VisualElement divTiles;

        private TileData selectTile;
        private bool isAddMap;
        public List<MapTileData> tiles;

        private void Awake()
        {
            document = GetComponent<UIDocument>();
            document.rootVisualElement.style.display = DisplayStyle.None;
            btnAccept = document.rootVisualElement.Q<Button>("BtnAccept");
            btnCancel = document.rootVisualElement.Q<Button>("BtnCancel");
            txtFileName = document.rootVisualElement.Q<TextField>("TxtFileName");
            txtMapId = document.rootVisualElement.Q<TextField>("TxtMapId");
            drpMusic = document.rootVisualElement.Q<DropdownField>("DrpMusic");
            intMapSizeX = document.rootVisualElement.Q<UnsignedIntegerField>("IntMapSizeX");
            intMapSizeY = document.rootVisualElement.Q<UnsignedIntegerField>("IntMapSizeY");
            drpTileSet = document.rootVisualElement.Q<DropdownField>("DrpTileSet");
            divTiles = document.rootVisualElement.Q<VisualElement>("DivTiles");
        }

        public void SetManager(MapEditorManager mng)
        {
            btnAccept.clicked += () =>
            {
                var map = new MapData();
                if (string.IsNullOrWhiteSpace(txtMapId.value))
                {
                    mng.MapEditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_S0001", LocaleUtil.GetEntry("lbl_id_map")));
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtFileName.value))
                {
                    mng.MapEditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_S0001", LocaleUtil.GetEntry("lbl_name_file")));
                    return;
                }
                map.MapId = txtMapId.value;
                map.Max = new Vector2Int((int)intMapSizeX.value, (int)intMapSizeY.value);
                map.Tiles = tiles.Where(t => t.Position.x < map.Max.x && t.Position.y < map.Max.y).ToList();
                map.Music = drpMusic.value;
                if (selectTile != null)
                {
                    map.TileSet = selectTile.TileSetId;
                    map.InitTile = selectTile.TileId;
                }
                mng.FileName = txtFileName.value;
                if (isAddMap)
                {
                    if (mng.Maps.ContainsKey(txtMapId.value))
                    {
                        mng.MapEditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_C0002", LocaleUtil.GetEntry("lbl_id_map")));
                        return;
                    }
                    mng.InitializeMap(map);
                }
                else
                {
                    mng.InitializeMaps(map);
                }
                mng.WaitNavigate = 0;
                document.rootVisualElement.style.display = DisplayStyle.None;
            };
            btnCancel.clicked += () =>
            {
                mng.WaitNavigate = 0;
                document.rootVisualElement.style.display = DisplayStyle.None;
            };
            intMapSizeX.RegisterValueChangedCallback(v =>
            {
                if (v.newValue < 1)
                { intMapSizeX.value = 1; }
                else if (v.newValue > 99)
                { intMapSizeX.value = 99; }
            });
            intMapSizeY.RegisterValueChangedCallback(v =>
            {
                if (v.newValue < 1)
                { intMapSizeY.value = 1; }
                else if (v.newValue > 99)
                { intMapSizeY.value = 99; }
            });
            foreach (var path in DataUtil.GetAudios("*.ogg"))
            { drpMusic.choices.Add(Path.GetFileName(path)); }
            foreach (var path in DataUtil.GetAudios("*.mp3"))
            { drpMusic.choices.Add(Path.GetFileName(path)); }
            drpTileSet.choices.Add("－");
            foreach (var key in mng.TilesDatas.Keys)
            { drpTileSet.choices.Add(key); }
            drpTileSet.index = 0;
            drpTileSet.RegisterValueChangedCallback(v =>
            {
                InitializeTileSet(mng, v.newValue);
                mng.WaitNavigate = 0;
            });
        }

        private void InitializeTileSet(MapEditorManager mng, string tileSet)
        {
            divTiles.Clear();
            if (tileSet.Equals("－"))
            {
                selectTile = null;
                return;
            }
            foreach (var tile in mng.TilesDatas[tileSet].Tiles.Values)
            {
                var btn = templateTile.Instantiate();
                var elmtBtn = btn.Q<Button>("BtnTile");
                var divBack = btn.Q<VisualElement>("DivBack");
                var divFront = btn.Q<VisualElement>("DivFront");
                SetStyle(elmtBtn, tile.BaseLayer);
                SetStyle(divBack, tile.OverlayBackLayer);
                SetStyle(divFront, tile.OverlayFrontLayer);
                elmtBtn.clicked += () =>
                {
                    selectTile = tile;
                    UnselectTile();
                    elmtBtn.style.borderBottomColor = Color.green;
                    elmtBtn.style.borderLeftColor = Color.green;
                    elmtBtn.style.borderRightColor = Color.green;
                    elmtBtn.style.borderTopColor = Color.green;
                    mng.WaitNavigate = 0;
                };
                if (selectTile == tile)
                {
                    elmtBtn.style.borderBottomColor = Color.green;
                    elmtBtn.style.borderLeftColor = Color.green;
                    elmtBtn.style.borderRightColor = Color.green;
                    elmtBtn.style.borderTopColor = Color.green;
                }
                divTiles.Add(elmtBtn);
            }
        }

        public void UnselectTile()
        {
            foreach (var elm in divTiles.Children())
            {
                elm.style.borderBottomColor = Color.black;
                elm.style.borderLeftColor = Color.black;
                elm.style.borderRightColor = Color.black;
                elm.style.borderTopColor = Color.black;
            }
        }

        private void SetStyle(VisualElement btn, LayerData dat)
        {
            if (dat != null)
            {
                if (dat.Tile is AnimatedTile)
                {
                    btn.style.backgroundImage = new StyleBackground(((AnimatedTile)dat.Tile).m_AnimatedSprites[0]);
                }
                else if (dat.Tile is IsometricRuleTile)
                {
                    btn.style.backgroundImage = new StyleBackground(((IsometricRuleTile)dat.Tile).m_DefaultSprite);
                }
                else if (dat.Tile is RuleOverrideTile)
                {
                    btn.style.backgroundImage = new StyleBackground(((RuleOverrideTile)dat.Tile).m_Tile.m_DefaultSprite);
                }
                else
                {
                    btn.style.backgroundImage = new StyleBackground(((Tile)dat.Tile).sprite);
                    btn.style.unityBackgroundImageTintColor = ((Tile)dat.Tile).color;
                }
                if (dat.FlipX)
                {
                    btn.resolvedStyle.translate.Scale(new Vector3(-1, 1, 1));
                }
            }
        }

        public void InitializeDisplay(MapEditorManager mng)
        {
            isAddMap = false;
            document.rootVisualElement.style.display = DisplayStyle.Flex;
            txtFileName.value = "default_map";
            txtMapId.value = "default_map";
            drpMusic.index = -1;
            intMapSizeX.value = 10;
            intMapSizeY.value = 10;
            selectTile = null;
            if (drpTileSet.choices.Count > 0)
            { drpTileSet.index = 0; }
            else
            { drpTileSet.index = -1; }
            InitializeTileSet(mng, drpTileSet.value);
            tiles = new List<MapTileData>();
        }

        public void AddMapDisplay(MapEditorManager mng)
        {
            isAddMap = true;
            document.rootVisualElement.style.display = DisplayStyle.Flex;
            txtFileName.value = mng.FileName;
            txtMapId.value = "default_map";
            drpMusic.index = -1;
            intMapSizeX.value = 10;
            intMapSizeY.value = 10;
            selectTile = null;
            if (drpTileSet.choices.Count > 0)
            { drpTileSet.index = 0; }
            else
            { drpTileSet.index = -1; }
            InitializeTileSet(mng, drpTileSet.value);
            tiles = new List<MapTileData>();
        }

        public void EditMapDisplay(MapEditorManager mng)
        {
            isAddMap = true;
            document.rootVisualElement.style.display = DisplayStyle.Flex;
            txtFileName.value = mng.FileName;
            var map = mng.MapData;
            txtMapId.value = map.MapId;
            drpMusic.value = map.Music;
            intMapSizeX.value = (uint)map.Max.x;
            intMapSizeY.value = (uint)map.Max.y;
            if (mng.TilesDatas.ContainsKey(map.TileSet))
            {
                drpTileSet.value = map.TileSet;
                if (mng.TilesDatas[map.TileSet].Tiles.ContainsKey(map.InitTile))
                { selectTile = mng.TilesDatas[map.TileSet].Tiles[map.InitTile]; }
                else
                { selectTile = null; }
            }
            InitializeTileSet(mng, drpTileSet.value);
            tiles = mng.MapData.Tiles;
        }

        public bool IsDisplay()
        {
            return document.rootVisualElement.style.display == DisplayStyle.Flex;
        }
    }
}
