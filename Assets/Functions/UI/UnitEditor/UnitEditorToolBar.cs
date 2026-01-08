using System;
using System.IO;
using Functions.Data.Units;
using Functions.Enum;
using Functions.Manager;
using Functions.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Functions.UI.UnitEditor
{
    public class UnitEditorToolBar : MonoBehaviour
    {
        private UIDocument document;
        private VisualElement divTopToolbar;
        private VisualElement divToolWindow;
        private Button btnNew;
        private Button btnSave;
        private Button btnLoad;
        private Button btnSettings;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnClose;
        private Button btnTabBase;
        private Button btnTabWeapon;
        private DropdownField drpUnit;

        private TextField txtUnitId;
        private TextField txtUnitName;
        private DropdownField drpImage;
        private VisualElement imgUnit;
        private EnumField enmSpace;
        private EnumField enmAir;
        private EnumField enmGround;
        private EnumField enmUnderwater;

        private TextField txtAccuracy;
        private TextField txtManeuver;
        private TextField txtPower;
        private TextField txtArmor;
        private TextField txtReduction;
        private TextField txtMove;
        private TextField txtHP;
        private TextField txtEN;

        private bool isOverTopToolbar;
        private bool isOverToolWindow;

        private void Awake()
        {
            document = GetComponent<UIDocument>();
            divTopToolbar = document.rootVisualElement.Q<VisualElement>("div-top-toolbar");
            divToolWindow = document.rootVisualElement.Q<VisualElement>("div-tool-window");
            btnNew = document.rootVisualElement.Q<Button>("BtnNew");
            btnSave = document.rootVisualElement.Q<Button>("BtnSave");
            btnLoad = document.rootVisualElement.Q<Button>("BtnLoad");
            btnSettings = document.rootVisualElement.Q<Button>("BtnSettings");
            btnAdd = document.rootVisualElement.Q<Button>("BtnAdd");
            btnDelete = document.rootVisualElement.Q<Button>("BtnDelete");
            btnClose = document.rootVisualElement.Q<Button>("BtnClose");
            btnTabBase = document.rootVisualElement.Q<Button>("BtnTabBase");
            btnTabWeapon = document.rootVisualElement.Q<Button>("BtnTabWeapon");
            drpUnit = document.rootVisualElement.Q<DropdownField>("DrpUnit");
            
            txtUnitId = document.rootVisualElement.Q<TextField>("TxtUnitId");
            txtUnitName = document.rootVisualElement.Q<TextField>("TxtUnitName");
            drpImage = document.rootVisualElement.Q<DropdownField>("DrpImage");
            imgUnit = document.rootVisualElement.Q<VisualElement>("Image");
            imgUnit.style.backgroundImage = Texture2D.whiteTexture;
            enmSpace = document.rootVisualElement.Q<EnumField>("EnmSpace");
            enmAir = document.rootVisualElement.Q<EnumField>("EnmAir");
            enmGround = document.rootVisualElement.Q<EnumField>("EnmGround");
            enmUnderwater = document.rootVisualElement.Q<EnumField>("EnmUnderwater");
            txtAccuracy = document.rootVisualElement.Q<TextField>("TxtAccuracy");
            txtManeuver = document.rootVisualElement.Q<TextField>("TxtManeuver");
            txtPower = document.rootVisualElement.Q<TextField>("TxtPower");
            txtArmor = document.rootVisualElement.Q<TextField>("TxtArmor");
            txtReduction = document.rootVisualElement.Q<TextField>("TxtReduction");
            txtMove = document.rootVisualElement.Q<TextField>("TxtMove");
            txtHP = document.rootVisualElement.Q<TextField>("TxtHP");
            txtEN = document.rootVisualElement.Q<TextField>("TxtEN");
        }

        public void SetManager(UnitEditorManager mng)
        {
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
            mng.EditorWindowManager.SettingWindow.SetManager(mng);
            btnNew.clicked += () =>
            {
                mng.EditorWindowManager.SettingWindow.InitializeDisplay(mng);
            };
            btnSave.clicked += mng.SaveUnits;
            btnLoad.clicked += () =>
            {
                mng.EditorWindowManager.LoadWindow.InitializeDisplay(mng);
                mng.EditorWindowManager.LoadWindow.VisibleDisplay();
            };
            btnSettings.clicked += () =>
            {
                mng.EditorWindowManager.SettingWindow.EditDisplay(mng);
            };
            btnAdd.clicked += () =>
            {
                mng.EditorWindowManager.SettingWindow.AddDisplay(mng);
            };
            btnDelete.clicked += mng.DeleteUnit;
            btnClose.clicked += () =>
            {
                SceneManager.LoadScene(0);
            };

            drpUnit.RegisterValueChangedCallback(v =>
            {
                mng.SetUnit(v.newValue);
            });
            drpImage.RegisterValueChangedCallback(v =>
            {
                if (string.IsNullOrWhiteSpace(v.newValue))
                {
                    imgUnit.style.backgroundImage = Texture2D.whiteTexture;
                    return;
                }
                var pathResource = Path.Combine(DataUtil.PathBase, "resource", "image", "unit", v.newValue);
                var texture = DataUtil.GetImage(pathResource);
                imgUnit.style.backgroundImage = texture;
            });
            
            drpImage.choices.Add(" ");
            foreach (var exp in new[]{"png", "bmp", "tif", "tga", "jpg", "psd"})
            {
                foreach (var path in DataUtil.GetImages("unit", $"*.{exp}"))
                { drpImage.choices.Add(Path.GetFileName(path)); }
            }
        }

        public void SetUnit(UnitData mngUnit)
        {
            if (mngUnit == null) return;
            txtUnitId.value = mngUnit.UnitId;
            txtUnitName.value = mngUnit.UnitName;
            drpImage.value = mngUnit.UnitImagePath;
            
            // 地形適正
            enmSpace.value = mngUnit.Space.suitable;
            enmAir.value = mngUnit.Air.suitable;
            enmGround.value = mngUnit.Ground.suitable;
            enmUnderwater.value = mngUnit.Underwater.suitable;
            
            // 能力
            txtAccuracy.value = mngUnit.Accuracy.status.ToString();
            txtManeuver.value = mngUnit.Maneuver.status.ToString();
            txtPower.value = mngUnit.Power.status.ToString();
            txtArmor.value = mngUnit.Armor.status.ToString();
            txtReduction.value = mngUnit.Reduction.status.ToString();
            txtMove.value = mngUnit.Move.status.ToString();
            txtHP.value = mngUnit.HP.status.ToString();
            txtEN.value = mngUnit.EN.status.ToString();
        }

        public void UpdateUnit(UnitData mngUnit)
        {
            if (mngUnit == null) return;
            mngUnit.UnitId = txtUnitId.value;
            mngUnit.UnitName = txtUnitName.value;
            mngUnit.UnitImagePath = drpImage.value;
            mngUnit.Space.suitable = (Suitable)enmSpace.value;
            mngUnit.Air.suitable = (Suitable)enmAir.value;
            mngUnit.Ground.suitable = (Suitable)enmGround.value;
            mngUnit.Underwater.suitable = (Suitable)enmUnderwater.value;
            mngUnit.Accuracy.status = Convert.ToInt32(txtAccuracy.value);
            mngUnit.Maneuver.status = Convert.ToInt32(txtManeuver.value);
            mngUnit.Power.status = Convert.ToInt32(txtPower.value);
            mngUnit.Armor.status = Convert.ToInt32(txtArmor.value);
            mngUnit.Reduction.status = Convert.ToInt32(txtReduction.value);
            mngUnit.Move.status = Convert.ToInt32(txtMove.value);
            mngUnit.HP.status = Convert.ToInt32(txtHP.value);
            mngUnit.EN.status = Convert.ToInt32(txtEN.value);
        }

        public DropdownField UnitDropdown => drpUnit;
        public bool IsOver => isOverTopToolbar || isOverToolWindow;
    }
}
