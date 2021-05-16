using System;
using System.Collections.Generic;
using UnityEngine;
using UNKO.Utils;

namespace UNKO.ManageResource
{
    public abstract class SoundSlotComponentBase : MonoBehaviour, ISoundSlot
    {
        [SerializeField]
        protected float _globalVolume = 0.5f;
        [SerializeField]
        protected float _localVolume = 0.5f;

        public virtual float globalVolume { get => _globalVolume; set => _globalVolume = value; }
        public virtual float localVolume { get => _localVolume; set => _localVolume = value; }

        public abstract AudioClip clip { get; set; }

        public abstract bool IsPlaying();
        public abstract Coroutine Play();
        public abstract void Reset();
        public abstract void SetLoop(bool isLoop);
        public abstract void Stop();
    }

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
        UnitytComponentPool<SoundSlotComponentBase> _slotPool;
        [SerializeField]
        CommandPool _commandPool = new CommandPool(new SoundPlayCommand());
        MonoBehaviour _monoOwner;

        public SoundManager(MonoBehaviour owner)
        {
            _monoOwner = owner;
            _slotPool = new UnitytComponentPool<SoundSlotComponentBase>(() =>
            {
                SoundSlotComponent soundSlotComponent = new GameObject(nameof(SoundSlotComponent)).AddComponent<SoundSlotComponent>();
                soundSlotComponent.transform.SetParent(_monoOwner.transform);

                AudioSource source = soundSlotComponent.gameObject.GetComponent<AudioSource>();
                source.playOnAwake = false;

                return soundSlotComponent;
            });
        }

        public ISoundManager InitSoundSlot(System.Func<SoundSlotComponentBase> onCreateSlot, int initializeSize = 0)
        {
            _slotPool = new UnitytComponentPool<SoundSlotComponentBase>(onCreateSlot, initializeSize)
                .SetParents(_monoOwner.transform);

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

            foreach (var slot in _slotPool.use)
                slot.SetGlobalVolume(_volume_0_1);

            return this;
        }

        public SoundPlayCommand PlaySound(string soundKey) => GetSlot(soundKey).PlayResource();
        public SoundPlayCommand PlaySound(ISoundData data) => GetSlot(data).PlayResource();

        public SoundPlayCommand GetSlot(string soundKey)
        {
            if (TryGetData(soundKey, out var data) == false)
                Debug.LogError($"{nameof(SoundManager)} - data not contain(id:{soundKey})");

            return GetSlot(data);
        }
        public SoundPlayCommand GetSlot(ISoundData data)
        {
            ISoundSlot unusedSlot = _slotPool
                .Spawn()
                .SetGlobalVolume(_volume_0_1);

            if (data != null)
            {
                unusedSlot.clip = data.GetAudioClip(this);
                if (unusedSlot.clip == null)
                    Debug.LogError($"[{_monoOwner.name}.{nameof(GetSlot)}]soundKey:{data.GetSoundKey()} Clip is null", _monoOwner);

                unusedSlot.SetLocalVolume(data.GetVolume_0_1(this));
            }

            SoundPlayCommand command = _commandPool.Spawn();
            command.Init(unusedSlot, _monoOwner.StartCoroutine, _monoOwner.StopCoroutine, OnReleaseSlot);

            return command;
        }

        public bool TryGetData(string soundKey, out ISoundData data)
        {
            return _data.TryGetValue(soundKey, out data);
        }

        private void OnReleaseSlot(SoundPlayCommand command)
        {
            _commandPool.DeSpawn(command);
            _slotPool.DeSpawn(command.soundSlot as SoundSlotComponent);
        }

        public void StopAll()
        {
            _slotPool.DeSpawnAll();
        }
    }
}
