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

        public event Action<IResourcePlayer> OnPlayStart;
        public event Action<IResourcePlayer> OnPlayFinish;

        [SerializeField]
        private AudioSource _audioSource; public AudioSource audioSource => _audioSource;
        [SerializeField]
        private float _globalVolume = 0.5f;
        [SerializeField]
        private float _localVolume = 0.5f;

        public AudioClip clip { get => audioSource.clip; set => audioSource.clip = value; }
        public float globalVolume { get => _globalVolume; set => _globalVolume = value; }
        public float localVolume { get => _localVolume; set => _localVolume = value; }

        public bool IsPlaying() => _audioSource.isPlaying;


        public Coroutine Play()
        {
            return StartCoroutine(PlaySoundCoroutine(false));
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void SetDelay(float delay)
        {
            throw new NotImplementedException();
        }

        public void SetLoop(bool isLoop)
        {
            throw new NotImplementedException();
        }

        public ISoundSlot SetGlobalVolume(float volume_0_1)
        {
            _globalVolume = volume_0_1;
            return UpdateVolume();
        }

        public ISoundSlot SetLocalVolume(float volume_0_1)
        {
            _localVolume = volume_0_1;
            return UpdateVolume();
        }

        public void Stop()
        {
            audioSource.Stop();
        }


        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private ISoundSlot UpdateVolume()
        {
            audioSource.volume = _localVolume * _globalVolume;

            return this;
        }

        IEnumerator PlaySoundCoroutine(bool loop)
        {
            audioSource.Play();
            OnPlayStart?.Invoke(this);

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

            OnPlayFinish?.Invoke(this);
        }
    }
}
