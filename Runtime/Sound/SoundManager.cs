using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNKO.Utils;

namespace UNKO.ManageResource
{
    [System.Serializable]
    public class SoundManager : ISoundManager
    {
        [System.Serializable]
        public class CommandPool : SimplePool<SoundPlayCommand>
        {
            public CommandPool(SoundPlayCommand originItem, int initializeSize = 0) : base(originItem, initializeSize)
            {
            }

            public CommandPool(Func<SoundPlayCommand> onCreateInstance, int initializeSize = 0) : base(onCreateInstance, initializeSize)
            {
            }
        }


        [SerializeField, Range(0, 1)]
        float _volume_0_1 = 0.5f;

        Dictionary<string, ISoundData> _data = new Dictionary<string, ISoundData>();
        SimplePool<ISoundSlot> _slotPool;
        [SerializeField]
        CommandPool _commandPool = new CommandPool(new SoundPlayCommand());

        System.Func<IEnumerator, Coroutine> _onStartCoroutine;
        System.Action<Coroutine> _onStopCoroutine;

        List<ISoundSlot> _playingSlots = new List<ISoundSlot>();

        public SoundManager()
        {
            _slotPool = new SimplePool<ISoundSlot>(() =>
                new GameObject(nameof(SoundPlayerComponent), typeof(AudioSource)).AddComponent<SoundSlotComponent>());
        }

        public void SetCoroutineFunc(System.Func<IEnumerator, Coroutine> onStartCoroutine, System.Action<Coroutine> onStopCoroutine)
        {
            _onStartCoroutine = onStartCoroutine;
            _onStopCoroutine = onStopCoroutine;
        }

        public ISoundManager InitSoundSlot(System.Func<ISoundSlot> onCreateSlot, int initializeSize = 0)
        {
            _slotPool = new SimplePool<ISoundSlot>(onCreateSlot, initializeSize);

            return this;
        }

        public ISoundManager AddData<T>(params T[] soundData)
            where T : ISoundData
        {
            foreach (ISoundData item in soundData)
            {
                _data.Add(item.GetSoundKey(), item);
            }

            return this;
        }

        public ISoundManager SetVolume(float volume_0_1)
        {
            _volume_0_1 = volume_0_1;
            foreach (var slot in _playingSlots)
                slot.SetGlobalVolume(_volume_0_1);

            return this;
        }

        public SoundPlayCommand PlaySound(string soundKey) => GetSlot(soundKey).PlayResource();
        public SoundPlayCommand PlaySound(ISoundData data) => GetSlot(data).PlayResource();

        public SoundPlayCommand GetSlot(string soundKey)
        {
            if (_data.TryGetValue(soundKey, out var data) == false)
            {
                Debug.LogError($"{nameof(SoundManager)} - data not contain(id:{soundKey})");
                return null;
            }

            return GetSlot(data);
        }

        public SoundPlayCommand GetSlot(ISoundData data)
        {
            ISoundSlot unusedSlot = _slotPool
                .Spawn()
                .SetLocalVolume(data.GetVolume_0_1())
                .SetGlobalVolume(_volume_0_1);
            unusedSlot.clip = data.GetAudioClip();
            _playingSlots.Add(unusedSlot);

            SoundPlayCommand command = _commandPool.Spawn();
            command.Init(unusedSlot, _onStartCoroutine, _onStopCoroutine, OnReleaseSlot);

            return command;
        }

        private void OnReleaseSlot(SoundPlayCommand command)
        {
            _commandPool.DeSpawn(command);
            _playingSlots.Remove(command);
        }

        public void StopAll()
        {
            _slotPool.DeSpawnAll();
        }
    }
}
