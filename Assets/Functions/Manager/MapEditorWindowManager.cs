using Functions.Data;
using Functions.Data.Maps;
using Functions.UI;
using Functions.UI.MapEditor;
using UnityEngine;
using UnityEngine.Serialization;
namespace Functions.Manager
{
    public class MapEditorWindowManager : MonoBehaviour
    {
        [FormerlySerializedAs("editorToolBar")]
        [SerializeField] private MapEditorToolBar mapEditorToolBar;
        [SerializeField] private MapSettingWindow settingWindow;
        [SerializeField] private MapLoadWindow loadWindow;
        [SerializeField] private ErrorWindow errorWindow;

        private bool isDisplayCommandMenu;

        public void Initialize(MapEditorManager _mng)
        {
            mapEditorToolBar.SetManager(_mng);
            loadWindow.SetManager(_mng);
        }

        public bool WindowProcess(MapEditorManager _mng)
        {
            // エラー処理
            if (errorWindow.IsDisplay())
            {
                if (_mng.Action.UI.Enter.triggered || _mng.Action.UI.Click.WasPressedThisFrame())
                {
                    if (errorWindow.IsCritical())
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
                    }
                    else
                    {
                        errorWindow.HiddenDisplay();
                    }
                }
                return true;
            }
            // マップ設定画面表示中
            // マップロード画面表示中
            // タイルエディット画面表示中
            if (settingWindow && settingWindow.IsDisplay()
                || loadWindow && loadWindow.IsDisplay())
            { return true; }
            if (_mng.Action.UI.PageUp.triggered)
            {
                mapEditorToolBar.SetHeight(mapEditorToolBar.NowHeight + 1);
            }
            if (_mng.Action.UI.PageDown.triggered)
            {
                mapEditorToolBar.SetHeight(mapEditorToolBar.NowHeight - 1);
            }
            return mapEditorToolBar.IsOver;
        }

        public void SetDebug(Vector3Int _pos, TileData _dat)
        {
            mapEditorToolBar.SetData("Position", $"({_pos.x}, {_pos.y}, {_pos.z})");
            if (_dat != null)
            {
                mapEditorToolBar.SetData("TileSet", $"{_dat.TileSetId}");
                mapEditorToolBar.SetData("Tile", $"{_dat.TileId}");
            }
            else
            {
                mapEditorToolBar.SetData("TileSet", $"None");
                mapEditorToolBar.SetData("Tile", $"None");
            }
        }

        public void SetError(string err)
        {
            errorWindow.SetError(err);
        }

        public void SetWarning(string err)
        {
            errorWindow.SetWarning(err);
        }

        public void SelectTile(TileData dat)
        {
            mapEditorToolBar.SelectTile(dat);
        }

        public void UnselectTile()
        {
            mapEditorToolBar.UnselectTile();
        }

        public bool IsDisplayCommandMenu => isDisplayCommandMenu;

        public MapEditorToolBar MapEditorToolBar => mapEditorToolBar;

        public MapSettingWindow MapSettingWindow => settingWindow;

        public MapLoadWindow MapLoadWindow => loadWindow;
    }
}
