using System;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Functions.Data;
using Functions.Util;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
namespace Functions.UI
{
    public class SettingWindow : BaseWindow
    {
        [SerializeField] private ErrorWindow errorWindow;
        private bool isError;

        private Button btnGraphic;
        private Button btnSound;
        private Button btnBack;
        private Button btnApply;
        private Button btnCancel;
        private RadioButtonGroup radBtnWindowMode;
        private DropdownField drpWindowSize;
        private DropdownField drpLocale;
        private SliderInt slideBgm;
        private SliderInt slideSound;
        private TextField txtBgm;
        private TextField txtSound;
        private TabView tabSetting;

        private BasicAction action;
        public event Action OnChange;

        public override void Setup()
        {
            action = new BasicAction();
            action.Enable();
            btnGraphic = document.rootVisualElement.Q<Button>("BtnGraphic");
            btnSound = document.rootVisualElement.Q<Button>("BtnSound");
            btnBack = document.rootVisualElement.Q<Button>("BtnBack");
            btnApply = document.rootVisualElement.Q<Button>("BtnApply");
            btnCancel = document.rootVisualElement.Q<Button>("BtnCancel");

            radBtnWindowMode = document.rootVisualElement.Q<RadioButtonGroup>("RadBtnWindowMode");
            drpWindowSize = document.rootVisualElement.Q<DropdownField>("DrpWindowSize");
            drpLocale = document.rootVisualElement.Q<DropdownField>("DrpLocale");

            slideBgm = document.rootVisualElement.Q<SliderInt>("SlideBgm");
            slideSound = document.rootVisualElement.Q<SliderInt>("SlideSound");
            txtBgm = document.rootVisualElement.Q<TextField>("TxtBgm");
            txtSound = document.rootVisualElement.Q<TextField>("TxtSound");
            tabSetting = document.rootVisualElement.Q<TabView>("TabSetting");

            btnGraphic.clicked += () =>
            {
                tabSetting.selectedTabIndex = 0;
            };
            btnSound.clicked += () =>
            {
                tabSetting.selectedTabIndex = 1;
            };
            btnBack.clicked += HiddenDisplay;
            btnApply.clicked += SaveSetting;
            btnCancel.clicked += InitializeSetting;

            slideBgm.RegisterValueChangedCallback(v =>
            {
                txtBgm.value = v.newValue.ToString();
            });
            slideSound.RegisterValueChangedCallback(v =>
            {
                txtSound.value = v.newValue.ToString();
            });
            drpLocale.choices.Clear();
            foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
            {
                drpLocale.choices.Add(locale.LocaleName);
            }
            drpLocale.RegisterValueChangedCallback(async evt =>
            {
                var locale = LocalizationSettings.AvailableLocales.Locales.Find(x => x.LocaleName == drpLocale.value);
                LocalizationSettings.SelectedLocale = locale;
                await LocalizationSettings.InitializationOperation.Task;
            });
        }

        private void OnDestroy()
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

        public new void VisibleDisplay()
        {
            tabSetting.selectedTabIndex = 0;
            InitializeSetting();
            base.VisibleDisplay();
        }

        private void InitializeSetting()
        {
            var dat = DataUtil.SystemSettingsData;
            if (dat == null)
            { return; }
            radBtnWindowMode.value = dat.WindowMode;
            drpWindowSize.value = $"{dat.WindowWidth} x {dat.WindowHeight}";
            drpLocale.index = dat.SelectLocale;
            slideBgm.value = Mathf.CeilToInt( dat.BgmVolume * 100);
            slideSound.value = Mathf.CeilToInt( dat.SoundVolume * 100);
            txtBgm.value = slideBgm.value.ToString();
            txtSound.value = slideSound.value.ToString();
        }

        private void SaveSetting()
        {
            DataUtil.SystemSettingsData.WindowMode = radBtnWindowMode.value;
            var size = drpWindowSize.value.Split(" x ");
            DataUtil.SystemSettingsData.WindowWidth = Convert.ToInt32(size[0]);
            DataUtil.SystemSettingsData.WindowHeight = Convert.ToInt32(size[1]);
            DataUtil.SystemSettingsData.SelectLocale = drpLocale.index;
            DataUtil.SystemSettingsData.BgmVolume = slideBgm.value / 100.0f;
            DataUtil.SystemSettingsData.SoundVolume = slideSound.value / 100.0f;
            DataUtil.SaveData(Path.Combine(DataUtil.PathBase, "system.json"), DataUtil.SystemSettingsData);
            switch (DataUtil.SystemSettingsData.WindowMode)
            {
                case 0:
                    Screen.SetResolution(DataUtil.SystemSettingsData.WindowWidth, DataUtil.SystemSettingsData.WindowHeight, true);
                    break;
                case 1:
                    Screen.SetResolution(DataUtil.SystemSettingsData.WindowWidth, DataUtil.SystemSettingsData.WindowHeight, false);
                    break;
            }
            OnChange?.Invoke();
        }
    }
}
