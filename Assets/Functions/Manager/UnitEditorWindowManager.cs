using Functions.UI;
using Functions.UI.UnitEditor;
using UnityEngine;

namespace Functions.Manager
{
    public class UnitEditorWindowManager : MonoBehaviour
    {
        [SerializeField] private UnitEditorToolBar editorToolBar;
        [SerializeField] private UnitSettingWindow settingWindow;
        [SerializeField] private UnitLoadWindow loadWindow;
        [SerializeField] private ErrorWindow errorWindow;

        public void Initialize(UnitEditorManager mng)
        {
            editorToolBar.SetManager(mng);
            loadWindow.SetManager(mng);
        }

        public bool WindowProcess(UnitEditorManager mng)
        {
            // エラー処理
            if (errorWindow.IsDisplay())
            {
                if (mng.Action.UI.Enter.triggered || mng.Action.UI.Click.WasPressedThisFrame())
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }
                return true;
            }
            // キャラクター設定画面表示中
            // キャラクターロード画面表示中
            if (settingWindow && settingWindow.IsDisplay()
                || loadWindow && loadWindow.IsDisplay())
            { return true; }
            return editorToolBar.IsOver;
        }

        public void SetError(string err)
        {
            errorWindow.SetError(err);
        }

        public void SetWarning(string err)
        {
            errorWindow.SetWarning(err);
        }

        public UnitEditorToolBar EditorToolBar => editorToolBar;

        public UnitSettingWindow SettingWindow => settingWindow;

        public UnitLoadWindow LoadWindow => loadWindow;
    }
}