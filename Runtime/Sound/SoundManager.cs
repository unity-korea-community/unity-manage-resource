using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UNKO.Utils;

namespace UNKO.ManageResource
{
    [Serializable]
    public class SoundManager : ISoundManager
    {
        public enum PoolingMode
        {
            Increase,
            Dummy,
        }

        // NOTE inspector 노출용 - 유니티 에디터는 제네릭 클래스를 인스펙터에 노출을 못하기 때문에
        [Serializable]
        public class SoundSlotPool : UnityComponentPool<SoundSlotComponentBase>
        {
            public SoundSlotPool(SoundSlotComponentBase originItem) : base(originItem) { }
            public SoundSlotPool(Func<SoundSlotComponentBase> onCreateInstance) : base(onCreateInstance) { }

            public SoundSlotPool(Func<SoundSlotComponentBase> onCreateInstance, int initializeSize) : base(onCreateInstance, initializeSize)
            {
            }
        }

        public event Action<ISoundSlot> OnPlaySound;
        public event Action<ISoundSlot> OnFinishSound;

        public SoundSlotPool SlotPool => _slotPool;
        public SoundPlayCommand.Pool CommandPool => _commandPool;

        [SerializeField]
        float _globalVolume_0_1 = 0.5f;
        [SerializeField] // NOTE:for debug
        SoundSlotPool _slotPool = null;
        [SerializeField] // NOTE:for debug
        SoundPlayCommand.Pool _commandPool = new SoundPlayCommand.Pool(new SoundPlayCommand());

        Dictionary<string, ISoundData> _data = new Dictionary<string, ISoundData>();
        Dictionary<string, bool> _muteBySoundCategory = new Dictionary<string, bool>();
        Dictionary<string, bool> _muteBySoundKey = new Dictionary<string, bool>();
        Dictionary<string, float> _localVolumeBySoundCategory = new Dictionary<string, float>();
        Dictionary<string, float> _localVolumeBySoundKey = new Dictionary<string, float>();

        MonoBehaviour _monoOwner;
        bool _isGlobalMute;


        public SoundManager(MonoBehaviour owner)
        {
            _monoOwner = owner;
            _slotPool = new SoundSlotPool(() =>
            {
                SoundSlotComponentBase soundSlotComponent = new GameObject(nameof(SoundSlotComponent)).AddComponent<SoundSlotComponent>();
                soundSlotComponent.transform.SetParent(_monoOwner.transform);

                AudioSource source = soundSlotComponent.gameObject.GetComponent<AudioSource>();
                source.playOnAwake = false;

                return soundSlotComponent;
            });
            _slotPool.SetParents(_monoOwner.transform);
        }

        public SoundManager(MonoBehaviour owner, Func<SoundSlotComponentBase> onCreateSlot)
        {
            _monoOwner = owner;
            _slotPool = new SoundSlotPool(onCreateSlot);
            // NOTE fail test case, _slotPool.SetParents(_monoOwner.transform);
        }

        public SoundManager()
        {
        }

        public ISoundManager InitSoundSlot(Func<SoundSlotComponentBase> onCreateSlot, int initializeSize = 0)
        {
            if (_slotPool != null)
            {
                _slotPool.Dispose();
            }
            _slotPool = new SoundSlotPool(onCreateSlot, initializeSize);
            _slotPool.SetParents(_monoOwner.transform);

            return this;
        }

        public ISoundManager AddData<T>(params T[] soundData)
            where T : ISoundData
        {
            soundData.Foreach(item => _data.Add(item.GetSoundKey(), item));
            return this;
        }

        public SoundPlayCommand PlaySound(string soundKey) => GetSlot(soundKey).PlayResource();
        public SoundPlayCommand PlaySound(ISoundData data) => GetSlot(data).PlayResource();

        public ISoundManager SetGlobalVolume(float volume_0_1)
        {
            _globalVolume_0_1 = volume_0_1;
            ForeachSlot(slot => true, slot => slot.SetGlobalVolume(_globalVolume_0_1));
            return this;
        }

        public ISoundManager SetVolumeBySoundCategory(string soundCategory, float localVolume_0_1)
        {
            _localVolumeBySoundCategory[soundCategory] = localVolume_0_1;
            return ForeachSlot(slot => slot.SoundCategory == soundCategory, slot => slot.SetLocalVolume(localVolume_0_1));
        }

        public ISoundManager SetVolumeBySoundKey(string soundKey, float localVolume_0_1)
        {
            _localVolumeBySoundKey[soundKey] = localVolume_0_1;
            return ForeachSlot(slot => slot.Soundkey == soundKey, slot => slot.SetLocalVolume(localVolume_0_1));
        }

