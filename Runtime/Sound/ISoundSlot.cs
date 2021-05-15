using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNKO.ManageResource
{
    public interface ISoundSlot
    {
        event System.Action<ISoundSlot> OnPlayStartAudio;
        event System.Action<ISoundSlot> OnPlayFinishAudio;
        bool isPlaying { get; }

        ISoundSlot PlaySound(bool loop = false);
        ISoundSlot SetAudioClip(AudioClip clip);
        ISoundSlot SetVolume(float volume_0_1);
        void Stop();
    }
}
