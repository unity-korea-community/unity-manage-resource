using System;
using System.Collections.Generic;
using UnityEngine;
using UNKO.Utils;

namespace UNKO.ManageResource
{
    public class SoundManagerComponent : SingletonComponentBase<SoundManagerComponent>, ISoundManager
    {
        [SerializeField]
        SoundManager _manager;

        protected override void InitSingleton()
        {
            base.InitSingleton();

            _manager = new SoundManager(this);
            SoundSystem.Init(this);
        }

        public ISoundManager AddData<T>(params T[] soundData) where T : ISoundData => _manager.AddData(soundData);
        public ISoundManager InitSoundSlot(Func<SoundSlotComponentBase> onCreateSlot, int initializeSize = 0) => InitSoundSlot(onCreateSlot, initializeSize);

        public SoundPlayCommand GetSlot(string soundKey) => _manager.GetSlot(soundKey);
        public SoundPlayCommand GetSlot(ISoundData data) => _manager.GetSlot(data);
        public SoundPlayCommand PlaySound(string soundKey) => _manager.PlaySound(soundKey);
        public SoundPlayCommand PlaySound(ISoundData data) => _manager.PlaySound(data);

        public bool TryGetData(string soundKey, out ISoundData data) => _manager.TryGetData(soundKey, out data);
        public ISoundManager SetVolume(float volume_0_1) => _manager.SetVolume(volume_0_1);
        public void StopAll() => _manager.StopAll();
    }
}
