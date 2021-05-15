
using UnityEngine;

namespace UNKO.ManageResource
{
    public interface ISoundData
    {
        string GetSoundKey();
        AudioClip GetAudioClip();
        float GetVolume_0_1();
    }

    public interface ISoundManager
    {
        ISoundManager AddData<T>(params T[] soundData) where T : ISoundData;
        ISoundManager InitSoundSlot(System.Func<ISoundSlot> onCreateSlot, int initializeSize = 0);
        SoundPlayCommand GetSlot(string soundKey);
        SoundPlayCommand GetSlot(ISoundData data);
        SoundPlayCommand PlaySound(string soundKey);
        SoundPlayCommand PlaySound(ISoundData data);

        ISoundManager SetVolume(float volume_0_1);
        void StopAll();
    }
}