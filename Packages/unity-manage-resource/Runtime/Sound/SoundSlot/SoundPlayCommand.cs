
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using UNKO.Utils;

namespace UNKO.ManageResource
{
    [System.Serializable]
    public class SoundPlayCommand : IResourcePlayCommand, ISoundSlot
    {
        public event Action<IResourcePlayer> OnPlayStart = delegate { }; // avoid null check
        public event Action<IResourcePlayer> OnPlayFinish = delegate { };

        public ISoundSlot soundSlot { get; private set; }

        System.Func<IEnumerator, Coroutine> _onStartCoroutine;
        System.Action<Coroutine> _onStopCoroutine;
        System.Action<SoundPlayCommand> _onDispose;

        public float delay { get; set; }
        public AudioClip clip { get => soundSlot.clip; set => soundSlot.clip = value; }
        public float globalVolume { get => soundSlot.globalVolume; set => soundSlot.globalVolume = value; }
        public float localVolume { get => soundSlot.localVolume; set => soundSlot.localVolume = value; }

        Coroutine _coroutine;

        public void Init(ISoundSlot soundSlot, System.Func<IEnumerator, Coroutine> onStartCoroutine, System.Action<Coroutine> onStopCoroutine, System.Action<SoundPlayCommand> onDispose)
        {
            this.soundSlot = soundSlot;
            _onStartCoroutine = onStartCoroutine;
            _onStopCoroutine = onStopCoroutine;
            _onDispose = onDispose;

            Reset();
        }

        public void Reset()
        {
            delay = 0f;
            OnPlayStart = delegate { };
            OnPlayFinish = delegate { };
            soundSlot.Reset();
        }

        public bool IsPlaying() => soundSlot.IsPlaying();

        public Coroutine Play()
        {
            _coroutine = _onStartCoroutine(this.PlayCoroutine(soundSlot, OnPlayStart.Invoke, OnPlayFinish.Invoke));
            return _coroutine;
        }

        public void Stop()
        {
            if (_coroutine != null)
                _onStopCoroutine(_coroutine);
            soundSlot.Stop();
        }

        public void SetLoop(bool isLoop) => soundSlot.SetLoop(isLoop);

        public void Dispose()
        {
            _onDispose.Invoke(this);
        }
    }
}
