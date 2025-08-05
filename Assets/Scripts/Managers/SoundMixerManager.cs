using UnityEngine;
using UnityEngine.Audio;

namespace Managers
{
    public class SoundMixerManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;

        public void SetMasterVolume(float volume)
        {
            audioMixer.SetFloat("MasterVolume", volume);
        }

        public void SetFxVolume(float volume)
        {
            audioMixer.SetFloat("SoundFXVolume", volume);
        }

        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("MusicVolume", volume);
        }
    }
}
