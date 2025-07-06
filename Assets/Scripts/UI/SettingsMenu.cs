using UnityEngine;
using UnityEngine.Audio;

namespace Scripts
{
    public class SettingsMenu : MonoBehaviour
    {
        
        [SerializeField] AudioMixer audioMixer;
        
        public void SetVolume(float volume)
        {
            audioMixer.SetFloat("volume", volume);
        }
    }
}