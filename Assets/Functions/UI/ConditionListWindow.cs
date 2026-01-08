using System.Collections.Generic;
using Functions.Data;
using Functions.Data.Scripts;
using Functions.Enum;
using UnityEngine;
using UnityEngine.UIElements;
namespace Functions.UI
{
    public class ConditionListWindow : BaseWindow
    {
        [SerializeField]
        private VisualTreeAsset conditionRecord;
        private ScrollView listVictory;
        private ScrollView listDefeat;

        public override void Setup()
        {
            var victory = document.rootVisualElement.Q<VisualElement>("DivVictoryListWindow"); 
            listVictory = victory.Q<ScrollView>("List");
            var defeat = document.rootVisualElement.Q<VisualElement>("DivDefeatListWindow"); 
            listDefeat = defeat.Q<ScrollView>("List");
        }

        public void SetCondition(List<ConditionData> victory, List<ConditionData> defeat)
        {
            listVictory.Clear();
            listDefeat.Clear();
            AddCondition(listVictory, victory);
            AddCondition(listDefeat, defeat);
        }

        private void AddCondition(ScrollView view, List<ConditionData> conditions)
        {
            if (conditions.Count == 0)
            {
                var record = conditionRecord.Instantiate();
                var text = record.Q<Label>("Text");
                text.text = "なし";
                view.Add(record);
                return;
            }
            foreach (var condition in conditions)
            {
                var record = conditionRecord.Instantiate();
                var text = record.Q<Label>("Text");
                switch (condition.ObjectiveType)
                {
                    case ObjectiveType.Reach:
                        text.text = $"指定目標の指定座標への到達";
                        break;
                    case ObjectiveType.ReachAll:
                        text.text = $"全指定目標の指定座標への到達";
                        break;
                    case ObjectiveType.Destroy:
                        text.text = $"指定目標の撃破";
                        break;
                    case ObjectiveType.DestroyAll:
                        text.text = $"指定目標の全滅";
                        break;
                }
                view.Add(record);
            }
        }
    }
}
