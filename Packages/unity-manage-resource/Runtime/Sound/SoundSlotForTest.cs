using System;
using UnityEngine;

namespace UNKO.ManageResource
{
    public class SoundSlotForTest : ISoundSlot
    {
        public event Action<ISoundSlot> OnPlayStartAudio;
        public event Action<ISoundSlot> OnPlayFinishAudio;

        public AudioClip clip { get; private set; }
        public float volume_0_1 { get; private set; }

        public bool isPlaying { get; private set; }

        public ISoundSlot PlaySound(bool loop = false)
        {
            isPlaying = true;

            return this;
        }

        public ISoundSlot SetAudioClip(AudioClip clip)
        {
            this.clip = clip;

            return this;
        }

        public ISoundSlot SetVolume(float volume_0_1)
        {
            this.volume_0_1 = volume_0_1;

            return this;
        }

        public void Stop()
        {
            isPlaying = false;
        }
    }
}