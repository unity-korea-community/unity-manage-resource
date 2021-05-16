using System;
using System.Collections;
using UnityEngine;

namespace UNKO.ManageResource
{
    public interface IResourcePlayer
    {
        bool IsPlaying();

        void Reset();
        Coroutine Play();
        void Stop();
        void SetLoop(bool isLoop);
    }

    public interface IResourcePlayCommand : IDisposable
    {
        event System.Action<IResourcePlayer> OnPlayStart;
        event System.Action<IResourcePlayer> OnPlayFinish;

        float delay { get; set; }

        void Reset();
        Coroutine Play();
        void Stop();
        void SetLoop(bool isLoop);
    }

    public static class ResourcePlayCommandHelper
    {
        public static T ResetResource<T>(this T command)
            where T : IResourcePlayCommand
        {
            command.Reset();
            return command;
        }

        public static T PlayResource<T>(this T command)
            where T : IResourcePlayCommand
        {
            command.Play();
            return command;
        }

        public static T SetLoopResource<T>(this T command, bool isLoop)
            where T : IResourcePlayCommand
        {
            command.SetLoop(isLoop);
            return command;
        }

        public static T SetDelay<T>(this T command, float delay)
            where T : IResourcePlayCommand
        {
            command.delay = delay;
            return command;
        }

        public static T SetOnStart<T>(this T command, System.Action<IResourcePlayer> onStart)
            where T : IResourcePlayCommand
        {
            command.OnPlayStart -= onStart;
            command.OnPlayStart += onStart;
            return command;
        }

        public static T SetOnFinish<T>(this T command, System.Action<IResourcePlayer> onFinish)
            where T : IResourcePlayCommand
        {
            command.OnPlayFinish -= onFinish;
            command.OnPlayFinish += onFinish;
            return command;
        }

        public static IEnumerator PlayCoroutine<T>(this T command, IResourcePlayer player, System.Action<IResourcePlayer> onStart, System.Action<IResourcePlayer> onFinish)
            where T : IResourcePlayCommand
        {
            if (command.delay > 0f)
                yield return new WaitForSeconds(command.delay);

            onStart(player);
            yield return player.Play();
            onFinish(player);
            command.Dispose();
        }
    }
}
