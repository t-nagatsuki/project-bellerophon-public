using System;
using Functions.Manager;
using Functions.Util;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
namespace Functions.UI
{
    public class VariableInputWindow : BaseWindow
    {
        private Label lblText;
        private Label lblError;
        private TextField txtInput;
        private Button btnOk;

        private Action btnOkAction;

        public override void Setup()
        {
            lblText = document.rootVisualElement.Q<Label>("LblText");
            lblError = document.rootVisualElement.Q<Label>("LblError");
            txtInput = document.rootVisualElement.Q<TextField>("TxtInput");
            btnOk = document.rootVisualElement.Q<Button>("BtnOk");
        }

        public void SetupDisplay(SlgSceneManager mng, string[] text, string scope, string variableName, int min, int max)
        {
            lblError.text = string.Empty;
            lblText.text = mng.ScriptManager.AnalysisEmbeddedvariable(String.Join(Environment.NewLine, text));
            txtInput.maxLength = max;

            btnOkAction = () =>
            {
                if (txtInput.text.Length < min)
                {
                    lblError.text = LocaleUtil.GetMessage("E_I0001", min, max);
                    return;
                }
                btnOk.clicked -= btnOkAction;
                mng.ScriptManager.SetVariable(scope, variableName, txtInput.value);
                document.rootVisualElement.style.display = DisplayStyle.None;
            };
            btnOk.clicked += btnOkAction;
        }
    }
}
