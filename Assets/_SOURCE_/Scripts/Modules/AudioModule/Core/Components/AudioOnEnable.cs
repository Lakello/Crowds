namespace AudioModule.Components
{
    using AudioModule.Extensions;
    using UnityEngine;

    public class AudioOnEnable : MonoBehaviour
    {
        [SerializeField] private AudioID _id;
        
        private void OnEnable()
        {
            _id.PlayOneShot();
        }
    }
}