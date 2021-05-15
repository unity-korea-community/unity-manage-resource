using System.Collections.Generic;
using UnityEngine;
using UNKO.Utils;

namespace UNKO.ManageResource
{
    public interface ISoundData
    {
        string GetSoundKey();
        AudioClip GetAudioClip();
        float GetVolume_0_1();
    }

    [System.Serializable]
    public class SoundManager
    {
        public class SoundSlotPool : SimplePool<ISoundSlot>
        {
            System.Func<ISoundSlot> _OnCreateInstance;

            public SoundSlotPool(System.Func<ISoundSlot> OnCreateInstance, int initializeSize = 0)
            {
                ISoundSlot origin = OnCreateInstance();
                _OnCreateInstance = OnCreateInstance;

                Init(origin, initializeSize);
            }

            protected override ISoundSlot OnRequireNewInstance(ISoundSlot originItem)
            {
                return _OnCreateInstance.Invoke();
            }
        }

        Dictionary<string, ISoundData> _data = new Dictionary<string, ISoundData>();
        SoundSlotPool _pool;

        [SerializeField, Range(0, 1)]
        float _volume_0_1 = 0.5f;

        public SoundManager()
        {
            _pool = new SoundSlotPool(() =>
                new GameObject(nameof(SoundPlayerComponent), typeof(AudioSource)).AddComponent<SoundSlotComponent>());
        }

        public SoundManager InitSoundSlot(System.Func<ISoundSlot> onCreateSlot, int initializeSize = 0)
        {
            _pool = new SoundSlotPool(onCreateSlot, initializeSize);

            return this;
        }

        public SoundManager AddData<T>(params T[] soundData)
            where T : ISoundData
        {
            foreach (ISoundData item in soundData)
            {
                _data.Add(item.GetSoundKey(), item);
            }

            return this;
        }

        public SoundManager SetVolume(float volume_0_1)
        {
            _volume_0_1 = volume_0_1;

            return this;
        }

        public ISoundSlot PlaySound(string soundID) => PlaySound(soundID, false);
        public ISoundSlot PlaySound(string soundID, bool loop)
        {
            if (_data.TryGetValue(soundID, out var data) == false)
            {
                Debug.LogError($"{nameof(SoundManager)} - data not contain(id:{soundID})");
                return null;
            }

            ISoundSlot unusedSlot = _pool
                .Spawn()
                .SetAudioClip(data.GetAudioClip())
                .SetVolume(_volume_0_1 * data.GetVolume_0_1())
                .PlaySound(loop);

            return unusedSlot;
        }

        public void StopAll()
        {
            _pool.DeSpawnAll();
        }
    }
}
