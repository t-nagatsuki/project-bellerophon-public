using Functions.Data;
using Functions.Data.Units;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Functions.UI
{
    public class StatusWindow : BaseWindow
    {
        [SerializeField]
        private Color32 colorHpHigh = Color.green;
        [SerializeField]
        private Color32 colorHpLow = Color.yellow;
        [SerializeField]
        private Color32 colorHpDanger = Color.red;
        [SerializeField]
        private Color32 colorEn = Color.cyan;
        [SerializeField]
        private Color32 colorSp = Color.magenta;
        [SerializeField]
        private VisualTreeAsset templateButton;

        private TabView tabView;
        private VisualElement imgUnit;
        private Label lblUnitName;
        private Label lblUnitSpace;
        private Label lblUnitAir;
        private Label lblUnitGround;
        private Label lblUnitUnderwater;
        private Label lblAccuracy;
        private Label lblManeuver;
        private Label lblPower;
        private Label lblArmor;
        private Label lblReduction;
        private Label lblMove;
        private ProgressBar barHp;
        private ProgressBar barEn;

        private VisualElement imgChara;
        private Label lblCharacterName;
        private Label lblLv;
        private Label lblSpace;
        private Label lblAir;
        private Label lblGround;
        private Label lblUnderwater;
        private Label lblConcentration;
        private Label lblReaction;
        private Label lblAbility;
        private Label lblPerception;
        private Label lblIntention;
        private Label lblEndurance;
        private Label lblExpertise;
        private ProgressBar barExp;
        private ProgressBar barSp;

        private VisualElement divWeapon;

        public override void Setup()
        {
            tabView = document.rootVisualElement.Q<TabView>("TabView");
            imgUnit = document.rootVisualElement.Q<VisualElement>("UnitImage");
            lblUnitName = document.rootVisualElement.Q<Label>("LblUnitName");
            lblUnitSpace = document.rootVisualElement.Q<Label>("LblUnitSpace");
            lblUnitAir = document.rootVisualElement.Q<Label>("LblUnitAir");
            lblUnitGround = document.rootVisualElement.Q<Label>("LblUnitGround");
            lblUnitUnderwater = document.rootVisualElement.Q<Label>("LblUnitUnderwater");
            lblAccuracy = document.rootVisualElement.Q<Label>("LblAccuracy");
            lblManeuver = document.rootVisualElement.Q<Label>("LblManeuver");
            lblPower = document.rootVisualElement.Q<Label>("LblPower");
            lblArmor = document.rootVisualElement.Q<Label>("LblArmor");
            lblReduction = document.rootVisualElement.Q<Label>("LblReduction");
            lblMove = document.rootVisualElement.Q<Label>("LblMove");
            barHp = document.rootVisualElement.Q<ProgressBar>("BarHP");
            barEn = document.rootVisualElement.Q<ProgressBar>("BarEN");

            imgChara = document.rootVisualElement.Q<VisualElement>("Image");
            lblCharacterName = document.rootVisualElement.Q<Label>("LblCharacterName");
            lblLv = document.rootVisualElement.Q<Label>("LblLV");
            lblSpace = document.rootVisualElement.Q<Label>("LblSpace");
            lblAir = document.rootVisualElement.Q<Label>("LblAir");
            lblGround = document.rootVisualElement.Q<Label>("LblGround");
            lblUnderwater = document.rootVisualElement.Q<Label>("LblUnderwater");
            lblConcentration = document.rootVisualElement.Q<Label>("LblConcentration");
            lblReaction = document.rootVisualElement.Q<Label>("LblReaction");
            lblAbility = document.rootVisualElement.Q<Label>("LblAbility");
            lblPerception = document.rootVisualElement.Q<Label>("LblPerception");
            lblIntention = document.rootVisualElement.Q<Label>("LblIntention");
            lblEndurance = document.rootVisualElement.Q<Label>("LblEndurance");
            lblExpertise = document.rootVisualElement.Q<Label>("LblExpertise");
            barExp = document.rootVisualElement.Q<ProgressBar>("BarEXP");
            barSp = document.rootVisualElement.Q<ProgressBar>("BarSP");

            divWeapon = document.rootVisualElement.Q<VisualElement>("DivWeapon");
        }

        public void SetStatus(PermanenceUnitData unit, PermanenceCharacterData chara)
        {
            tabView.selectedTabIndex = 0;
            // ユニット
            imgUnit.style.backgroundImage = unit.Unit.UnitImage;
            lblUnitName.text = unit.UnitName;
            lblUnitSpace.text = unit.Unit.Space.DisplayText;
            lblUnitAir.text = unit.Unit.Air.DisplayText;
            lblUnitGround.text = unit.Unit.Ground.DisplayText;
            lblUnitUnderwater.text = unit.Unit.Underwater.DisplayText;
            lblAccuracy.text = $"{unit.Unit.Accuracy.status}";
            lblManeuver.text = $"{unit.Unit.Maneuver.status}";
            lblPower.text = $"{unit.Unit.Power.status}";
            lblArmor.text = $"{unit.Unit.Armor.status}";
            lblReduction.text = $"{unit.Unit.Reduction.status}";
            lblMove.text = $"{unit.Unit.Move.status}";

            barHp.title = unit.HP.DisplayText;
            barHp.highValue = unit.HP.Max;
            barHp.value = unit.HP.Now;
            if ((double)unit.HP.Now / unit.HP.Max > 0.5)
            { barHp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorHpHigh); }
            else if ((double)unit.HP.Now / unit.HP.Max > 0.2)
            { barHp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorHpLow); }
            else
            { barHp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorHpDanger); }

            barEn.title = unit.EN.DisplayText;
            barEn.highValue = unit.EN.Max;
            barEn.value = unit.EN.Now;
            barEn.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorEn);

            // キャラクター
            imgChara.style.backgroundImage = chara.Character.CharacterImage;
            lblCharacterName.text = chara.CharacterName;
            lblLv.text = $"{chara.LV.Now}";
            lblSpace.text = chara.Space.DisplayText;
            lblAir.text = chara.Air.DisplayText;
            lblGround.text = chara.Ground.DisplayText;
            lblUnderwater.text = chara.Underwater.DisplayText;
            lblConcentration.text = $"{chara.Concentration.Now}";
            lblReaction.text = $"{chara.Reaction.Now}";
            lblAbility.text = $"{chara.Ability.Now}";
            lblPerception.text = $"{chara.Perception.Now}";
            lblIntention.text = $"{chara.Intention.Now}";
            lblEndurance.text = $"{chara.Endurance.Now}";
            lblExpertise.text = $"{chara.Expertise.Now}";

            barExp.title = chara.EXP.DisplayText;
            barExp.highValue = chara.EXP.Max;
            barExp.value = chara.EXP.Now;

            barSp.title = chara.SP.DisplayText;
            barSp.highValue = chara.SP.Max;
            barSp.value = chara.SP.Now;
            barSp.Q<VisualElement>(className: "unity-progress-bar__progress").style.backgroundColor = new StyleColor(colorSp);

            divWeapon.Clear();
            foreach (var wep in unit.Unit.Weapons)
            { divWeapon.Add(AddMenu(wep, FontStyle.Normal)); }
        }

        private Button AddMenu(WeaponData dat, FontStyle style)
        {
            var btn = templateButton.Instantiate();
            var elmtBtn = btn.Q<Button>("Button");
            var elmtWeaponName = elmtBtn.Q<Label>("WeaponName");
            var elmtRange = elmtBtn.Q<Label>("Range");
            var elmtSuitability = elmtBtn.Q<Label>("Suitability");
            var elmtAtk = elmtBtn.Q<Label>("Atk");
            var elmtBullets = elmtBtn.Q<Label>("Bullets");
            var elmtEnergy = elmtBtn.Q<Label>("Energy");
            var elmtSpecial = elmtBtn.Q<Label>("Special");
            elmtWeaponName.text = dat.WeaponName;
            if (dat.RangeMin != dat.RangeMax)
            { elmtRange.text = $"{dat.RangeMin} ～ {dat.RangeMax}"; }
            else
            { elmtRange.text = $"{dat.RangeMax}"; }
            elmtSuitability.text = $"{dat.Space.DisplayText}{dat.Air.DisplayText}{dat.Ground.DisplayText}{dat.Underwater.DisplayText}";
            elmtAtk.text = $"{dat.AttackPower}";
            if (dat.Bullets > 0)
            { elmtBullets.text = $"{dat.Bullets} / {dat.Bullets}"; }
            else
            { elmtBullets.text = "－"; }
            if (dat.Energy > 0)
            { elmtEnergy.text = $"{dat.Energy}"; }
            else
            { elmtEnergy.text = "－"; }
            // TODO : 特殊効果は要検討
            elmtSpecial.text = "";
            elmtBtn.style.unityFontStyleAndWeight = style;
            return elmtBtn;
        }
    }
}
