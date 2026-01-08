using UnityEngine;
using UnityEngine.UIElements;
namespace Functions.UI
{
    public class BattleConfirmWindow : BaseWindow
    {
        [SerializeField]
        private VisualTreeAsset templateButton;
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

        private BattleStatusPanel pnlOwn;
        private BattleStatusPanel pnlTarget;
    
        public override void Setup()
        {
            pnlOwn = new BattleStatusPanel(document.rootVisualElement.Q<VisualElement>("Own"), colorHpHigh, colorHpLow, colorHpDanger, colorEn, colorSp, templateButton);
            pnlTarget = new BattleStatusPanel(document.rootVisualElement.Q<VisualElement>("Target"), colorHpHigh, colorHpLow, colorHpDanger, colorEn, colorSp, templateButton);
        }

        public BattleStatusPanel Own => pnlOwn;

        public BattleStatusPanel Target => pnlTarget;
    }
}
