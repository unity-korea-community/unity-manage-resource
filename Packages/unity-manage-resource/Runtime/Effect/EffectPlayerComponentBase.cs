using System.Collections;
using UnityEngine;

namespace UNKO.ManageResource
{
    public interface IEffectPlayer : IResourcePlayer
    {
        GameObject gameObject { get; }
        string GetEffectID();
    }

    public abstract class EffectPlayerComponentBase : MonoBehaviour, IEffectPlayer
    {
        string _effectID;

        public string GetEffectID() => _effectID;

        public void SetEffectID(string effectID)
            => _effectID = effectID;

        public abstract bool IsPlayingResource();
        public abstract IEnumerator PlayCoroutine();
        public abstract void Reset();
        public abstract void SetLoop(bool isLoop);
        public abstract void Stop();

        public void SetWorldPosition(Vector3 position)
            => transform.position = position;

        public void SetLocalPosition(Vector3 position)
            => transform.localPosition = position;

        public void SetParentTransform(Transform parent)
            => transform.parent = parent;
    }
}