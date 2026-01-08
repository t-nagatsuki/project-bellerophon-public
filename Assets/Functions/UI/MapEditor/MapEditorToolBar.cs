using System.Collections.Generic;
using Functions.Data;
using Functions.Data.Maps;
using Functions.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using TileData = Functions.Data.Maps.TileData;
using Toggle = UnityEngine.UIElements.Toggle;

namespace Functions.UI.MapEditor
{
    public class MapEditorToolBar : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset templateDisplay;
        [SerializeField] private VisualTreeAsset templateTile;

        private UIDocument document;
        private VisualElement divTopToolbar;
        private VisualElement divToolWindow;
        private Button btnNew;
        private Button btnSave;
        private Button btnLoad;
        private Button btnSettings;
        private Button btnAddMap;
        private Button btnDeleteMap;
        private Button btnClose;
        private SliderInt slideHeight;
        private Toggle tglGrid;
        private Toggle tglCollision;
        private Toggle tglMusic;
        private DropdownField drpMapId;
        private DropdownField drpTileset;
        private VisualElement divTiles;
        private VisualElement divDebug;
        private Dictionary<string, TemplateContainer> dictData;
        private string selectTileSet;
        private string selectTile;

        private bool isOverTopToolbar;
        private bool isOverToolWindow;

        private void Awake()
        {
            dictData = new Dictionary<string, TemplateContainer>();
            document = GetComponent<UIDocument>();
            divTopToolbar = document.rootVisualElement.Q<VisualElement>("div-top-toolbar");
            divToolWindow = document.rootVisualElement.Q<VisualElement>("div-tool-window");
            btnNew = document.rootVisualElement.Q<Button>("BtnNew");
            btnSave = document.rootVisualElement.Q<Button>("BtnSave");
            btnLoad = document.rootVisualElement.Q<Button>("BtnLoad");
            btnAddMap = document.rootVisualElement.Q<Button>("BtnAddMap");
            btnDeleteMap = document.rootVisualElement.Q<Button>("BtnDeleteMap");
            btnSettings = document.rootVisualElement.Q<Button>("BtnSettings");
            btnClose = document.rootVisualElement.Q<Button>("BtnClose");
            slideHeight = document.rootVisualElement.Q<SliderInt>("SlideHeight");
            tglGrid = document.rootVisualElement.Q<Toggle>("TglGrid");
            tglCollision = document.rootVisualElement.Q<Toggle>("TglCollision");
            tglMusic = document.rootVisualElement.Q<Toggle>("TglMusic");
            drpMapId = document.rootVisualElement.Q<DropdownField>("DrpMapId");
            drpTileset = document.rootVisualElement.Q<DropdownField>("DrpTileSet");
            divTiles = document.rootVisualElement.Q<VisualElement>("DivTiles");
            divDebug = document.rootVisualElement.Q<VisualElement>("DivDebug");
        }

        public void SetManager(MapEditorManager _mng)
        {
            divTopToolbar.RegisterCallback<MouseOverEvent>(v =>
            {
                isOverTopToolbar = true;
            });
            divTopToolbar.RegisterCallback<MouseOutEvent>(v =>
            {
                isOverTopToolbar = false;
            });
            divToolWindow.RegisterCallback<MouseOverEvent>(v =>
            {
                isOverToolWindow = true;
            });
            divToolWindow.RegisterCallback<MouseOutEvent>(v =>
            {
                isOverToolWindow = false;
            });

            _mng.MapEditorWindowManager.MapSettingWindow.SetManager(_mng);
            btnNew.clicked += () =>
            {
                _mng.MapEditorWindowManager.MapSettingWindow.InitializeDisplay(_mng);
                _mng.WaitNavigate = 0;
            };
            btnSave.clicked += () =>
            {
                _mng.SaveMap();
                _mng.WaitNavigate = 0;

            };
            btnLoad.clicked += () =>
            {
                _mng.MapEditorWindowManager.MapLoadWindow.InitializeDisplay( _mng);
                _mng.MapEditorWindowManager.MapLoadWindow.VisibleDisplay();
                _mng.WaitNavigate = 0;
            };
            btnSettings.clicked += () =>
            {
                _mng.MapEditorWindowManager.MapSettingWindow.EditMapDisplay(_mng);
                _mng.WaitNavigate = 0;
            };
            drpMapId.RegisterValueChangedCallback(v =>
            {
                _mng.SetMap(v.newValue);
            });
            btnAddMap.clicked += () =>
            {
                _mng.MapEditorWindowManager.MapSettingWindow.AddMapDisplay(_mng);
            };
            btnDeleteMap.clicked += _mng.DeleteMap;
            btnClose.clicked += () =>
            {
                SceneManager.LoadScene(0);
            };
            slideHeight.RegisterValueChangedCallback(v =>
            {
                _mng.SetHeight(v.newValue);
                _mng.WaitNavigate = 0;
            });
            tglGrid.RegisterValueChangedCallback(v =>
            {
                _mng.SetDisplayGrid(v.newValue);
                _mng.WaitNavigate = 0;
            });
            tglCollision.RegisterValueChangedCallback(v =>
            {
                _mng.SetDisplayCollision(v.newValue);
                _mng.WaitNavigate = 0;
            });
            tglMusic.RegisterValueChangedCallback(v =>
            {
                _mng.SetPlayMusic(v.newValue);
                _mng.WaitNavigate = 0;
            });
            foreach (var key in _mng.TilesDatas.Keys)
            { drpTileset.choices.Add(key); }
            if (drpTileset.choices.Count > 0)
            {
                drpTileset.index = 0;
                InitializeTileSet(_mng, drpTileset.value);
            }
            drpTileset.RegisterValueChangedCallback(v =>
            {
                _mng.SelectTile = null;
                InitializeTileSet(_mng, v.newValue);
                _mng.WaitNavigate = 0;
            });
        }

