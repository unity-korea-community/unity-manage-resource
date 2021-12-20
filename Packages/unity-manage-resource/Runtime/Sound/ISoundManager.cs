using UnityEngine;

namespace UNKO.ManageResource
{
    public interface ISoundData
    {
        string GetSoundKey();
        string GetSoundCategory();
        AudioClip GetAudioClip(ISoundManager manager);
        float GetVolume_0_1(ISoundManager manager);
    }

    public class SoundDataDummy : ISoundData
    {
        string _key;
        AudioClip _clip;
        float _volume;
        string _soundCategory;

        public SoundDataDummy(string key, AudioClip clip, float volume = 1, string soundCategory = "")
        {
            _key = key;
            _clip = clip;
            _volume = volume;
            _soundCategory = soundCategory;
        }

        public AudioClip GetAudioClip(ISoundManager manager)
            => _clip;

        public string GetSoundCategory()
            => _soundCategory;

        public string GetSoundKey()
            => _key;

        public float GetVolume_0_1(ISoundManager manager)
            => _volume;
    }

    public interface ISoundManager
    {
        event System.Action<ISoundSlot> OnPlaySound;
        event System.Action<ISoundSlot> OnFinishSound;

        ISoundManager AddData<T>(params T[] soundData) where T : ISoundData;
        ISoundManager ResetPool();
        ISoundManager ResetPool(System.Func<SoundSlotComponentBase, bool> onFilter);

        ISoundManager InitSoundSlot(System.Func<SoundSlotComponentBase> onCreateSlot, int initializeSize = 0);
        SoundPlayCommand PlaySound(string soundKey);
        SoundPlayCommand PlaySound(ISoundData data);

        SoundPlayCommand GetSlot(string soundKey);
        SoundPlayCommand GetSlot(ISoundData data);
        bool TryGetData(string soundKey, out ISoundData data);
        float GetGlobalVolume();

        ISoundManager SetMuteAll(bool mute);
        ISoundManager SetMuteBySoundCategory(string soundCategory, bool mute);
        ISoundManager SetMuteBySoundKey(string soundKey, bool mute);
        ISoundManager SetGlobalVolume(float volume_0_1);
        ISoundManager SetVolumeBySoundCategory(string soundCategory, float volume_0_1);
        ISoundManager SetVolumeBySoundKey(string soundKey, float volume_0_1);

        void StopAll();
        void StopAll(System.Func<SoundSlotComponentBase, bool> OnFilter);
    }
}
