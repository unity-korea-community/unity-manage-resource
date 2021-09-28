using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

namespace UNKO.ManageResource
{
    public class SoundSlotForTest : SoundSlotComponentBase
    {
        public bool IsPlaying { get; private set; }
        public bool IsLoop { get; private set; }
        public bool IsMute { get; private set; }

        public override bool IsPlayingResource() => IsPlaying;
        [SerializeField] // NOTE: 이게 없으면 instantiate를 통해 copy를 못함
        private float _duration;

        public SoundSlotForTest SetDuration(float duration)
        {
            _duration = duration;

            return this;
        }

        public ISoundSlot SetAudioClip(AudioClip clip)
        {
            this.Clip = clip;

            return this;
        }

        public override IEnumerator PlayCoroutine()
        {
            IEnumerator routine = DummyPlay();
            StartCoroutine(routine);

            return routine;
        }

        IEnumerator DummyPlay()
        {
            IsPlaying = true;
            float remainTime = _duration;
            while (remainTime > 0f)
            {
                remainTime -= Time.deltaTime;
                yield return null;
            }

            IsPlaying = false;
        }

        public override void Stop()
        {
            IsPlaying = false;
        }

        public override void SetLoop(bool isLoop)
        {
            this.IsLoop = isLoop;
        }

        public override void Reset()
        {
        }

        public override void SetMute(bool mute)
        {
            IsMute = mute;
        }

        public override float GetCurrentVolume()
        {
            return IsMute ? 0f : GlobalVolume * LocalVolume;
        }
    }
}
