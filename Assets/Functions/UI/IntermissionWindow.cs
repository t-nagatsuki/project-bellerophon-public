using System;
using System.IO;
using System.Linq;
using Functions.Data;
using Functions.Enum;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using File = System.IO.File;

namespace Functions.UI
{
    public class IntermissionWindow : MonoBehaviour
    {
        [SerializeField] private SettingWindow settingWindow;
        
        private UIDocument document;
        private Button btnSave;
        private Button btnLoad;
        private Button btnUnitList;
        private Button btnCharacterList;
        private Button btnSetting;
        private Button btnNextStage;
        private Button btnExit;

        private string nextStageClass;
        private string nextStageMethod;

        private Action btnSaveAction;
        private Action btnLoadAction;
        private Action btnUnitListAction;
        private Action btnCharacterListAction;
        private Action btnNextStageAction;

        private void Awake()
        {
            document = GetComponent<UIDocument>();
            document.rootVisualElement.style.display = DisplayStyle.None;
            btnSave = document.rootVisualElement.Q<Button>("BtnSave");
            btnLoad = document.rootVisualElement.Q<Button>("BtnLoad");
            btnUnitList = document.rootVisualElement.Q<Button>("BtnUnitList");
            btnCharacterList = document.rootVisualElement.Q<Button>("BtnCharacterList");
            btnSetting = document.rootVisualElement.Q<Button>("BtnSetting");
            btnNextStage = document.rootVisualElement.Q<Button>("BtnNextStage");
            btnExit = document.rootVisualElement.Q<Button>("BtnExit");

            SettingButtonDisplay();
        }

        public void Display(SlgSceneManager mng, string name, string method)
        {
            if (string.IsNullOrWhiteSpace(name))
            { nextStageClass = mng.ScriptManager.NowClass; }
            else
            { nextStageClass = name; }
            nextStageMethod = method;
            
            document.rootVisualElement.style.display = DisplayStyle.Flex;
            btnLoadAction = () =>
            {
                mng.Load(1);
            };
            btnLoad.clicked += btnLoadAction;

            btnSaveAction = () =>
            {
                mng.Save(1);
                SettingButtonDisplay();
            };
            btnSave.clicked += btnSaveAction;

            btnUnitListAction = () =>
            {
                mng.SetUnitList(true);
            };
            btnUnitList.clicked += btnUnitListAction;

            btnCharacterListAction = () =>
            {
                mng.SetCharaList(true);
            };
            btnCharacterList.clicked += btnCharacterListAction;

            settingWindow.OnChange += mng.LoadSystemSettings;
            btnSetting.clicked += () =>
            {
                settingWindow.VisibleDisplay();
                // SceneManager.LoadScene(4);
            };
            btnExit.clicked += () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            };

            btnNextStageAction = () =>
            {
                mng.ProcessMode = ProcessMode.None;
                mng.ScriptManager.CallMethod(nextStageClass, nextStageMethod, 0);
                document.rootVisualElement.style.display = DisplayStyle.None;
            };
            btnNextStage.clicked += btnNextStageAction;
        }

        public void HiddenDisplay()
        {
            btnLoad.clicked -= btnLoadAction;
            btnSave.clicked -= btnSaveAction;
            btnUnitList.clicked -= btnUnitListAction;
            btnCharacterList.clicked -= btnCharacterListAction;
            btnNextStage.clicked -= btnNextStageAction;
            document.rootVisualElement.style.display = DisplayStyle.None;
        }

        private void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        private void SettingButtonDisplay()
        {
            var path = Path.Combine(DataUtil.PathBase, "save", "dat_0001.json");
            btnLoad.style.display = !File.Exists(path) ? DisplayStyle.None : DisplayStyle.Flex;
        }
        
        public bool IsDisplay()
        {
            return document.rootVisualElement.style.display == DisplayStyle.Flex;
        }

        public string NextStageClass => nextStageClass;
        public string NextStageMethod => nextStageMethod;
    }
}
