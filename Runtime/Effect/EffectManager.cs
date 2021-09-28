using System.Collections.Generic;
using UnityEngine;
using UNKO.Utils;

namespace UNKO.ManageResource
{
    [System.Serializable]
    public class EffectManager : IEffectManager
    {
        public EffectPlayCommand.Pool CommandPool => _commandPool;

        public GameObject gameObject => _owner.gameObject;

        [SerializeField] // NOTE:for debug
        EffectPlayCommand.Pool _commandPool = new EffectPlayCommand.Pool(new EffectPlayCommand());

        Dictionary<string, IEffectData> _data = new Dictionary<string, IEffectData>();
        Dictionary<string, UnityComponentPool<EffectPlayerComponentBase>> _effectPoolByEffectID
            = new Dictionary<string, UnityComponentPool<EffectPlayerComponentBase>>();

        MonoBehaviour _owner;

        public EffectManager(MonoBehaviour owner)
        {
            _owner = owner;
        }

        public EffectManager AddData<T>(params T[] effectData)
            where T : IEffectData
        {
            effectData.Foreach(item => _data.Add(item.GetEffectID(), item));
            return this;
        }

        public EffectPlayCommand PlayEffect(string effectID) => GetEffect(effectID).PlayResource();
        public EffectPlayCommand PlayEffect(IEffectData effectData) => GetEffect(effectData).PlayResource();

        public bool TryGetData(string effectID, out IEffectData data)
            => _data.TryGetValue(effectID, out data);

        public EffectPlayCommand GetEffect(string effectID)
        {
            if (!TryGetData(effectID, out IEffectData data))
            {
                Debug.LogError($"{nameof(EffectManager)} - data is not contain(id:{effectID})");
            }

            return GetEffect(data);
        }

        public EffectPlayCommand GetEffect(IEffectData data)
        {
            string effectID = data.GetEffectID();
            UnityComponentPool<EffectPlayerComponentBase> effectPool = GetOrCreatePool(data, effectID);

            EffectPlayerComponentBase unusedEffect = effectPool.Spawn();
            unusedEffect.SetEffectID(effectID);

            EffectPlayCommand playCommand = _commandPool.Spawn();
            playCommand.Init(unusedEffect, DespawnCommand);

            return playCommand;
        }

        public void PrePoolEffect(IEffectData data, int prePoolCount)
        {
            string effectID = data.GetEffectID();
            UnityComponentPool<EffectPlayerComponentBase> effectPool = GetOrCreatePool(data, effectID);
            effectPool.PrePooling(prePoolCount);
        }

        private void DespawnCommand(ResourcePlayCommandBase<IEffectPlayer> command)
        {
            _commandPool.DeSpawn(command as EffectPlayCommand);
            EffectPlayerComponentBase returnedPlayer = command.ResourcePlayer as EffectPlayerComponentBase;
            string returnedEffectID = returnedPlayer.GetEffectID();

            if (_effectPoolByEffectID.TryGetValue(returnedEffectID, out var effectPoolForReturnedPlayer))
            {
                effectPoolForReturnedPlayer.DeSpawn(returnedPlayer);
            }
            else
            {
                Debug.LogError($"{nameof(EffectManager)} - effectPlayer is not contain effectPool name:{returnedPlayer}, effectID:{returnedEffectID}", returnedPlayer);
            }
        }

        private UnityComponentPool<EffectPlayerComponentBase> GetOrCreatePool(IEffectData data, string effectID)
        {
            if (!_effectPoolByEffectID.TryGetValue(effectID, out var effectPool))
            {
                EffectPlayerComponentBase effectPlayerOrigin = data.GetEffectPlayer();
                if (effectPlayerOrigin == null)
                {
                    Debug.LogError($"{nameof(EffectManager)} - data is EffectPlayerComponent is null (id:{effectID})");
                }
                else
                {
                    effectPool = new UnityComponentPool<EffectPlayerComponentBase>(effectPlayerOrigin);
                    Transform poolParents = new GameObject().transform;
                    poolParents.SetParent(_owner.transform);
                    effectPool.SetParents(poolParents);

                    _effectPoolByEffectID.Add(effectID, effectPool);
                }
            }

            return effectPool;
        }
    }
}