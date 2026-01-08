using UnityEngine;
using UnityEngine.UIElements;
namespace Functions.UI
{
    public class ErrorWindow : BaseWindow
    {
        private Label lblTitle;
        private Label lblError;

        public override void Setup()
        {
            lblTitle = document.rootVisualElement.Q<Label>("LblTitle");
            lblError = document.rootVisualElement.Q<Label>("LblError");
        }

        public void SetError(string err)
        {
            SetError("Error", err);
        }

        public void SetWarning(string err)
        {
            SetError("Warning", err);
        }

        public void SetError(string title, string err)
        {
            lblTitle.text = title;
            lblError.text = err;
            document.rootVisualElement.style.display = DisplayStyle.Flex;
        }
        
        public bool IsCritical()
        {
            return lblTitle.text == "Error";
        }
    }
}
