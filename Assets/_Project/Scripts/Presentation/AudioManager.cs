using UnityEngine;

namespace _Project.Scripts.Presentation
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")] 
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource ambientSource;

        [Header("Volumes")] [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 0.8f;
        [Range(0f, 1f)] public float sfxVolume = 1f;
        [Range(0f, 1f)] public float ambientVolume = 0.6f;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void PlaySfx(AudioClip clip, float volumeMultiplier = 1f)
        {
            if (clip == null || sfxSource == null) return;
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume * volumeMultiplier);
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (musicSource == null || clip == null) return;
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.Play();
        }

        public void PlayAmbient(AudioClip clip, bool loop = true)
        {
            if (ambientSource == null || clip == null) return;
            ambientSource.clip = clip;
            ambientSource.loop = loop;
            ambientSource.volume = ambientVolume * masterVolume;
            ambientSource.Play();
        }

        public void StopMusic() => musicSource?.Stop();
        public void StopAmbient() => ambientSource?.Stop();
    }
}