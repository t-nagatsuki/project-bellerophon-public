using System;
using UnityEngine;
using UnityEngine.UIElements;
namespace Functions.UI
{
    public class MessageWindow : BaseWindow
    {
        [SerializeField]
        private bool isPositionUp;
        private VisualElement divImage;
        private VisualElement divName;
        private VisualElement image;
        private Label lblName;
        private Label lblText;

        public override void Setup()
        {
            divImage = document.rootVisualElement.Q<VisualElement>("DivImage");
            divName = document.rootVisualElement.Q<VisualElement>("DivName");
            image = document.rootVisualElement.Q<VisualElement>("Image");
            lblName = document.rootVisualElement.Q<Label>("LblName");
            lblText = document.rootVisualElement.Q<Label>("LblText");
            if (isPositionUp)
            { document.rootVisualElement.Q<VisualElement>("DivSpace").style.display = DisplayStyle.None; }
        }

        public void SetMessage(Texture2D _img, string _name, string[] _text)
        {
            if (_img != null)
            {
                divImage.style.display = DisplayStyle.Flex;
                image.style.backgroundImage = _img;
            }
            else
            {
                divImage.style.display = DisplayStyle.None;
            }
            if (!String.IsNullOrWhiteSpace(_name))
            {
                divName.style.display = DisplayStyle.Flex;
                lblName.text = _name;
            }
            else
            {
                divName.style.display = DisplayStyle.None;
            }
            lblText.text = String.Join(Environment.NewLine, _text);
        }
    }
}