        private void InitializeTileSet(MapEditorManager _mng, string _tileSet)
        {
            divTiles.Clear();
            if (selectTileSet != _tileSet)
            { selectTile = null; }
            foreach (var tile in _mng.TilesDatas[_tileSet].Tiles.Values)
            {
                if (tile.ToolHidden)
                { continue; }
                AddTile(_mng, tile);
            }
        }

        private void AddTile(MapEditorManager _mng, TileData tile)
        {
            var btn = templateTile.Instantiate();
            var elmtBtn = btn.Q<Button>("BtnTile");
            elmtBtn.name = tile.TileId;
            elmtBtn.tooltip = tile.TileId;
            var divBack = btn.Q<VisualElement>("DivBack");
            var divFront = btn.Q<VisualElement>("DivFront");
            SetStyle(elmtBtn, tile.BaseLayer);
            SetStyle(divBack, tile.OverlayBackLayer);
            SetStyle(divFront, tile.OverlayFrontLayer);
            elmtBtn.RegisterCallback<ClickEvent>(v => 
            {
                UnselectTile();
                elmtBtn.style.borderBottomColor = Color.green;
                elmtBtn.style.borderLeftColor = Color.green;
                elmtBtn.style.borderRightColor = Color.green;
                elmtBtn.style.borderTopColor = Color.green;
                selectTile = tile.TileId;
                _mng.SelectTile = tile;
                _mng.WaitNavigate = 0;
            });
            if (selectTile == tile.TileId)
            {
                elmtBtn.style.borderBottomColor = Color.green;
                elmtBtn.style.borderLeftColor = Color.green;
                elmtBtn.style.borderRightColor = Color.green;
                elmtBtn.style.borderTopColor = Color.green;
                selectTile = tile.TileId;
                _mng.SelectTile = tile;
            }
            divTiles.Add(elmtBtn);        }

        public void SelectTile(TileData _dat)
        {
            selectTileSet = _dat.TileSetId;
            selectTile = _dat.TileId;
            if (drpTileset.value != _dat.TileSetId)
            {
                drpTileset.value = _dat.TileSetId;
            }
            else
            {
                foreach (var elm in divTiles.Children())
                {
                    if (elm.name == selectTile)
                    {
                        elm.style.borderBottomColor = Color.green;
                        elm.style.borderLeftColor = Color.green;
                        elm.style.borderRightColor = Color.green;
                        elm.style.borderTopColor = Color.green;
                    }
                    else
                    {
                        elm.style.borderBottomColor = Color.black;
                        elm.style.borderLeftColor = Color.black;
                        elm.style.borderRightColor = Color.black;
                        elm.style.borderTopColor = Color.black;
                    }
                }
            }
        }

        public void UnselectTile()
        {
            selectTileSet = null;
            selectTile = null;
            foreach (var elm in divTiles.Children())
            {
                elm.style.borderBottomColor = Color.black;
                elm.style.borderLeftColor = Color.black;
                elm.style.borderRightColor = Color.black;
                elm.style.borderTopColor = Color.black;
            }
        }

        private void SetStyle(VisualElement _btn, LayerData _dat)
        {
            if (_dat != null)
            {
                if (_dat.Tile is AnimatedTile)
                {
                    _btn.style.backgroundImage = new StyleBackground(((AnimatedTile)_dat.Tile).m_AnimatedSprites[0]);
                }
                else if (_dat.Tile is IsometricRuleTile)
                {
                    _btn.style.backgroundImage = new StyleBackground(((IsometricRuleTile)_dat.Tile).m_DefaultSprite);
                }
                else if (_dat.Tile is RuleOverrideTile)
                {
                    _btn.style.backgroundImage = new StyleBackground(((RuleOverrideTile)_dat.Tile).m_Tile.m_DefaultSprite);
                }
                else
                {
                    _btn.style.backgroundImage = new StyleBackground(((Tile)_dat.Tile).sprite);
                    _btn.style.unityBackgroundImageTintColor = ((Tile)_dat.Tile).color;
                }
                if (_dat.FlipX)
                {
                    _btn.transform.scale = new Vector3(-1, 1, 1);
                }
            }
            else
            {
                _btn.style.backgroundImage = null;
            }
        }
        
        private Texture2D CombineTexutes(Texture2D _textureA, Texture2D _textureB)
        {
            if (_textureA == null)
            { return _textureB; }
            if (_textureB == null)
            { return _textureA; }
            var textureResult = new Texture2D(_textureA.width, _textureA.height);
            textureResult.SetPixels(_textureA.GetPixels());
            for (var x = 0; x<_textureB.width; x++)
            {
                for (var y = 0; y<_textureB.height; y++)
                {
                    Color32 c = _textureB.GetPixel(x, y);
                    if (c.a > 0.0f)
                    {
                        textureResult.SetPixel(x, y, c);
                    }
                }
            }
            textureResult.Apply();
            return textureResult;
        }

        public void SetHeight(int _height)
        {
            slideHeight.value = _height;
        }

        public void SetData(string key, string value)
        {
            if (!dictData.ContainsKey(key))
            {
                var dat = templateDisplay.Instantiate();
                dat.Q<Label>("LblData").text = key;
                dictData[key] = dat;
                divDebug.Add(dat);
            }
            dictData[key].Q<Label>("TxtData").text = value;
        }

        public int NowHeight => slideHeight.value;

        public DropdownField MapIdDropdown => drpMapId;

        public Toggle Grid => tglGrid;

        public Toggle Collision => tglCollision;

        public Toggle Music => tglMusic;

        public bool IsOver => isOverTopToolbar || isOverToolWindow;
    }
}
