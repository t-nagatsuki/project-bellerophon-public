using System;
using System.IO;
using System.Linq;
using Functions.Data;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Windows;

namespace Functions.UI
{
    public class ToolSelectorWindow : MonoBehaviour
    {
        [SerializeField] private SettingWindow settingWindow;
        [SerializeField] private ErrorWindow errorWindow;

        private bool isError;
        
        private UIDocument document;
        private GroupBox grpTool;
        private Button btnTileEditor;
        private Button btnMapEditor;
        private Button btnCharaEditor;
        private Button btnUnitEditor;
        private Button btnLoad;
        private Button btnPlay;
        private Button btnInfo;
        private Button btnSetting;
        private Button btnExit;

        private BasicAction action;

        private async void Awake()
        {
            if (!string.IsNullOrWhiteSpace(DataUtil.ErrorMessage))
            {
                errorWindow.SetError(DataUtil.ErrorMessage);
                return;
            }
            var pathBase = DataUtil.PathBase;
            var copyrightSetting = await DataUtil.LoadCopyright(pathBase);
            action = new BasicAction();
            action.Enable();
            document = GetComponent<UIDocument>();
            var background = document.rootVisualElement;
            grpTool = document.rootVisualElement.Q<GroupBox>("GrpTool");
            btnTileEditor = document.rootVisualElement.Q<Button>("BtnTileEditor");
            btnMapEditor = document.rootVisualElement.Q<Button>("BtnMapEditor");
            btnCharaEditor = document.rootVisualElement.Q<Button>("BtnCharacterEditor");
            btnUnitEditor = document.rootVisualElement.Q<Button>("BtnUnitEditor");
            btnLoad = document.rootVisualElement.Q<Button>("BtnLoad");
            btnPlay = document.rootVisualElement.Q<Button>("BtnPlay");
            btnInfo = document.rootVisualElement.Q<Button>("BtnInfo");
            btnSetting = document.rootVisualElement.Q<Button>("BtnSetting");
            btnExit = document.rootVisualElement.Q<Button>("BtnExit");
            
            // 共通設定読込
            var datCommonSettings = await DataUtil.LoadCommonSettings(DataUtil.PathBase);
            if (datCommonSettings == null)
            {
                isError = true;
                errorWindow.SetError("data/common/settings.jsonが正常に読み込めませんでした。");
                return;
            }

            if (datCommonSettings.ToolMode)
            {
                btnTileEditor.clicked += () => { SceneManager.LoadScene(1); };
                btnMapEditor.clicked += () => { SceneManager.LoadScene(2); };
                btnCharaEditor.clicked += () => { SceneManager.LoadScene(4); };
                btnUnitEditor.clicked += () => { SceneManager.LoadScene(5); };
                btnUnitEditor.style.display = DisplayStyle.None;

            }
            else
            { grpTool.style.display = DisplayStyle.None; }

            btnLoad.clicked += () =>
            {
                SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
                SceneManager.LoadScene(3);
            };
            btnPlay.clicked += () =>
            {
                SceneManager.LoadScene(3);
            };
            var path = Path.Combine(DataUtil.PathBase, "save", "dat_0001.json");
            var dat = await DataUtil.LoadData<SaveData>(path);
            if (dat == null)
            {
                btnLoad.style.display = DisplayStyle.None;
            }

            btnInfo.clicked += () =>
            {
                var copyright = "<size=+2><b>Bellerophon Launcher</b> : ver 1.00α8(20251222)</size>" +
                                Environment.NewLine + Environment.NewLine;
                foreach (var setting in copyrightSetting)
                {
                    copyright += $"<b><u>{setting.title}</u></b>" + Environment.NewLine + Environment.NewLine;
                    foreach (var text in setting.text)
                    {
                        copyright += $"<b>{text.title}</b> : {text.text}";
                        if (!string.IsNullOrWhiteSpace(text.link))
                        {
                            copyright += $" ( <a href='{text.link}'>{text.link}</a> )";
                        }
                        copyright += "" + Environment.NewLine;
                    }
                }
                errorWindow.SetError("Info", copyright);
            };

            btnSetting.clicked += () =>
            {
                settingWindow.VisibleDisplay();
            };
            btnExit.clicked += Exit;
        }

        private void OnDisable()
        {
            action.Disable();
        }

        void Update()
        {
            if (errorWindow.IsDisplay())
            {
                if (action.UI.Enter.triggered || action.UI.Click.WasPressedThisFrame())
                {
                    if (isError)
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                    }
                    else
                    { errorWindow.HiddenDisplay(); }
                }
            }
        }

        private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            foreach (var mng in arg0.GetRootGameObjects())
            {
                var comp = mng.GetComponent<SlgSceneManager>();
                if (!comp)
                { continue; }
                comp.LoadingData = "dat_0001.json";
            }
            SceneManager.sceneLoaded -= SceneManagerOnsceneLoaded;
        }

        private void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
    }
}
