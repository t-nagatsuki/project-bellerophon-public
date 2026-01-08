using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
namespace Functions.UI
{
    public class TelopWindow : BaseWindow
    {
        private Label lblTelop;
        private string telopText;
        private float telopTime;
        private float fadeTime;
        private float deltaTime;

        public override void Setup()
        {
            lblTelop = document.rootVisualElement.Q<Label>("LblTelop");
        }

        public void SetTelop(string[] texts, float time, float fade)
        {
            SetTelop(String.Join(Environment.NewLine, texts), time, fade);
        }

        private void SetTelop(string text, float time, float fade)
        {
            deltaTime = 0;
            telopText = text;
            telopTime = time;
            fadeTime = fade;
            lblTelop.text = string.Empty;
        }

        public void ClearTelop()
        {
            SetTelop(string.Empty, 0, 0);
        }

        public bool UpdateProcess(float delta)
        {
            if (!IsDisplay())
            { return false; }
            deltaTime += delta;
            var t = (int)math.ceil(math.lerp(0, telopText.Length, deltaTime / telopTime));
            if (t > telopText.Length)
            { t = telopText.Length; }
            lblTelop.text = telopText.Substring(0, t);
            if (deltaTime - telopTime > fadeTime)
            {
                // TODO : fadeout
                document.rootVisualElement.style.display = DisplayStyle.None;
            }
            return true;
        }
    }
}
