using UnityEngine;

namespace UNKO.ManageResource
{
    public interface IEffectData
    {
        string GetEffectID();
        EffectPlayerComponentBase GetEffectPlayer();
    }

    public interface IEffectManager
    {
        GameObject gameObject { get; }
        EffectPlayCommand.Pool CommandPool { get; }

        EffectManager AddData<T>(params T[] effectData) where T : IEffectData;
        EffectPlayCommand GetEffect(string effectID);
        EffectPlayCommand GetEffect(IEffectData data);
        EffectPlayCommand PlayEffect(string effectID);
        EffectPlayCommand PlayEffect(IEffectData effectData);
        bool TryGetData(string effectID, out IEffectData data);

        void PrePoolEffect(IEffectData data, int prePoolCount);
    }
}
