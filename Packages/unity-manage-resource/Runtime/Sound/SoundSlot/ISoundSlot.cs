using System.Collections;
using UnityEngine;

namespace UNKO.ManageResource
{
    public interface ISoundSlot : IResourcePlayer
    {
        AudioClip Clip { get; }
        string SoundCategory { get; }
        string Soundkey { get; }
        float GlobalVolume { get; set; }
        float LocalVolume { get; set; }

        void InitSlot(AudioClip clip, string soundCategory, string soundKey);
        void SetMute(bool mute);
        float GetCurrentVolume();
    }

    public static class SoundSlotHelper
    {
        public static T SetGlobalVolume<T>(this T slot, float volume_0_1)
            where T : ISoundSlot
        {
            slot.GlobalVolume = volume_0_1;
            return slot;
        }

        public static T SetLocalVolume<T>(this T slot, float volume_0_1)
            where T : ISoundSlot
        {
            slot.LocalVolume = volume_0_1;
            return slot;
        }
    }

    public abstract class SoundSlotComponentBase : MonoBehaviour, ISoundSlot
    {
        [SerializeField]
        protected float _globalVolume = 0.5f;
        [SerializeField]
        protected float _localVolume = 0.5f;
        [SerializeField]
        protected string _soundCategory = string.Empty;
        [SerializeField]
        protected string _soundKey = string.Empty;

        public AudioClip Clip { get; protected set; }
        public virtual float GlobalVolume { get => _globalVolume; set => _globalVolume = value; }
        public virtual float LocalVolume
        {
            get => _localVolume;
            set => _localVolume = value;
        }

        public virtual string Soundkey => _soundKey;
        public virtual string SoundCategory => _soundCategory;

        public virtual void InitSlot(AudioClip clip, string soundCategory, string soundKey)
        {
            this.Clip = clip;
            this._soundCategory = soundCategory;
            this._soundKey = soundKey;
        }

        public abstract bool IsPlayingResource();
        public abstract IEnumerator PlayCoroutine();
        public abstract void Reset();
        public abstract float GetCurrentVolume();
        public abstract void SetLoop(bool isLoop);
        public abstract void SetMute(bool mute);
        public abstract void Stop();

        public void SetWorldPosition(Vector3 position)
            => transform.position = position;

        public void SetLocalPosition(Vector3 position)
            => transform.localPosition = position;

        public void SetParentTransform(Transform parent)
            => transform.parent = parent;
    }
}
