using UnityEngine;
using UNKO.Utils;

namespace UNKO.ManageResource
{
    public class EffectManagerComponet : SingletonComponentBase<EffectManagerComponet>, IEffectManager
    {
        public EffectPlayCommand.Pool CommandPool => _manager.CommandPool;

        [SerializeField]
        EffectManager _manager = null;

        public override void InitSingleton()
        {
            base.InitSingleton();

            _manager = new EffectManager(this);
            DontDestroyOnLoad(this);
        }

        public EffectManager AddData<T>(params T[] effectData) where T : IEffectData
            => _manager.AddData(effectData);

        public EffectPlayCommand GetEffect(string effectID)
            => _manager.GetEffect(effectID);

        public EffectPlayCommand GetEffect(IEffectData data)
            => _manager.GetEffect(data);

        public EffectPlayCommand PlayEffect(string effectID)
            => _manager.PlayEffect(effectID);

        public EffectPlayCommand PlayEffect(IEffectData effectData)
            => _manager.PlayEffect(effectData);

        public bool TryGetData(string effectID, out IEffectData data)
            => _manager.TryGetData(effectID, out data);

        public void PrePoolEffect(IEffectData data, int prePoolCount)
            => _manager.PrePoolEffect(data, prePoolCount);

        private void OnDestroy()
        {
        }
    }
}