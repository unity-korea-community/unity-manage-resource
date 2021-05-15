using System;
using UnityEngine;

namespace UNKO.ManageResource
{
    public class SoundSlotForTest : ISoundSlot
    {
        public AudioClip clip { get; set; }
        public bool isPlaying { get; private set; }
        public bool isLoop { get; private set; }

        public float globalVolume { get; set; }
        public float localVolume { get; set; }

        public bool IsPlaying() => isPlaying;

        public ISoundSlot SetAudioClip(AudioClip clip)
        {
            this.clip = clip;

            return this;
        }

        public void Stop()
        {
            isPlaying = false;
        }

        public Coroutine Play()
        {
            isPlaying = true;

            return null;
        }

        public void SetLoop(bool isLoop)
        {
            this.isLoop = isLoop;
        }

        public void Reset()
        {
            Stop();
        }

    }
}