using System;
using UnityEngine;
using UNKO.Utils;

namespace UNKO.ManageResource
{
    public class SoundManagerComponent : SingletonComponentBase<SoundManagerComponent>, ISoundManager
    {
        [SerializeField]
        SoundManager _manager = null; public SoundManager Manager => _manager;

        public event Action<ISoundSlot> OnPlaySound
        {
            add => _manager.OnPlaySound += value;
            remove => _manager.OnPlaySound -= value;
        }

        public event Action<ISoundSlot> OnFinishSound
        {
            add => _manager.OnFinishSound += value;
            remove => _manager.OnFinishSound -= value;
        }

        public override void InitSingleton()
        {
            base.InitSingleton();

            _manager = new SoundManager(this);
            SoundSystem.Init(this);
            DontDestroyOnLoad(this);
        }

        public ISoundManager AddData<T>(params T[] soundData) where T : ISoundData => _manager.AddData(soundData);
        public ISoundManager InitSoundSlot(Func<SoundSlotComponentBase> onCreateSlot, int initializeSize = 0) => _manager.InitSoundSlot(onCreateSlot, initializeSize);

        public SoundPlayCommand PlaySound(string soundKey) => _manager.PlaySound(soundKey);
        public SoundPlayCommand PlaySound(ISoundData data) => _manager.PlaySound(data);

        public SoundPlayCommand GetSlot(string soundKey) => _manager.GetSlot(soundKey);
        public SoundPlayCommand GetSlot(ISoundData data) => _manager.GetSlot(data);

        public float GetGlobalVolume() => _manager.GetGlobalVolume();
        public bool TryGetData(string soundKey, out ISoundData data)
            => _manager.TryGetData(soundKey, out data);


        public ISoundManager SetGlobalVolume(float volume_0_1)
            => _manager.SetGlobalVolume(volume_0_1);

        public ISoundManager SetMuteAll(bool mute)
            => _manager.SetMuteAll(mute);

        public ISoundManager SetMuteBySoundCategory(string soundCategory, bool mute)
            => _manager.SetMuteBySoundCategory(soundCategory, mute);

        public ISoundManager SetMuteBySoundKey(string soundKey, bool mute)
            => _manager.SetMuteBySoundKey(soundKey, mute);

        public ISoundManager SetVolumeBySoundCategory(string soundCategory, float volume_0_1)
            => _manager.SetVolumeBySoundCategory(soundCategory, volume_0_1);

        public ISoundManager SetVolumeBySoundKey(string soundKey, float volume_0_1)
            => _manager.SetVolumeBySoundKey(soundKey, volume_0_1);

        public void StopAll() => _manager.StopAll();

        public void StopAll(Func<SoundSlotComponentBase, bool> OnFilter)
            => _manager.StopAll(OnFilter);

        public ISoundManager ResetPool()
            => _manager.ResetPool();

        public ISoundManager ResetPool(Func<SoundSlotComponentBase, bool> onFilter)
            => _manager.ResetPool(onFilter);

    }
}
