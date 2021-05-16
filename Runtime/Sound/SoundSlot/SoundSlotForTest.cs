using System;
using UnityEngine;

namespace UNKO.ManageResource
{
    public class SoundSlotForTest : SoundSlotComponentBase
    {
        public bool isPlaying { get; private set; }
        public bool isLoop { get; private set; }
        public override AudioClip clip { get; set; }

        public override bool IsPlaying() => isPlaying;

        public ISoundSlot SetAudioClip(AudioClip clip)
        {
            this.clip = clip;

            return this;
        }

        public override Coroutine Play()
        {
            isPlaying = true;

            return null;
        }

        public override void Stop()
        {
            isPlaying = false;
        }

        public override void SetLoop(bool isLoop)
        {
            this.isLoop = isLoop;
        }

        public override void Reset()
        {
            Stop();
        }
    }
}
