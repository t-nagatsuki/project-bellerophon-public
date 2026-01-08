using System;
using System.Collections.Generic;
using Functions.Json;
using Functions.Manager;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
namespace Functions.UI
{
    public class ChoiceWindow : BaseWindow
    {
        [SerializeField]
        private VisualTreeAsset choiceItem;

        private Label lblText;
        private VisualElement elmChoice;

        public override void Setup()
        {
            lblText = document.rootVisualElement.Q<Label>("LblText");
            elmChoice = document.rootVisualElement.Q<VisualElement>("Choices");
        }

        public void SetupChoice(SlgSceneManager mng, string[] text, ChoiceJson[] choices)
        {
            lblText.text = mng.ScriptManager.AnalysisEmbeddedvariable(String.Join(Environment.NewLine, text));
            elmChoice.Clear();

            foreach (var choice in choices)
            {
                var item = choiceItem.Instantiate();
                var btn = item.Q<Button>("BtnChoice");
                btn.text = String.Join(Environment.NewLine, mng.ScriptManager.AnalysisEmbeddedvariable(choice.text));
                void Action()
                {
                    mng.ScriptManager.CallMethod(choice.name, choice.method);
                    HiddenDisplay();
                }
                btn.clicked += Action;
                elmChoice.Add(item);
            }
        }

        public void SetupYesNoChoice(SlgSceneManager mng, string[] text, ChoiceJson[] choices)
        {
            lblText.text = mng.ScriptManager.AnalysisEmbeddedvariable(String.Join(Environment.NewLine, text));
            elmChoice.Clear();

            foreach (var choice in choices)
            {
                var item = choiceItem.Instantiate();
                var btn = item.Q<Button>("BtnChoice");
                btn.text = String.Join(Environment.NewLine, mng.ScriptManager.AnalysisEmbeddedvariable(choice.text));
                void Action()
                {
                    mng.ScriptManager.CallMethod(choice.name, choice.method);
                    HiddenDisplay();
                }
                btn.clicked += Action;
                elmChoice.Add(item);
            }
        }
    }
}
