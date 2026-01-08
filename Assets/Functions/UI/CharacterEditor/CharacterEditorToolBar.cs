using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Functions.Data;
using Functions.Data.Units;
using Functions.Enum;
using Functions.Manager;
using Functions.Util;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Functions.UI.CharacterEditor
{
    public class CharacterEditorToolBar : MonoBehaviour
    {
        [SerializeField]
        private VisualTreeAsset growthSuitabilityRecord;

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
        private DropdownField drpCharacter;

        private TextField txtCharacterId;
        private TextField txtCharacterName;
        private DropdownField drpImage;
        private DropdownField drpMusic;
        private Button btnMusicPlay;
        private VisualElement imgChara;
        private EnumField enmSpace;
        private EnumField enmAir;
        private EnumField enmGround;
        private EnumField enmUnderwater;
        private TextField txtConcentration;
        private TextField txtReaction;
        private TextField txtAbility;
        private TextField txtPerception;
        private TextField txtIntention;
        private TextField txtEndurance;
        private TextField txtExpertise;
        private TextField txtSp;
        private TextField txtGrowthConcentration;
        private TextField txtGrowthReaction;
        private TextField txtGrowthAbility;
        private TextField txtGrowthPerception;
        private TextField txtGrowthIntention;
        private TextField txtGrowthEndurance;
        private TextField txtGrowthExpertise;
        private TextField txtGrowthSp;
        private VisualElement divGrowthSuitability;
        private Button btnAddGrowthSuitability;
        private List<VisualTreeAsset> growthSuitabilityRecords = new();

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
            drpCharacter = document.rootVisualElement.Q<DropdownField>("DrpCharacter");
            
            txtCharacterId = document.rootVisualElement.Q<TextField>("TxtCharacterId");
            txtCharacterName = document.rootVisualElement.Q<TextField>("TxtCharacterName");
            drpImage = document.rootVisualElement.Q<DropdownField>("DrpImage");
            drpMusic = document.rootVisualElement.Q<DropdownField>("DrpMusic");
            btnMusicPlay = document.rootVisualElement.Q<Button>("BtnMusicPlay");
            imgChara = document.rootVisualElement.Q<VisualElement>("Image");
            imgChara.style.backgroundImage = Texture2D.whiteTexture;
            enmSpace = document.rootVisualElement.Q<EnumField>("EnmSpace");
            enmAir = document.rootVisualElement.Q<EnumField>("EnmAir");
            enmGround = document.rootVisualElement.Q<EnumField>("EnmGround");
            enmUnderwater = document.rootVisualElement.Q<EnumField>("EnmUnderwater");
            txtConcentration = document.rootVisualElement.Q<TextField>("TxtConcentration");
            txtReaction = document.rootVisualElement.Q<TextField>("TxtReaction");
            txtAbility = document.rootVisualElement.Q<TextField>("TxtAbility");
            txtPerception = document.rootVisualElement.Q<TextField>("TxtPerception");
            txtIntention = document.rootVisualElement.Q<TextField>("TxtIntention");
            txtEndurance = document.rootVisualElement.Q<TextField>("TxtEndurance");
            txtExpertise = document.rootVisualElement.Q<TextField>("TxtExpertise");
            txtSp = document.rootVisualElement.Q<TextField>("TxtSP");
            
            txtGrowthConcentration = document.rootVisualElement.Q<TextField>("TxtGrowthConcentration");
            txtGrowthReaction = document.rootVisualElement.Q<TextField>("TxtGrowthReaction");
            txtGrowthAbility = document.rootVisualElement.Q<TextField>("TxtGrowthAbility");
            txtGrowthPerception = document.rootVisualElement.Q<TextField>("TxtGrowthPerception");
            txtGrowthIntention = document.rootVisualElement.Q<TextField>("TxtGrowthIntention");
            txtGrowthEndurance = document.rootVisualElement.Q<TextField>("TxtGrowthEndurance");
            txtGrowthExpertise = document.rootVisualElement.Q<TextField>("TxtGrowthExpertise");
            txtGrowthSp = document.rootVisualElement.Q<TextField>("TxtGrowthSP");
            
            divGrowthSuitability = document.rootVisualElement.Q<VisualElement>("DivGrowthSuitability");
            btnAddGrowthSuitability = document.rootVisualElement.Q<Button>("BtnAddGrowthSuitability");
        }

        public void SetManager(CharacterEditorManager mng)
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
            btnSave.clicked += () =>
            {
                if (!Validation(mng)) return;
                mng.SaveCharacters();
            };
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
            btnDelete.clicked += mng.DeleteCharacter;
            btnClose.clicked += () =>
            {
                SceneManager.LoadScene(0);
            };

            drpCharacter.RegisterValueChangedCallback(v =>
            {
                mng.SetCharacter(v.newValue);
            });
            drpImage.RegisterValueChangedCallback(v =>
            {
                if (string.IsNullOrWhiteSpace(v.newValue))
                {
                    imgChara.style.backgroundImage = Texture2D.whiteTexture;
                    return;
                }
                var pathResource = Path.Combine(DataUtil.PathBase, "resource", "image", "character", v.newValue);
                var texture = DataUtil.GetImage(pathResource);
                imgChara.style.backgroundImage = texture;
            });

            btnMusicPlay.clicked += () =>
            {
                if (drpMusic.index == -1) return;
                mng.SetPlayMusic(drpMusic.value);
            };

            btnAddGrowthSuitability.clicked += () =>
            {
                var lv = 0;
                if (divGrowthSuitability.Children().Any())
                    lv = divGrowthSuitability.Children().Select(x => Convert.ToInt32(x.Q<TextField>("TxtLv").value)).Max() + 1;
                AddGrowthSuitabilityRecord(lv, false, Suitable.E, false, Suitable.E, false, Suitable.E, false, Suitable.E);
            };
            
            drpMusic.choices.Add(" ");
            foreach (var exp in new[]{"ogg", "mp3"})
            {
                foreach (var path in DataUtil.GetAudios($"*.{exp}"))
                { drpMusic.choices.Add(Path.GetFileName(path)); }
            }
            drpImage.choices.Add(" ");
            foreach (var exp in new[]{"png", "bmp", "tif", "tga", "jpg", "psd"})
            {
                foreach (var path in DataUtil.GetImages("character", $"*.{exp}"))
                { drpImage.choices.Add(Path.GetFileName(path)); }
            }
        }

        public void SetCharacter(CharacterData mngCharacter)
        {
            if (mngCharacter == null) return;
            txtCharacterId.value = mngCharacter.CharacterId;
            txtCharacterName.value = mngCharacter.CharacterName;
            drpMusic.value = mngCharacter.Music;
            drpImage.value = mngCharacter.CharacterImagePath;
            // var pathResource = Path.Combine(DataUtil.PathBase, "resource", "image", "character", mngCharacter.CharacterImagePath);
            // mngCharacter.CharacterImage = DataUtil.GetImage(pathResource);
            // imgChara.style.backgroundImage = mngCharacter.CharacterImage;
            
            // 地形適正
            enmSpace.value = mngCharacter.Space.suitable;
            enmAir.value = mngCharacter.Air.suitable;
            enmGround.value = mngCharacter.Ground.suitable;
            enmUnderwater.value = mngCharacter.Underwater.suitable;
            
            // 能力値
            txtConcentration.value = mngCharacter.Concentration.status.ToString();
            txtReaction.value = mngCharacter.Reaction.status.ToString();
            txtAbility.value = mngCharacter.Ability.status.ToString();
            txtPerception.value = mngCharacter.Perception.status.ToString();
            txtIntention.value = mngCharacter.Intention.status.ToString();
            txtEndurance.value = mngCharacter.Endurance.status.ToString();
            txtExpertise.value = mngCharacter.Expertise.status.ToString();
            txtSp.value = mngCharacter.SP.status.ToString();
            
            // 成長値
            txtGrowthConcentration.value = mngCharacter.GrowthConcentration.ToString();
            txtGrowthReaction.value = mngCharacter.GrowthReaction.ToString();
            txtGrowthAbility.value = mngCharacter.GrowthAbility.ToString();
            txtGrowthPerception.value = mngCharacter.GrowthPerception.ToString();
            txtGrowthIntention.value = mngCharacter.GrowthIntention.ToString();
            txtGrowthEndurance.value = mngCharacter.GrowthEndurance.ToString();
            txtGrowthExpertise.value = mngCharacter.GrowthExpertise.ToString();
            txtGrowthSp.value = mngCharacter.GrowthSP.ToString();
            
            divGrowthSuitability.Clear();
            Dictionary<int, GrowthSuitabilityData> growths = mngCharacter.GetGrowthSuitability();
            foreach (var lv in growths.Keys.OrderBy(x => x))
            {
                var growth = growths[lv];
                AddGrowthSuitabilityRecord(
                    lv, 
                    growth.EnabledSpace, growth.Space, 
                    growth.EnabledAir, growth.Air, 
                    growth.EnabledGround, growth.Ground, 
                    growth.EnabledUnderwater, growth.Underwater);
            }
        }

        public void UpdateCharacter(CharacterData mngCharacter, MessageData mngMessage)
        {
            if (mngCharacter == null) return;
            mngCharacter.CharacterName = txtCharacterName.value;
            mngCharacter.CharacterImagePath = drpImage.value.Trim();
            mngCharacter.Music = drpMusic.value.Trim();
            mngCharacter.Space.suitable = (Suitable)enmSpace.value;
            mngCharacter.Air.suitable = (Suitable)enmAir.value;
            mngCharacter.Ground.suitable = (Suitable)enmGround.value;
            mngCharacter.Underwater.suitable = (Suitable)enmUnderwater.value;
            mngCharacter.Concentration.status = Convert.ToInt32(txtConcentration.value);
            mngCharacter.Reaction.status = Convert.ToInt32(txtReaction.value);
            mngCharacter.Ability.status = Convert.ToInt32(txtAbility.value);
            mngCharacter.Perception.status = Convert.ToInt32(txtPerception.value);
            mngCharacter.Intention.status = Convert.ToInt32(txtIntention.value);
            mngCharacter.Endurance.status = Convert.ToInt32(txtEndurance.value);
            mngCharacter.Expertise.status = Convert.ToInt32(txtExpertise.value);
            mngCharacter.SP.status = Convert.ToInt32(txtSp.value);
            
            mngCharacter.GrowthConcentration = Convert.ToInt32(txtGrowthConcentration.value);
            mngCharacter.GrowthReaction = Convert.ToInt32(txtGrowthReaction.value);
            mngCharacter.GrowthAbility = Convert.ToInt32(txtGrowthAbility.value);
            mngCharacter.GrowthPerception = Convert.ToInt32(txtGrowthPerception.value);
            mngCharacter.GrowthIntention = Convert.ToInt32(txtGrowthIntention.value);
            mngCharacter.GrowthEndurance = Convert.ToInt32(txtGrowthEndurance.value);
            mngCharacter.GrowthExpertise = Convert.ToInt32(txtGrowthExpertise.value);
            mngCharacter.GrowthSP = Convert.ToInt32(txtGrowthSp.value);

            foreach (var growth in divGrowthSuitability.Children())
            {
                var lv = Convert.ToInt32(growth.Q<TextField>("TxtLv").value);
                var isSpace = growth.Q<Toggle>("TglSpace").value;
                var space = (Suitable)growth.Q<EnumField>("EnmSpace").value;
                var isAir = growth.Q<Toggle>("TglAir").value;
                var air = (Suitable)growth.Q<EnumField>("EnmAir").value;
                var isGround = growth.Q<Toggle>("TglGround").value;
                var ground = (Suitable)growth.Q<EnumField>("EnmGround").value;
                var isUnderwater = growth.Q<Toggle>("TglUnderwater").value;
                var underwater = (Suitable)growth.Q<EnumField>("EnmUnderwater").value;
                if (!isSpace && !isAir && !isGround && !isUnderwater) continue;
                if (isSpace) mngCharacter.Space.Growth[lv] = space;
                if (isAir) mngCharacter.Air.Growth[lv] = air;
                if (isGround) mngCharacter.Ground.Growth[lv] = ground;
                if (isUnderwater) mngCharacter.Underwater.Growth[lv] = underwater;
            }
            if (mngMessage == null) return;
        }

        private void AddGrowthSuitabilityRecord(
            int lv, 
            bool enableSpace, Suitable space, 
            bool enableAir, Suitable air, 
            bool enableGround, Suitable ground, 
            bool enableUnderwater, Suitable underwater)
        {
            var record = growthSuitabilityRecord.Instantiate();
            record.Q<TextField>("TxtLv").value = lv.ToString();
            AddGrowthSuitability(record, "Space", enableSpace, space);
            AddGrowthSuitability(record, "Air", enableAir, air);
            AddGrowthSuitability(record, "Ground", enableGround, ground);
            AddGrowthSuitability(record, "Underwater", enableUnderwater, underwater);
            record.Q<Button>("BtnRemoveGrowthSuitability").clicked += () =>
            {
                divGrowthSuitability.Remove(record);
            };
            divGrowthSuitability.Add(record);
        }

        private void AddGrowthSuitability(TemplateContainer template, string suitableName, bool isEnabled, Suitable suitable)
        {
            var enm = template.Q<EnumField>($"Enm{suitableName}");
            enm.value = suitable;
            enm.SetEnabled(isEnabled);
            var tgl = template.Q<Toggle>($"Tgl{suitableName}");
            tgl.RegisterValueChangedCallback(v =>
            {
                enm.SetEnabled(v.newValue);
            });
        }

        private bool Validation(CharacterEditorManager mng)
        {
            if (string.IsNullOrWhiteSpace(txtCharacterName.value))
            {
                mng.EditorWindowManager.SetWarning(LocaleUtil.GetMessage("E_S0001", LocaleUtil.GetEntry("lbl_character_name")));
                return false;
            }
            return true;
        }

        public DropdownField CharacterDropdown => drpCharacter;
        public bool IsOver => isOverTopToolbar || isOverToolWindow;
    }
}
