using UnityEngine;

namespace Functions.Manager
{
    public class IntermissionSceneManager : MonoBehaviour
    {
        private string nextStageClass;
        private string nextStageMethod;

        private void Start()
        {
        }

        public string NextStageClass
        {
            get => nextStageClass;
            set => nextStageClass = value;
        }

        public string NextStageMethod
        {
            get => nextStageMethod;
            set => nextStageMethod = value;
        }
    }
}