        public ISoundManager SetMuteAll(bool mute)
        {
            _isGlobalMute = mute;
            return ForeachSlot(slot => true, slot => slot.SetMute(mute));
        }

        public ISoundManager SetMuteBySoundCategory(string soundCategory, bool mute)
        {
            _muteBySoundCategory[soundCategory] = mute;
            return ForeachSlot(slot => slot.SoundCategory == soundCategory, slot => slot.SetMute(mute));
        }

        public ISoundManager SetMuteBySoundKey(string soundKey, bool mute)
        {
            _muteBySoundKey[soundKey] = mute;
            return ForeachSlot(slot => slot.Soundkey == soundKey, slot => slot.SetMute(mute));
        }


        public bool TryGetData(string soundKey, out ISoundData data)
            => _data.TryGetValue(soundKey, out data);

        public float GetGlobalVolume()
            => _globalVolume_0_1;

        public SoundPlayCommand GetSlot(string soundKey)
        {
            if (TryGetData(soundKey, out ISoundData data) == false)
            {
                Debug.LogError($"{nameof(SoundManager)} - data not contain(id:{soundKey})");
            }

            return GetSlot(data);
        }

        public SoundPlayCommand GetSlot(ISoundData data)
        {
            // ISoundSlot unusedSlot = _slotPool.IsEmptyPool() ?
            ISoundSlot unusedSlot = _slotPool
                    .Spawn()
                    .SetGlobalVolume(_globalVolume_0_1);

            if (data != null)
            {
                AudioClip playClip = data.GetAudioClip(this);
                if (playClip == null)
                {
                    // Debug.LogError($"[{_monoOwner.name}.{nameof(GetSlot)}]soundKey:{data.GetSoundKey()} Clip is null", _monoOwner);
                }

                string soundCategory = data.GetSoundCategory();
                string soundKey = data.GetSoundKey();
                float localVolume = CalculateLocalVolume(data, soundCategory, soundKey);
                bool isMute = CalculateMute(soundCategory, soundKey);

                unusedSlot.InitSlot(playClip, soundCategory, soundKey);
                unusedSlot.SetLocalVolume(localVolume);
                unusedSlot.SetMute(isMute);
            }

            Debug.Log($"key:{data.GetSoundKey()}, play");

            SoundPlayCommand playCommand = _commandPool.Spawn();
            playCommand.Init(unusedSlot, DespawnCommand);
            OnPlaySound?.Invoke(unusedSlot);

            return playCommand;
        }

        private void DespawnCommand(ResourcePlayCommandBase<ISoundSlot> command)
        {
            _commandPool.DeSpawn(command as SoundPlayCommand);

            ISoundSlot soundSlot = command.ResourcePlayer;
            _slotPool.DeSpawn(soundSlot as SoundSlotComponentBase);
            OnFinishSound?.Invoke(soundSlot);
        }

        public void StopAll()
        {
            _slotPool.DeSpawnAll();
        }

        public void StopAll(Func<SoundSlotComponentBase, bool> OnFilter)
        {
            _slotPool.Use.Where(OnFilter).ToList().ForEach(_slotPool.DeSpawn);
        }

        private ISoundManager ForeachSlot(Func<SoundSlotComponentBase, bool> comparer, Action<SoundSlotComponentBase> onEach)
        {
            foreach (SoundSlotComponentBase slot in _slotPool.Use.Where(comparer))
            {
                onEach.Invoke(slot);
            }

            return this;
        }

        private float CalculateLocalVolume(ISoundData data, string soundCategory, string soundKey)
        {
            float localVolume = data.GetVolume_0_1(this);
            if (_localVolumeBySoundCategory.TryGetValue(soundCategory, out float localVolumeByCategory))
            {
                localVolume *= localVolumeByCategory;
            }

            if (_localVolumeBySoundKey.TryGetValue(soundKey, out float localVolumeByKey))
            {
                localVolume *= localVolumeByKey;
            }

            return localVolume;
        }

        private bool CalculateMute(string soundCategory, string soundKey)
        {
            bool isMute = _isGlobalMute;
            if (_muteBySoundCategory.TryGetValue(soundCategory, out bool muteByCategory))
            {
                isMute |= muteByCategory;
            }

            if (_muteBySoundKey.TryGetValue(soundKey, out bool muteByKey))
            {
                isMute |= muteByKey;
            }

            return isMute;
        }

        public ISoundManager ResetPool()
        {
            _slotPool.Dispose();
            _commandPool.Dispose();

            return this;
        }

        public ISoundManager ResetPool(Func<SoundSlotComponentBase, bool> onFilter)
        {
            _slotPool.Use.Where(onFilter).Foreach(_slotPool.OnDisposeItem);

            return this;
        }
    }
}
