using UnityEngine;

namespace Managers
{
    public class SoundFXManager : MonoBehaviour
    {
        public static SoundFXManager instance;
        
        [SerializeField] private AudioSource _soundFXSource;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public void PlaySoundFXClip(AudioClip clip, Transform transform, float volume)
        {
            AudioSource audioSource = Instantiate(_soundFXSource, transform.position, Quaternion.identity);
            
            audioSource.clip = clip;
            
            audioSource.volume = volume;
            
            audioSource.Play();
            
            float clipLength = audioSource.clip.length;
            
            Destroy(audioSource.gameObject, clipLength);

        }
    }
}