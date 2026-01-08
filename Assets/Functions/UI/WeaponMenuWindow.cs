using Functions.Data;
using Functions.Data.Units;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Functions.UI
{
    public class WeaponMenuWindow : BaseWindow
    {
        [SerializeField]
        private VisualTreeAsset templateButton;

        private VisualElement divMenu;

        public override void Setup()
        {
            divMenu = document.rootVisualElement.Q<VisualElement>("DivMenu");
        }

        public Button AddMenu(WeaponData _dat, FontStyle _style)
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
            elmtWeaponName.text = _dat.WeaponName;
            if (_dat.RangeMin != _dat.RangeMax)
            { elmtRange.text = $"{_dat.RangeMin} ～ {_dat.RangeMax}"; }
            else
            { elmtRange.text = $"{_dat.RangeMax}"; }
            elmtSuitability.text = $"{_dat.Space.DisplayText}{_dat.Air.DisplayText}{_dat.Ground.DisplayText}{_dat.Underwater.DisplayText}";
            elmtAtk.text = $"{_dat.AttackPower}";
            if (_dat.Bullets > 0)
            { elmtBullets.text = $"{_dat.Bullets} / {_dat.Bullets}"; }
            else
            { elmtBullets.text = "－"; }
            if (_dat.Energy > 0)
            { elmtEnergy.text = $"{_dat.Energy}"; }
            else
            { elmtEnergy.text = "－"; }
            // TODO : 特殊効果は要検討
            elmtSpecial.text = "";
            elmtBtn.style.unityFontStyleAndWeight = _style;
            divMenu.Add(btn);
            return elmtBtn;
        }

        public void ClearButton()
        {
            divMenu.Clear();
        }
    }
}
