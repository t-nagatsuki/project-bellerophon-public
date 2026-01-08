using System.Collections.Generic;
using System.IO;
using Functions.Data;
using Functions.Data.Maps;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using TileData = Functions.Data.Maps.TileData;
namespace Functions.UI.TileEditor
{
    public class TileSettingWindow : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset templateTile;

        private UIDocument document;
        private Button btnAccept;
        private Button btnCancel;
        private TextField txtFileName;
        private TextField txtTileSet;
        private TextField txtTile;
        private UnsignedIntegerField intPixels;

        private bool isInitialize;
        private bool isAddTiles;
        private bool isAddTile;
        
        private void Awake()
        {
            document = GetComponent<UIDocument>();
            document.rootVisualElement.style.display = DisplayStyle.None;
            btnAccept = document.rootVisualElement.Q<Button>("BtnAccept");
            btnCancel = document.rootVisualElement.Q<Button>("BtnCancel");
            txtFileName = document.rootVisualElement.Q<TextField>("TxtFileName");
            txtTileSet = document.rootVisualElement.Q<TextField>("TxtTileSet");
            txtTile = document.rootVisualElement.Q<TextField>("TxtTile");
            intPixels = document.rootVisualElement.Q<UnsignedIntegerField>("IntPixels");
        }

        public void SetManager(TileEditorManager _mng)
        {
            btnAccept.clicked += () =>
            {
                var isChangeTileSet = true;
                var isChangeTile = true;
                _mng.FileName = txtFileName.value;
                if (string.IsNullOrWhiteSpace(txtTileSet.value))
                {
                    _mng.TileEditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_S0001", LocaleUtil.GetEntry("lbl_id_tileset")));
                    return;
                }
                if (txtTileSet.value == "－")
                {
                    _mng.TileEditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_C0003", LocaleUtil.GetEntry("lbl_id_tileset")));
                    return;
                }
                if (isAddTiles)
                {
                    if (_mng.TilesDatas.ContainsKey(txtTileSet.value))
                    {
                        _mng.TileEditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_C0002", LocaleUtil.GetEntry("lbl_id_tileset")));
                        return;
                    }

                    _mng.SelectTiles = new TilesData(txtTileSet.value);
                }
                else if (_mng.SelectTiles.TileSetId != txtTileSet.value)
                {
                    _mng.TilesDatas.Remove(_mng.SelectTiles.TileSetId);
                }
                else
                {
                    isChangeTileSet = false;
                }

                _mng.SelectTiles.TileSetId = txtTileSet.value;
                _mng.SelectTiles.Pixels = (int)intPixels.value;
                if (_mng.TileEditorWindowManager.TileEditorToolBar.Pixel != _mng.SelectTiles.Pixels)
                {
                    _mng.TileEditorWindowManager.TileEditorToolBar.SetPixel(_mng, _mng.SelectTiles.Pixels);
                }
                _mng.TilesDatas[txtTileSet.value] = _mng.SelectTiles;
                if (isInitialize)
                {
                    _mng.SelectTiles.Tiles.Clear();
                }

                if (string.IsNullOrWhiteSpace(txtTile.value))
                {
                    _mng.TileEditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_S0001", LocaleUtil.GetEntry("lbl_id_tile")));
                    return;
                }
                if (isAddTile)
                {
                    if (_mng.SelectTiles.Tiles.ContainsKey(txtTile.value))
                    {
                        _mng.TileEditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_C0002", LocaleUtil.GetEntry("lbl_id_tile")));
                        return;
                    }

                    _mng.SelectTile = new TileData(_mng.SelectTiles.TileSetId, txtTile.value)
                    {
                        Collision = true
                    };
                }
                else if (_mng.SelectTile.TileId != txtTile.value)
                {
                    _mng.SelectTiles.Tiles.Remove(_mng.SelectTile.TileId);
                }
                else
                {
                    isChangeTile = false;
                }

                foreach (var tile in _mng.SelectTiles.Tiles.Values)
                {
                    tile.TileSetId = txtTileSet.value;
                }

                _mng.SelectTile.TileSetId = txtTileSet.value;
                _mng.SelectTiles.Tiles[txtTile.value] = _mng.SelectTile;
                if (isChangeTileSet)
                {
                    _mng.TileEditorWindowManager.TileEditorToolBar.InitializeTileSet(
                        _mng, 
                        _mng.SelectTiles.TileSetId,
                        _mng.SelectTile.TileId);
                }
                else if (isChangeTile)
                {
                    _mng.TileEditorWindowManager.TileEditorToolBar.InitializeTile(
                        _mng, 
                        _mng.SelectTiles.TileSetId,
                        _mng.SelectTile.TileId);
                }
                _mng.WaitNavigate = 0;
                document.rootVisualElement.style.display = DisplayStyle.None;
            };
            btnCancel.clicked += () =>
            {
                _mng.WaitNavigate = 0;
                document.rootVisualElement.style.display = DisplayStyle.None;
            };
            intPixels.RegisterValueChangedCallback(v =>
            {
                if (v.newValue < 1)
                { intPixels.value = 1; }
                else if (v.newValue > 512)
                { intPixels.value = 512; }
            });
        }

        public void InitializeDisplay(TileEditorManager _mng)
        {
            isInitialize = true;
            isAddTiles = false;
            isAddTile = true;
            document.rootVisualElement.style.display = DisplayStyle.Flex;
            txtFileName.value = "default_tiles";
            txtTileSet.value = "default_tiles";
            txtTile.value = "default_tile";
            intPixels.value = 32;
        }

        public void AddTilesDisplay(TileEditorManager _mng)
        {
            isInitialize = false;
            isAddTiles = true;
            isAddTile = true;
            document.rootVisualElement.style.display = DisplayStyle.Flex;
            txtTileSet.value = "default_tiles";
            txtFileName.value = _mng.FileName;
            txtTile.value = "default_tile";
            intPixels.value = 32;
        }

        public void AddTileDisplay(TileEditorManager _mng)
        {
            isInitialize = false;
            isAddTiles = false;
            isAddTile = true;
            document.rootVisualElement.style.display = DisplayStyle.Flex;
            txtFileName.value = _mng.FileName;
            txtTileSet.value = _mng.SelectTiles.TileSetId;
            txtTile.value = "default_tile";
            intPixels.value = (uint)_mng.SelectTiles.Pixels;
        }

        public void EditDisplay(TileEditorManager _mng)
        {
            isInitialize = false;
            isAddTiles = false;
            isAddTile = false;
            document.rootVisualElement.style.display = DisplayStyle.Flex;
            txtFileName.value = _mng.FileName;
            txtTileSet.value = _mng.SelectTiles.TileSetId;
            txtTile.value = _mng.SelectTile.TileId;
            intPixels.value = (uint)_mng.SelectTiles.Pixels;
        }

        public bool IsDisplay()
        {
            return document.rootVisualElement.style.display == DisplayStyle.Flex;
        }
    }
}
