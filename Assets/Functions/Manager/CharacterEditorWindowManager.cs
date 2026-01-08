using Functions.UI;
using Functions.UI.CharacterEditor;
using UnityEngine;

namespace Functions.Manager
{
    public class CharacterEditorWindowManager : MonoBehaviour
    {
        [SerializeField] private CharacterEditorToolBar editorToolBar;
        [SerializeField] private CharacterSettingWindow settingWindow;
        [SerializeField] private CharacterLoadWindow loadWindow;
        [SerializeField] private ErrorWindow errorWindow;

        public void Initialize(CharacterEditorManager mng)
        {
            editorToolBar.SetManager(mng);
            loadWindow.SetManager(mng);
        }

        public bool WindowProcess(CharacterEditorManager mng)
        {
            // エラー処理
            if (errorWindow.IsDisplay())
            {
                if (mng.Action.UI.Enter.triggered || mng.Action.UI.Click.WasPressedThisFrame())
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

        public CharacterEditorToolBar EditorToolBar => editorToolBar;

        public CharacterSettingWindow SettingWindow => settingWindow;

        public CharacterLoadWindow LoadWindow => loadWindow;
    }
}