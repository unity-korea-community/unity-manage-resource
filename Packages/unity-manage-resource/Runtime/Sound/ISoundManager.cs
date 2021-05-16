
using UnityEngine;

namespace UNKO.ManageResource
{
    public interface ISoundData
    {
        string GetSoundKey();
        AudioClip GetAudioClip(ISoundManager manager);
        float GetVolume_0_1(ISoundManager manager);
    }

    public interface ISoundManager
    {
        ISoundManager AddData<T>(params T[] soundData) where T : ISoundData;
        ISoundManager InitSoundSlot(System.Func<SoundSlotComponentBase> onCreateSlot, int initializeSize = 0);
        SoundPlayCommand GetSlot(string soundKey);
        SoundPlayCommand GetSlot(ISoundData data);
        SoundPlayCommand PlaySound(string soundKey);
        SoundPlayCommand PlaySound(ISoundData data);

        bool TryGetData(string soundKey, out ISoundData data);
        ISoundManager SetVolume(float volume_0_1);
        void StopAll();
    }
}
