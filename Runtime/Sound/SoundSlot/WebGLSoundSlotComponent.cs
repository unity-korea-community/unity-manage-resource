using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNKO.ManageResource;

/// <summary>
/// https://forum.unity.com/threads/audio-distortion-crackling-on-ios-devices.1111855/
/// 그래서 HTML
/// </summary>
public class WebGLSoundSlotComponent : SoundSlotComponentBase
{
    private const float const_deltaTime = 0.02f;

    string _soundKey;
    float _volume;
    bool _isMute;
    bool _loop;

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

        _soundKey = soundKey;
        this.Reset();
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
        gameObject.SetActive(true);

        if (!string.IsNullOrEmpty(_soundKey))
        {
            WebSoundLibrary.Pause(_soundKey);
            SetLoop(false);
        }
    }

    public void SetPitch(float pitch)
    {
        if (!string.IsNullOrEmpty(_soundKey))
        {
            WebSoundLibrary.SetPitch(_soundKey, pitch);
        }
    }

    public override bool IsPlayingResource()
    {
        return true;
    }

    public override float GetCurrentVolume()
        => _volume;

    public override void SetLoop(bool isLoop)
    {
        _loop = isLoop;
        if (!string.IsNullOrEmpty(_soundKey))
        {
            WebSoundLibrary.SetLoop(_soundKey, isLoop);
        }
    }

    public override void SetMute(bool mute)
    {
        if (!string.IsNullOrEmpty(_soundKey))
        {
            WebSoundLibrary.SetMute(_soundKey, mute);
        }
    }

    public override void Stop()
    {
        if (!string.IsNullOrEmpty(_soundKey))
        {
            Debug.Log($"Stop Sound key:{_soundKey}");
            WebSoundLibrary.Stop(_soundKey);
            WebSoundLibrary.SetMute(_soundKey, false);
        }
    }

    private ISoundSlot UpdateVolume()
    {
        if (!string.IsNullOrEmpty(_soundKey))
        {
            WebSoundLibrary.SetMute(_soundKey, _isMute);

            _volume = _isMute ? 0f : _localVolume * _globalVolume;
            WebSoundLibrary.SetVolume(_soundKey, _volume);
        }

        return this;
    }

    IEnumerator PlaySoundCoroutine()
    {
        Debug.Log($"Play Sound key:{_soundKey}");
        WebSoundLibrary.PlaySound(_soundKey);

        do
        {

            float waitSecondTotal = WebSoundLibrary.GetDuration(_soundKey);
#if UNITY_EDITOR
            if (waitSecondTotal == -1f)
            {
                waitSecondTotal = 80f;
            }
#endif
            waitSecondTotal += 2f; // NOTE 자꾸 먼저 멈추는 경우가 있어서 임의로 늘림

            float waitSecond = waitSecondTotal;
            while (waitSecond > 0f)
            {

#if UNITY_EDITOR
                string loopString = _loop ? "_loop" : "";
                name = $"{_soundKey}/{waitSecond:F1}/{waitSecondTotal:F1}{loopString}";
#endif

                waitSecond -= Time.deltaTime;
                yield return null;
            }
        }
        while (_loop);

        DeActive();
    }

    private void DeActive()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        this.Stop();
    }
}
