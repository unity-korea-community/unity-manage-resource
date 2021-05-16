using System;
using System.Collections;
using UnityEngine;
using UNKO.ManageResource;

// namespace를 추가하면 어째선지 addcomponent가 되지않음;
// namespace UNKO.ManageResource
// {

[RequireComponent(typeof(AudioSource))]
public class SoundSlotComponent : SoundSlotComponentBase
{
    private const float const_deltaTime = 0.02f;

    [SerializeField]
    protected AudioSource _audioSource; public AudioSource audioSource => _audioSource;
    public override AudioClip clip { get => audioSource.clip; set => audioSource.clip = value; }

    public override float globalVolume
    {
        get => _globalVolume;
        set
        {
            _globalVolume = value;
            UpdateVolume();
        }
    }
    public override float localVolume
    {
        get => _localVolume;
        set
        {
            _localVolume = value;
            UpdateVolume();
        }
    }


    public override bool IsPlaying() => _audioSource.isPlaying;

    public override Coroutine Play()
    {
        return StartCoroutine(PlaySoundCoroutine());
    }

    public override void Reset()
    {
        gameObject.SetActive(true);
        Stop();
        SetLoop(false);
    }

    public override void SetLoop(bool isLoop)
    {
        audioSource.loop = isLoop;
    }

    public override void Stop()
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

    IEnumerator PlaySoundCoroutine()
    {
        audioSource.Play();

        float delayTime = 0f;
        if (audioSource.loop)
        {
            while (true)
            {
#if UNITY_EDITOR
                delayTime += const_deltaTime;
                name = $"{audioSource.clip.name}/{delayTime:F1}/{audioSource.clip.length:F1}_loop";
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
                name = $"{audioSource.clip.name}/{delayTime:F1}/{audioSource.clip.length:F1}";
#endif
                yield return new WaitForSeconds(const_deltaTime);
            }

            gameObject.SetActive(false);
        }
    }
}
// }
