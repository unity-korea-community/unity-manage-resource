using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNKO.ManageResource
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundSlotComponent : MonoBehaviour, ISoundSlot
    {
        private const float const_deltaTime = 0.1f;

        [SerializeField]
        private AudioSource _audioSource; public AudioSource audioSource => _audioSource;

        public bool isPlaying => _audioSource.isPlaying;

        public event Action<ISoundSlot> OnPlayStartAudio;
        public event Action<ISoundSlot> OnPlayFinishAudio;

        public ISoundSlot PlaySound(bool loop = false)
        {
            StartCoroutine(PlaySoundCoroutine(loop));

            return this;
        }

        public ISoundSlot SetAudioClip(AudioClip clip)
        {
            audioSource.clip = clip;

            return this;
        }

        public ISoundSlot SetVolume(float volume_0_1)
        {
            audioSource.volume = volume_0_1;

            return this;
        }

        public void Stop()
        {
            audioSource.Stop();
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        IEnumerator PlaySoundCoroutine(bool loop)
        {
            audioSource.Play();
            OnPlayStartAudio?.Invoke(this);

            float delayTime = 0f;
            if (audioSource.loop)
            {
                while (true)
                {
#if UNITY_EDITOR
                    delayTime += const_deltaTime;
                    name = $"{audioSource.clip.name}/{delayTime:F1)}/{audioSource.clip.length}_loop";
#endif
                    yield return new WaitForSeconds(const_deltaTime);
                }
            }
            else
            {
                while (audioSource.isPlaying)
                {
#if UNITY_EDITOR
                    delayTime += const_deltaTime;
                    name = $"{audioSource.clip.name}/{delayTime:F1)}/{audioSource.clip.length}";
#endif
                    yield return new WaitForSeconds(const_deltaTime);
                }

                gameObject.SetActive(false);
            }

            // OnPlayFinishAudio?.Invoke(this);
        }
    }
}
