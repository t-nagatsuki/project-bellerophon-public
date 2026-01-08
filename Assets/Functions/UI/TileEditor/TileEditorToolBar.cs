using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Functions.Data;
using Functions.Data.Maps;
using Functions.Enum;
using Functions.Manager;
using Functions.Util;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Functions.UI.TileEditor
{
    public class TileEditorToolBar : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset templateTile;
        [SerializeField] private Texture2D noneImage;

        private UIDocument document;
        private VisualElement divTopToolbar;
        private VisualElement divToolWindow;
        private Button btnNew;
        private Button btnSave;
        private Button btnLoad;
        private Button btnSettings;
        private Button btnAddTileSet;
        private Button btnDeleteTileSet;
        private Button btnAddTile;
        private Button btnDeleteTile;
        private Button btnClose;
        private DropdownField drpTileSet;
        private DropdownField drpTile;
        private DropdownField drpResource;
        private VisualElement divTiles;

        private Toggle tglHidden;
        private Toggle tglCollision;
        private Toggle tglSpace;
        private FloatField fltSpace;
        private Toggle tglAir;
        private FloatField fltAir;
        private Toggle tglGround;
        private FloatField fltGround;
        private Toggle tglUnderwater;
        private FloatField fltUnderwater;
        private IntegerField intBaseColorA;
        private IntegerField intBaseColorR;
        private IntegerField intBaseColorG;
        private IntegerField intBaseColorB;
        private IntegerField intBackColorA;
        private IntegerField intBackColorR;
        private IntegerField intBackColorG;
        private IntegerField intBackColorB;
        private IntegerField intFrontColorA;
        private IntegerField intFrontColorR;
        private IntegerField intFrontColorG;
        private IntegerField intFrontColorB;
        private Toggle tglBaseFlipX;
        private Toggle tglBackFlipX;
        private Toggle tglFrontFlipX;
        private FloatField fltBaseAnimation;
        private FloatField fltBackAnimation;
        private FloatField fltFrontAnimation;

        private Button btnDisplayBase;
        private Button btnDisplayBack;
        private Button btnDisplayFront;

        private Label lblBaseNums;
        private Label lblBackNums;
        private Label lblFrontNums;

        private Button btnBackSelectBase;
        private Button btnNextSelectBase;
        private Button btnDeleteSelectBase;

        private Button btnBackSelectBack;
        private Button btnNextSelectBack;
        private Button btnDeleteSelectBack;

        private Button btnBackSelectFront;
        private Button btnNextSelectFront;
        private Button btnDeleteSelectFront;

        private bool isOverTopToolbar;
        private bool isOverToolWindow;
        private int pixel = 32;

        private int selectLayer;
        private LayerData layerBase;
        private LayerData layerBack;
        private LayerData layerFront;

        private void Awake()
        {
            document = GetComponent<UIDocument>();
            divTopToolbar = document.rootVisualElement.Q<VisualElement>("div-top-toolbar");
            divToolWindow = document.rootVisualElement.Q<VisualElement>("div-tool-window");
            btnNew = document.rootVisualElement.Q<Button>("BtnNew");
            btnSave = document.rootVisualElement.Q<Button>("BtnSave");
            btnLoad = document.rootVisualElement.Q<Button>("BtnLoad");
            btnSettings = document.rootVisualElement.Q<Button>("BtnSettings");
            btnAddTileSet = document.rootVisualElement.Q<Button>("BtnAddTileSet");
            btnDeleteTileSet = document.rootVisualElement.Q<Button>("BtnDeleteTileSet");
            btnAddTile = document.rootVisualElement.Q<Button>("BtnAddTile");
            btnDeleteTile = document.rootVisualElement.Q<Button>("BtnDeleteTile");
            btnClose = document.rootVisualElement.Q<Button>("BtnClose");
            drpTileSet = document.rootVisualElement.Q<DropdownField>("DrpTileSet");
            drpTile = document.rootVisualElement.Q<DropdownField>("DrpTile");
            drpResource = document.rootVisualElement.Q<DropdownField>("DrpResource");
            divTiles = document.rootVisualElement.Q<VisualElement>("DivTiles");
            btnDisplayBase = document.rootVisualElement.Q<Button>("BtnDisplayBase");
            btnDisplayBack = document.rootVisualElement.Q<Button>("BtnDisplayBack");
            btnDisplayFront = document.rootVisualElement.Q<Button>("BtnDisplayFront");
            lblBaseNums = document.rootVisualElement.Q<Label>("LblBaseNums");
            lblBackNums = document.rootVisualElement.Q<Label>("LblBackNums");
            lblFrontNums = document.rootVisualElement.Q<Label>("LblFrontNums");
            
            tglHidden = document.rootVisualElement.Q<Toggle>("TglHidden");
            tglCollision = document.rootVisualElement.Q<Toggle>("TglCollision");
            tglSpace = document.rootVisualElement.Q<Toggle>("TglSpace");
            fltSpace = document.rootVisualElement.Q<FloatField>("FltSpace");
            tglAir = document.rootVisualElement.Q<Toggle>("TglAir");
            fltAir = document.rootVisualElement.Q<FloatField>("FltAir");
            tglGround = document.rootVisualElement.Q<Toggle>("TglGround");
            fltGround = document.rootVisualElement.Q<FloatField>("FltGround");
            tglUnderwater = document.rootVisualElement.Q<Toggle>("TglUnderwater");
            fltUnderwater = document.rootVisualElement.Q<FloatField>("FltUnderwater");
            intBaseColorA = document.rootVisualElement.Q<IntegerField>("IntBaseColorA");
            intBaseColorR = document.rootVisualElement.Q<IntegerField>("IntBaseColorR");
            intBaseColorG = document.rootVisualElement.Q<IntegerField>("IntBaseColorG");
            intBaseColorB = document.rootVisualElement.Q<IntegerField>("IntBaseColorB");
            intBackColorA = document.rootVisualElement.Q<IntegerField>("IntBackColorA");
            intBackColorR = document.rootVisualElement.Q<IntegerField>("IntBackColorR");
            intBackColorG = document.rootVisualElement.Q<IntegerField>("IntBackColorG");
            intBackColorB = document.rootVisualElement.Q<IntegerField>("IntBackColorB");
            intFrontColorA = document.rootVisualElement.Q<IntegerField>("IntFrontColorA");
            intFrontColorR = document.rootVisualElement.Q<IntegerField>("IntFrontColorR");
            intFrontColorG = document.rootVisualElement.Q<IntegerField>("IntFrontColorG");
            intFrontColorB = document.rootVisualElement.Q<IntegerField>("IntFrontColorB");
            tglBaseFlipX = document.rootVisualElement.Q<Toggle>("TglBaseFlipX");
            tglBackFlipX = document.rootVisualElement.Q<Toggle>("TglBackFlipX");
            tglFrontFlipX = document.rootVisualElement.Q<Toggle>("TglFrontFlipX");
            fltBaseAnimation = document.rootVisualElement.Q<FloatField>("FltBaseAnimation");
            fltBackAnimation = document.rootVisualElement.Q<FloatField>("FltBackAnimation");
            fltFrontAnimation = document.rootVisualElement.Q<FloatField>("FltFrontAnimation");

            btnBackSelectBase = document.rootVisualElement.Q<Button>("BtnBackSelectBase");
            btnNextSelectBase = document.rootVisualElement.Q<Button>("BtnNextSelectBase");
            btnDeleteSelectBase = document.rootVisualElement.Q<Button>("BtnDeleteSelectBase");

            btnBackSelectBack = document.rootVisualElement.Q<Button>("BtnBackSelectBack");
            btnNextSelectBack = document.rootVisualElement.Q<Button>("BtnNextSelectBack");
            btnDeleteSelectBack = document.rootVisualElement.Q<Button>("BtnDeleteSelectBack");

            btnBackSelectFront = document.rootVisualElement.Q<Button>("BtnBackSelectFront");
            btnNextSelectFront = document.rootVisualElement.Q<Button>("BtnNextSelectFront");
            btnDeleteSelectFront = document.rootVisualElement.Q<Button>("BtnDeleteSelectFront");

            foreach (var path in DataUtil.GetImages("map", "*.png"))
            { drpResource.choices.Add(Path.GetFileNameWithoutExtension(path)); }
        }

        public void SetManager(TileEditorManager mng)
        {
            DisplaySelectSprite(mng);
            // UI領域の識別
            divTopToolbar.RegisterCallback<MouseOverEvent>(_ =>
            {
                isOverTopToolbar = true;
            });
            divTopToolbar.RegisterCallback<MouseOutEvent>(_ =>
            {
                isOverTopToolbar = false;
            });
            divToolWindow.RegisterCallback<MouseOverEvent>(_ =>
            {
                isOverToolWindow = true;
            });
            divToolWindow.RegisterCallback<MouseOutEvent>(_ =>
            {
                isOverToolWindow = false;
            });

            // ボタンイベント設定
            mng.TileEditorWindowManager.TileSettingWindow.SetManager(mng);
            btnNew.clicked += () =>
            {
                mng.TileEditorWindowManager.TileSettingWindow.InitializeDisplay(mng);
            };
            btnSave.clicked += mng.SaveTiles;
            btnLoad.clicked += () =>
            {
                mng.TileEditorWindowManager.TileLoadWindow.InitializeDisplay(mng);
                mng.TileEditorWindowManager.TileLoadWindow.VisibleDisplay();
            };
            btnSettings.clicked += () =>
            {
                mng.TileEditorWindowManager.TileSettingWindow.EditDisplay(mng);
            };
            btnAddTileSet.clicked += () =>
            {
                mng.TileEditorWindowManager.TileSettingWindow.AddTilesDisplay(mng);
            };
            btnDeleteTileSet.clicked += () =>
            {
                if (mng.TilesDatas.Count < 2)
                { return; }
                mng.TilesDatas.Remove(mng.SelectTiles.TileSetId);
                mng.SelectTiles = mng.TilesDatas[mng.TilesDatas.Keys.ToArray()[0]];
                mng.SelectTile = mng.SelectTiles.Tiles[mng.SelectTiles.Tiles.Keys.ToArray()[0]];
                drpTileSet.value = mng.SelectTiles.TileSetId;
                InitializeTileSet(mng, drpTileSet.value, mng.SelectTile.TileId);
            };
            btnAddTile.clicked += () =>
            {
                mng.TileEditorWindowManager.TileSettingWindow.AddTileDisplay(mng);
            };
            btnDeleteTile.clicked += () =>
            {
                if (mng.SelectTiles.Tiles.Count < 2)
                { return; }
                mng.SelectTiles.Tiles.Remove(mng.SelectTile.TileId);
                mng.SelectTile = mng.SelectTiles.Tiles[mng.SelectTiles.Tiles.Keys.ToArray()[0]];
                InitializeTile(mng, mng.SelectTiles.TileSetId, mng.SelectTile.TileId);
            };
            btnClose.clicked += () =>
            {
                SceneManager.LoadScene(0);
            };
            foreach (var key in mng.TilesDatas.Keys)
            { drpTileSet.choices.Add(key); }
            drpTileSet.RegisterValueChangedCallback(v =>
            {
                if (String.IsNullOrWhiteSpace(v.newValue) || !mng.TilesDatas.ContainsKey(v.newValue))
                { return; }
                mng.TemporarilySavedTileSet();
                mng.SelectTiles = mng.TilesDatas[v.newValue];
                InitializeTile(mng, v.newValue, string.Empty);
                mng.ChangeTile(mng.SelectTiles.Tiles[drpTile.value]);
                if (pixel != mng.SelectTiles.Pixels)
                {
                    SetPixel(mng, mng.SelectTiles.Pixels);
                }
                InitializeTileSetting(mng);
                mng.DisplayTileSample();
            });
            drpTile.RegisterValueChangedCallback(v =>
            {
                if (String.IsNullOrWhiteSpace(v.newValue) || !mng.SelectTiles.Tiles.ContainsKey(v.newValue))
                { return; }
                mng.TemporarilySavedTile();
                mng.ChangeTile(mng.SelectTiles.Tiles[v.newValue]);
                InitializeTileSetting(mng);
                mng.DisplayTileSample();
            });
            drpResource.RegisterValueChangedCallback(v =>
            {
                LoadingResource(mng, v.newValue);
            });
            if (drpResource.choices.Count > 0)
            { drpResource.index = 0; }
            
            btnDisplayBase.clicked += () =>
            {
                selectLayer = 1;
                SetBorder(btnDisplayBase, Color.green);
                SetBorder(btnDisplayBack, Color.black);
                SetBorder(btnDisplayFront, Color.black);
            };
            btnDisplayBack.clicked += () =>
            {
                selectLayer = 2;
                SetBorder(btnDisplayBase, Color.black);
                SetBorder(btnDisplayBack, Color.green);
                SetBorder(btnDisplayFront, Color.black);
            };
            btnDisplayFront.clicked += () =>
            {
                selectLayer = 3;
                SetBorder(btnDisplayBase, Color.black);
                SetBorder(btnDisplayBack, Color.black);
                SetBorder(btnDisplayFront, Color.green);
            };
            
            // レイヤー操作
            btnBackSelectBase.clicked += () =>
            {
                mng.SelectBaseSprite = math.clamp(mng.SelectBaseSprite - 1, 0, mng.BaseSpriteList.Count);
                DisplaySelectSprite(mng);
            };
            btnNextSelectBase.clicked += () =>
            {
                mng.SelectBaseSprite = math.clamp(mng.SelectBaseSprite + 1, 0, mng.BaseSpriteList.Count);
                DisplaySelectSprite(mng);
            };
            btnDeleteSelectBase.clicked += () =>
            {
                mng.RemoveSprite(1);
            };
            
            btnBackSelectBack.clicked += () =>
            {
                mng.SelectBackSprite = math.clamp(mng.SelectBackSprite - 1, 0, mng.BackSpriteList.Count);
                DisplaySelectSprite(mng);
            };
            btnNextSelectBack.clicked += () =>
            {
                mng.SelectBackSprite = math.clamp(mng.SelectBackSprite + 1, 0, mng.BackSpriteList.Count);
                DisplaySelectSprite(mng);
            };
            btnDeleteSelectBack.clicked += () =>
            {
                mng.RemoveSprite(2);
            };
            
            btnBackSelectFront.clicked += () =>
            {
                mng.SelectFrontSprite = math.clamp(mng.SelectFrontSprite - 1, 0, mng.FrontSpriteList.Count);
                DisplaySelectSprite(mng);
            };
            btnNextSelectFront.clicked += () =>
            {
                mng.SelectFrontSprite = math.clamp(mng.SelectFrontSprite + 1, 0, mng.FrontSpriteList.Count);
                DisplaySelectSprite(mng);
            };
            btnDeleteSelectFront.clicked += () =>
            {
                mng.RemoveSprite(3);
            };
            
            // タイル設定バリデーション
            tglHidden.RegisterValueChangedCallback(v =>
            {
                mng.SelectTile.ToolHidden = v.newValue;
            });
            tglCollision.RegisterValueChangedCallback(v =>
            {
                mng.SelectTile.Collision = v.newValue;
            });
            tglSpace.RegisterValueChangedCallback(v => SetMoveCost(mng, v.newValue, MoveType.Space, fltSpace.value));
            fltSpace.RegisterValueChangedCallback(v => SetMoveCost(mng, tglSpace.value, MoveType.Space, v.newValue));
            tglAir.RegisterValueChangedCallback(v => SetMoveCost(mng, v.newValue, MoveType.Air, fltAir.value));
            fltAir.RegisterValueChangedCallback(v => SetMoveCost(mng, tglAir.value, MoveType.Air, v.newValue));
            tglGround.RegisterValueChangedCallback(v => SetMoveCost(mng, v.newValue, MoveType.Ground, fltGround.value));
            fltGround.RegisterValueChangedCallback(v => SetMoveCost(mng, tglGround.value, MoveType.Ground, v.newValue));
            tglUnderwater.RegisterValueChangedCallback(v => SetMoveCost(mng, v.newValue, MoveType.Underwater, fltUnderwater.value));
            fltUnderwater.RegisterValueChangedCallback(v => SetMoveCost(mng, tglUnderwater.value, MoveType.Underwater, v.newValue));
            intBaseColorA.RegisterValueChangedCallback(v => SetBaseLayerColor(mng, v.newValue, null, null, null));
            intBaseColorR.RegisterValueChangedCallback(v => SetBaseLayerColor(mng, null, v.newValue, null, null));
            intBaseColorG.RegisterValueChangedCallback(v => SetBaseLayerColor(mng, null, null, v.newValue, null));
            intBaseColorB.RegisterValueChangedCallback(v => SetBaseLayerColor(mng, null, null, null, v.newValue));
            intBackColorA.RegisterValueChangedCallback(v => SetBackLayerColor(mng, v.newValue, null, null, null));
            intBackColorR.RegisterValueChangedCallback(v => SetBackLayerColor(mng, null, v.newValue, null, null));
            intBackColorG.RegisterValueChangedCallback(v => SetBackLayerColor(mng, null, null, v.newValue, null));
            intBackColorB.RegisterValueChangedCallback(v => SetBackLayerColor(mng, null, null, null, v.newValue));
            intFrontColorA.RegisterValueChangedCallback(v => SetFrontLayerColor(mng, v.newValue, null, null, null));
            intFrontColorR.RegisterValueChangedCallback(v => SetFrontLayerColor(mng, null, v.newValue, null, null));
            intFrontColorG.RegisterValueChangedCallback(v => SetFrontLayerColor(mng, null, null, v.newValue, null));
            intFrontColorB.RegisterValueChangedCallback(v => SetFrontLayerColor(mng, null, null, null, v.newValue));
            tglBaseFlipX.RegisterValueChangedCallback(v =>
            {
                if (mng.SelectTile.BaseLayer != null)
                { mng.SelectTile.BaseLayer.FlipX = v.newValue; }
                mng.DisplayTileSample();
            });
            tglBackFlipX.RegisterValueChangedCallback(v =>
            {
                if (mng.SelectTile.OverlayBackLayer != null)
                { mng.SelectTile.OverlayBackLayer.FlipX = v.newValue; }
                mng.DisplayTileSample();
            });
            tglFrontFlipX.RegisterValueChangedCallback(v =>
            {
                if (mng.SelectTile.OverlayFrontLayer != null)
                { mng.SelectTile.OverlayFrontLayer.FlipX = v.newValue; }
                mng.DisplayTileSample();
            });
            fltBaseAnimation.RegisterValueChangedCallback(_ =>
            {
                if (mng.SelectTile.BaseLayer == null || mng.SelectTile.BaseLayer.Tile is not AnimatedTile)
                { return; }
                mng.DisplayTileSample();
            });
            fltBackAnimation.RegisterValueChangedCallback(_ =>
            {
                if (mng.SelectTile.OverlayBackLayer == null || mng.SelectTile.OverlayBackLayer.Tile is not AnimatedTile)
                { return; }
                mng.DisplayTileSample();
            });
            fltFrontAnimation.RegisterValueChangedCallback(_ =>
            {
                if (mng.SelectTile.OverlayFrontLayer == null || mng.SelectTile.OverlayFrontLayer.Tile is not AnimatedTile)
                { return; }
                mng.DisplayTileSample();
            });
        }

        public void InitializeTileSet(TileEditorManager mng, string tileSet, string tile)
        {
            drpTileSet.index = -1;
            drpTileSet.choices.Clear();
            foreach (var tileData in mng.TilesDatas.Values)
            {
                drpTileSet.choices.Add(tileData.TileSetId);
            }

            if (String.IsNullOrWhiteSpace(tileSet))
            {
                if (drpTileSet.choices.Count > 0)
                {
                    drpTileSet.index = 0;
                }
            }
            else
            {
                drpTileSet.value = tileSet;
            }
            if (drpTileSet.index > -1)
            {
                mng.SelectTiles = mng.TilesDatas[drpTileSet.value];
            }
            InitializeTile(mng, drpTileSet.value, tile);
        }

        public void InitializeTile(TileEditorManager mng, string tileSet, string tile)
        {
            Debug.Log($"InitializeTile : {tileSet}");
            drpTile.index = -1;
            drpTile.choices.Clear();
            if (String.IsNullOrWhiteSpace(tileSet) || !mng.TilesDatas.TryGetValue(tileSet, out var data))
            {
                return;
            }

            foreach (var tileData in data.Tiles.Values)
            {
                drpTile.choices.Add(tileData.TileId);
            }

            if (String.IsNullOrWhiteSpace(tile))
            {
                if (drpTile.choices.Count > 0)
                {
                    drpTile.index = 0;
                }
            }
            else
            {
                drpTile.value = tile;
            }
            if (drpTile.index > -1)
            {
                mng.SetTile(drpTile.value);
            }

            InitializeTileSetting(mng);
        }

        public void InitializeTileSetting(TileEditorManager mng)
        {
            tglHidden.value = mng.SelectTile.ToolHidden;
            tglCollision.value = mng.SelectTile.Collision;
            tglSpace.value = false;
            fltSpace.value = 1;
            tglAir.value = false;
            fltAir.value = 1;
            tglGround.value = false;
            fltGround.value = 1;
            tglUnderwater.value = false;
            fltUnderwater.value = 1;
            mng.SelectTile.MoveCost ??= new Dictionary<MoveType, MoveCostData>();
            foreach (var key in mng.SelectTile.MoveCost.Keys)
            {
                switch (key)
                {
                    case MoveType.Space:
                        tglSpace.value = true;
                        fltSpace.value = mng.SelectTile.MoveCost[key].MoveCost;
                        break;
                    case MoveType.Air:
                        tglAir.value = true;
                        fltAir.value = mng.SelectTile.MoveCost[key].MoveCost;
                        break;
                    case MoveType.Ground:
                        tglGround.value = true;
                        fltGround.value = mng.SelectTile.MoveCost[key].MoveCost;
                        break;
                    case MoveType.Underwater:
                        tglUnderwater.value = true;
                        fltUnderwater.value = mng.SelectTile.MoveCost[key].MoveCost;
                        break;
                }
            }
            if (mng.SelectTile.BaseLayer != null)
            {
                tglBaseFlipX.value = mng.SelectTile.BaseLayer.FlipX;
                intBaseColorA.value = mng.SelectTile.BaseLayer.TileColor.a;
                intBaseColorR.value = mng.SelectTile.BaseLayer.TileColor.r;
                intBaseColorG.value = mng.SelectTile.BaseLayer.TileColor.g;
                intBaseColorB.value = mng.SelectTile.BaseLayer.TileColor.b;
                if (mng.SelectTile.BaseLayer.Tile is AnimatedTile tile)
                { fltBaseAnimation.value = tile.m_MaxSpeed; }
                else
                { fltBaseAnimation.value = 1; }
            }
            else
            {
                tglBaseFlipX.value = false;
                intBaseColorA.value = 255;
                intBaseColorR.value = 255;
                intBaseColorG.value = 255;
                intBaseColorB.value = 255;
                fltBaseAnimation.value = 1;
            }
            if (mng.SelectTile.OverlayBackLayer != null)
            {
                tglBackFlipX.value = mng.SelectTile.OverlayBackLayer.FlipX;
                intBackColorA.value = mng.SelectTile.OverlayBackLayer.TileColor.a;
                intBackColorR.value = mng.SelectTile.OverlayBackLayer.TileColor.r;
                intBackColorG.value = mng.SelectTile.OverlayBackLayer.TileColor.g;
                intBackColorB.value = mng.SelectTile.OverlayBackLayer.TileColor.b;
                if (mng.SelectTile.OverlayBackLayer.Tile is AnimatedTile tile)
                { fltBackAnimation.value = tile.m_MaxSpeed; }
                else
                { fltBackAnimation.value = 1; }
            }
            else
            {
                tglBackFlipX.value = false;
                intBackColorA.value = 255;
                intBackColorR.value = 255;
                intBackColorG.value = 255;
                intBackColorB.value = 255;
                fltBackAnimation.value = 1;
            }
            if (mng.SelectTile.OverlayFrontLayer != null)
            {
                tglFrontFlipX.value = mng.SelectTile.OverlayFrontLayer.FlipX;
                intFrontColorA.value = mng.SelectTile.OverlayFrontLayer.TileColor.a;
                intFrontColorR.value = mng.SelectTile.OverlayFrontLayer.TileColor.r;
                intFrontColorG.value = mng.SelectTile.OverlayFrontLayer.TileColor.g;
                intFrontColorB.value = mng.SelectTile.OverlayFrontLayer.TileColor.b;
                if (mng.SelectTile.OverlayFrontLayer.Tile is AnimatedTile tile)
                { fltFrontAnimation.value = tile.m_MaxSpeed; }
                else
                { fltFrontAnimation.value = 1; }
            }
            else
            {
                tglFrontFlipX.value = false;
                intFrontColorA.value = 255;
                intFrontColorR.value = 255;
                intFrontColorG.value = 255;
                intFrontColorB.value = 255;
                fltFrontAnimation.value = 1;
            }
            DisplaySelectSprite(mng);
        }

        private void LoadingResource(TileEditorManager mng, string path)
        {
            divTiles.Clear();
            var pathResourceBase = Path.Combine(DataUtil.PathBase, "resource", "image", "map", $"{path}.png");
            var tex = DataUtil.GetImage(pathResourceBase);
            if (!tex)
            { return; }
            for (var y = 0; y < tex.height; y += pixel*3)
            {
                for (var x = 0; x < tex.width; x += pixel*2)
                {
                    var rect = new Rect(x, y, pixel*2, pixel*3);
                    var sprite = Sprite.Create(tex, rect, new Vector2(0.5f, 0), pixel*2);
                    var btn = templateTile.Instantiate();
                    var elmtBtn = btn.Q<Button>("BtnTile");
                    elmtBtn.style.backgroundImage = new StyleBackground(sprite);
                    elmtBtn.clicked += () =>
                    {
                        mng.SetSprite(selectLayer, path, sprite);
                        mng.WaitNavigate = 0;
                    };
                    divTiles.Add(elmtBtn);
                }
            }
        }

        public void SelectTile(string resource)
        {
            foreach (var elm in divTiles.Children())
            {
                SetBorder(elm, elm.name == resource ? Color.green : Color.black);
            }
        }
        
        public void UnselectTile()
        {
            foreach (var elm in divTiles.Children())
            {
                SetBorder(elm, Color.black);
            }
        }

        private void SetBorder(VisualElement elm, Color color)
        {
            elm.style.borderBottomColor = color;
            elm.style.borderLeftColor = color;
            elm.style.borderRightColor = color;
            elm.style.borderTopColor = color;
        }

        public void SetPixel(TileEditorManager mng, int px)
        {
            pixel = px;
            LoadingResource(mng, drpResource.value);
        }

        private void SetStyle(VisualElement btn, LayerData dat)
        {
            if (dat == null) return;
            switch (dat.Tile)
            {
                case AnimatedTile tile:
                    btn.style.backgroundImage = new StyleBackground(tile.m_AnimatedSprites[0]);
                    break;
                case IsometricRuleTile ruleTile:
                    btn.style.backgroundImage = new StyleBackground(ruleTile.m_DefaultSprite);
                    break;
                case RuleOverrideTile overrideTile:
                    btn.style.backgroundImage = new StyleBackground(overrideTile.m_Tile.m_DefaultSprite);
                    break;
                default:
                    btn.style.backgroundImage = new StyleBackground(((Tile)dat.Tile).sprite);
                    btn.style.unityBackgroundImageTintColor = ((Tile)dat.Tile).color;
                    break;
            }

            if (dat.FlipX)
            {
                btn.resolvedStyle.translate.Scale(new Vector3(-1, 1, 1));
            }
        }

        public void DisplaySelectSprite(TileEditorManager mng)
        {
            btnDisplayBase.style.backgroundImage = mng.SelectBaseSprite < mng.BaseSpriteList.Count ? new StyleBackground(mng.BaseSpriteList[mng.SelectBaseSprite]) : new StyleBackground(noneImage);
            lblBaseNums.text = mng.BaseSpriteList.Count > 0 ? $"{mng.SelectBaseSprite + 1} / {mng.BaseSpriteList.Count}" : "";

            btnDisplayBack.style.backgroundImage = mng.SelectBackSprite < mng.BackSpriteList.Count ? new StyleBackground(mng.BackSpriteList[mng.SelectBackSprite]) : new StyleBackground(noneImage);
            lblBackNums.text = mng.BackSpriteList.Count > 0 ? $"{mng.SelectBackSprite + 1} / {mng.BackSpriteList.Count}" : "";

            btnDisplayFront.style.backgroundImage = mng.SelectFrontSprite < mng.FrontSpriteList.Count ? new StyleBackground(mng.FrontSpriteList[mng.SelectFrontSprite]) : new StyleBackground(noneImage);
            lblFrontNums.text = mng.FrontSpriteList.Count > 0 ? $"{mng.SelectFrontSprite + 1} / {mng.FrontSpriteList.Count}" : "";
        }

        private void SetMoveCost(TileEditorManager mng, bool enable, MoveType moveType, float moveCost)
        {
            moveCost = math.clamp(moveCost, 0.5f, 10f);
            switch (moveType)
            {
                case MoveType.Space:
                    fltSpace.value = moveCost;
                    break;
                case MoveType.Air:
                    fltAir.value = moveCost;
                    break;
                case MoveType.Ground:
                    fltGround.value = moveCost;
                    break;
                case MoveType.Underwater:
                    fltUnderwater.value = moveCost;
                    break;
            }
            if (!enable) mng.SelectTile.SetMoveCost(moveType, 0);
            else mng.SelectTile.SetMoveCost(moveType, moveCost);
        }

        private void SetBaseLayerColor(TileEditorManager mng, int? alpha, int? red, int? green, int? blue)
        {
            if (alpha != null) intBaseColorA.value = math.clamp(alpha.Value, 0, 255);
            if (red != null) intBaseColorR.value = math.clamp(red.Value, 0, 255);
            if (green != null) intBaseColorG.value = math.clamp(green.Value, 0, 255);
            if (blue != null) intBaseColorB.value = math.clamp(blue.Value, 0, 255);
            mng.SelectTile.BaseLayer?.SetColor(intBaseColorA.value, intBaseColorR.value, intBaseColorG.value, intBaseColorB.value);
            mng.DisplayTileSample();
        }

        private void SetFrontLayerColor(TileEditorManager mng, int? alpha, int? red, int? green, int? blue)
        {
            if (alpha != null) intFrontColorA.value = math.clamp(alpha.Value, 0, 255);
            if (red != null) intFrontColorR.value = math.clamp(red.Value, 0, 255);
            if (green != null) intFrontColorG.value = math.clamp(green.Value, 0, 255);
            if (blue != null) intFrontColorB.value = math.clamp(blue.Value, 0, 255);
            mng.SelectTile.OverlayFrontLayer?.SetColor(intFrontColorA.value, intFrontColorR.value, intFrontColorG.value, intFrontColorB.value);
            mng.DisplayTileSample();
        }

        private void SetBackLayerColor(TileEditorManager mng, int? alpha, int? red, int? green, int? blue)
        {
            if (alpha != null) intBackColorA.value = math.clamp(alpha.Value, 0, 255);
            if (red != null) intBackColorR.value = math.clamp(red.Value, 0, 255);
            if (green != null) intBackColorG.value = math.clamp(green.Value, 0, 255);
            if (blue != null) intBackColorB.value = math.clamp(blue.Value, 0, 255);
            mng.SelectTile.OverlayBackLayer?.SetColor(intBackColorA.value, intBackColorR.value, intBackColorG.value, intBackColorB.value);
            mng.DisplayTileSample();
        }

        public DropdownField TileSetDropdown
        {
            get => drpTileSet;
        }
        public DropdownField TileDropdown
        {
            get => drpTile;
        }
        public int Pixel
        {
            get => pixel;
        }
        public bool IsOver
        {
            get => isOverTopToolbar || isOverToolWindow;
        }

        public bool IsHidden
        {
            get => tglHidden.value;
        }
        public bool IsCollision
        {
            get => tglCollision.value;
        }
        public bool IsBaseFlipX
        {
            get => tglBaseFlipX.value;
        }
        public bool IsBackFlipX
        {
            get => tglBackFlipX.value;
        }
        public bool IsFrontFlipX
        {
            get => tglFrontFlipX.value;
        }
        public float BaseAnimationSpeed
        {
            get => fltBaseAnimation.value;
        }
        public float BackAnimationSpeed
        {
            get => fltBackAnimation.value;
        }
        public float FrontAnimationSpeed
        {
            get => fltFrontAnimation.value;
        }
        public Color32 BaseLayerColor
        {
            get => new Color32((byte)intBaseColorR.value, (byte)intBaseColorG.value, (byte)intBaseColorB.value, (byte)intBaseColorA.value);
        }
        public Color32 BackLayerColor
        {
            get => new Color32((byte)intBackColorR.value, (byte)intBackColorG.value, (byte)intBackColorB.value, (byte)intBackColorA.value);
        }
        public Color32 FrontLayerColor
        {
            get => new Color32((byte)intFrontColorR.value, (byte)intFrontColorG.value, (byte)intFrontColorB.value, (byte)intFrontColorA.value);
        }
    }
}
