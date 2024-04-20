using UnityEngine;

namespace Audio
{
    public class SingletonMusic : MonoBehaviour
    {
        public static SingletonMusic instance;

        [SerializeField]
        private AudioClip[] musicClips; 

        private AudioSource audioSource;
        private int currentTrackIndex = 0;

        void Awake()
        {
            ManageSingleton();
            SetupAudioSource();
        }

        private void ManageSingleton()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void SetupAudioSource()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) 
            { 
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.clip = musicClips[currentTrackIndex];
            audioSource.loop = false; 
            audioSource.Play();
        }

        void Update()
        {
            if (!audioSource.isPlaying)
            {
                PlayNextTrack();
            }
        }

        private void PlayNextTrack()
        {
            currentTrackIndex = (currentTrackIndex + 1) % musicClips.Length; 
            audioSource.clip = musicClips[currentTrackIndex];
            audioSource.Play();
        }
    }
}