using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace Functions.UI
{
    public class BackImageWindow
        : MonoBehaviour
    {
        private UIDocument document;
        private VisualElement image;
        private bool isTransition;

        private void Awake()
        {
            document = GetComponent<UIDocument>();
            document.rootVisualElement.style.display = DisplayStyle.None;
            image = document.rootVisualElement.Q<VisualElement>("Image");
            image.RegisterCallback<TransitionEndEvent>(OnEndTransitionEvent);
        }

        public void SetImage(Texture2D tex, Color32 color)
        {
            image.style.backgroundImage = tex;
            document.rootVisualElement.style.display = DisplayStyle.Flex;
            SetImageColor(color, 0);
        }

        public void SetImageColor(Color32 color, float fade)
        {
            if (fade > 0)
            {
                isTransition = true;
            }
            image.style.transitionDuration = new List<TimeValue> { new TimeValue(fade, TimeUnit.Millisecond) };
            image.style.unityBackgroundImageTintColor = new StyleColor( color);
        }

        public void HiddenDisplay()
        {
            isTransition = false;
            if (image.style.transitionDuration.value != null)
            { image.style.transitionDuration.value.Clear(); }
            image.style.backgroundImage = null;
            image.style.unityBackgroundImageTintColor = Color.clear;
            document.rootVisualElement.style.display = DisplayStyle.None;
        }

        public bool IsDisplay()
        {
            return document.rootVisualElement.style.display == DisplayStyle.Flex;
        }
        
        private void OnEndTransitionEvent(ITransitionEvent evt)
        {
            isTransition = false;
        }
        
        public bool IsTransition
        {
            get => isTransition;
        }
    }
}
