using Functions.Data;
using Functions.UI;
using Functions.UI.MapEditor;
using Functions.UI.TileEditor;
using UnityEngine;
using UnityEngine.Serialization;
namespace Functions.Manager
{
    public class TileEditorWindowManager : MonoBehaviour
    {
        [FormerlySerializedAs("editorToolBar")]
        [SerializeField] private TileEditorToolBar tileEditorToolBar;
        [SerializeField] private TileSettingWindow settingWindow;
        [SerializeField] private TileLoadWindow loadWindow;
        [SerializeField] private ErrorWindow errorWindow;

        private bool isDisplayCommandMenu;

        public void Initialize(TileEditorManager _mng)
        {
            tileEditorToolBar.SetManager(_mng);
            loadWindow.SetManager(_mng);
        }

        public bool WindowProcess(TileEditorManager _mng)
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
            return tileEditorToolBar.IsOver;
        }

        public void SetError(string err)
        {
            errorWindow.SetError(err);
        }

        public void SetWarning(string err)
        {
            errorWindow.SetWarning(err);
        }

        public void SelectTile(string resource)
        {
            TileEditorToolBar.SelectTile(resource);
        }

        public void UnselectTile()
        {
            TileEditorToolBar.UnselectTile();
        }

        public bool IsDisplayCommandMenu => isDisplayCommandMenu;

        public TileEditorToolBar TileEditorToolBar => tileEditorToolBar;

        public TileSettingWindow TileSettingWindow => settingWindow;

        public TileLoadWindow TileLoadWindow => loadWindow;
    }
}
