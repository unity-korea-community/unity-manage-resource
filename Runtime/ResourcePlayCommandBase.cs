using System;
using System.Collections;
using UnityEngine;

namespace UNKO.ManageResource
{
    public abstract class ResourcePlayCommandBase<T> : IResourcePlayCommand
        where T : IResourcePlayer
    {
        public event Action<IResourcePlayer> OnPlayStart = delegate { }; // avoid null check
        public event Action<IResourcePlayer> OnPlayFinish = delegate { };

        public T ResourcePlayer { get; private set; }
        public float Delay { get; set; }

        Action<ResourcePlayCommandBase<T>> _onDispose;

        public IResourcePlayer GetPlayer() => ResourcePlayer;
        public bool IsPlayingResource()
            => ResourcePlayer.IsPlayingResource();

        public void Init(T resourcePlayerInstance, Action<ResourcePlayCommandBase<T>> onDispose)
        {
            this.ResourcePlayer = resourcePlayerInstance;
            _onDispose = onDispose;

            Reset();
        }

        public virtual void Reset()
        {
            Delay = 0f;
            OnPlayStart = delegate { };
            OnPlayFinish = delegate { };

            ResourcePlayer.Reset();
        }

        public void SetLoop(bool isLoop)
            => ResourcePlayer.SetLoop(isLoop);

        public virtual IEnumerator PlayCoroutine()
        {
            IEnumerator playCoroutine = this.PlayCoroutine(ResourcePlayer, OnPlayStart.Invoke, OnPlayFinish.Invoke);
            ResourcePlayer.StartCoroutine(playCoroutine);
            return playCoroutine;
        }

        public void Stop()
            => ResourcePlayer.Stop();

        public virtual void Dispose()
        {
            _onDispose.Invoke(this);
            GC.SuppressFinalize(this);
        }

        public Coroutine StartCoroutine(IEnumerator routine)
            => ResourcePlayer.StartCoroutine(routine);

        public void StopCoroutine(IEnumerator routine)
            => ResourcePlayer.StopCoroutine(routine);

        public void SetWorldPosition(Vector3 position)
            => ResourcePlayer.SetWorldPosition(position);

        public void SetLocalPosition(Vector3 position)
            => ResourcePlayer.SetLocalPosition(position);

        public void SetParentTransform(Transform parent)
            => ResourcePlayer.SetParentTransform(parent);
    }
}