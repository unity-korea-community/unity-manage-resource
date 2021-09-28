using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNKO.ManageResource;

// namespace를 추가하면 어째선지 addcomponent가 되지않음;
// namespace UNKO.ManageResource
// {

[RequireComponent(typeof(AudioSource))]
public class SoundSlotComponent : SoundSlotComponentBase
{
    private const float const_deltaTime = 0.02f;

    public AudioSource HasAudioSource => _audioSource;
    [SerializeField]
    protected AudioSource _audioSource;
    bool _isMute;

    public override float GlobalVolume
    {
        get => _globalVolume;
        set
        {
            _globalVolume = value;
            UpdateVolume();
        }
    }

    public override float LocalVolume
    {
        get => _localVolume;
        set
        {
            _localVolume = value;
            UpdateVolume();
        }
    }

    public override void InitSlot(AudioClip clip, string soundCategory, string soundKey)
    {
        base.InitSlot(clip, soundCategory, soundKey);

        _audioSource.clip = clip;
    }

    public override IEnumerator PlayCoroutine()
    {
        CancelInvoke(nameof(DeActive));
        IEnumerator routine = PlaySoundCoroutine();
        StartCoroutine(routine);

        return routine;
    }

    public override void Reset()
    {
        HasAudioSource.pitch = 1f;

        gameObject.SetActive(true);
        _audioSource.Stop();
        SetLoop(false);
    }

    public override bool IsPlayingResource() => _audioSource.isPlaying;
    public override float GetCurrentVolume() => _audioSource.volume;

    public override void SetLoop(bool isLoop)
    {
        _audioSource.loop = isLoop;
    }

    public override void Stop()
    {
        _audioSource.Stop();
    }

    public override void SetMute(bool mute)
    {
        _isMute = mute;
        UpdateVolume();
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private ISoundSlot UpdateVolume()
    {
        _audioSource.volume = _isMute ? 0f : _localVolume * _globalVolume;

        return this;
    }

    IEnumerator PlaySoundCoroutine()
    {
        _audioSource.Play();

        do
        {
            float delayTime = 0f;
            while (_audioSource.isPlaying)
            {
#if UNITY_EDITOR
                delayTime += const_deltaTime;

                string loopString = _audioSource.loop ? "_loop" : "";
                name = $"{_audioSource.clip.name}/{delayTime:F1}/{_audioSource.clip.length:F1}{loopString}";
#endif

                yield return new WaitForSecondsRealtime(const_deltaTime);
            }

            yield return null;

        } while (_audioSource.loop);

        // NOTE 코루틴에서 딜레이 없이 바로 DeActive 하는 경우
        // 코루틴을 기다리는 이 다음행이 실행이 안되서 풀링이 안됨
        Invoke(nameof(DeActive), 0.02f);
    }

    private void DeActive()
    {
        gameObject.SetActive(false);
    }
}
// }
