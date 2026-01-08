using UnityEngine;
using UnityEngine.UIElements;

namespace Functions.UI
{
    public abstract class BaseWindow : MonoBehaviour
    {
        protected UIDocument document;
        
        public void Awake()
        {
            document = GetComponent<UIDocument>();
            document.rootVisualElement.style.display = DisplayStyle.None;
            Setup();
        }
        
        public abstract void Setup();
        
        public void VisibleDisplay()
        {
            document.rootVisualElement.style.display = DisplayStyle.Flex;
        }

        public void HiddenDisplay()
        {
            document.rootVisualElement.style.display = DisplayStyle.None;
        }

        public bool IsDisplay()
        {
            return document.rootVisualElement.style.display == DisplayStyle.Flex;
        }
    }
}