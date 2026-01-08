using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

namespace Functions.Effect
{
    public class BlastControl : MonoBehaviour
    {
        [SerializeField] public UnityEvent OnStop = new UnityEvent();
        
        void Start()
        {
            var main = GetComponent<ParticleSystem>().main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        void OnParticleSystemStopped()
        {
            OnStop.Invoke();
            Destroy(gameObject);
        }
    }
}