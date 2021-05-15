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

    public interface IResourcePlayCommand : IResourcePlayer, IDisposable
    {
        event System.Action<IResourcePlayer> OnPlayStart;
        event System.Action<IResourcePlayer> OnPlayFinish;

        float delay { get; set; }
    }

    public static class ResourcePlayCommandHelper
    {
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

        public static IEnumerator PlayCoroutine<T>(this T command, Func<Coroutine> onPlayCoroutine, System.Action<IResourcePlayCommand> onStart, System.Action<IResourcePlayCommand> onFinish)
            where T : IResourcePlayCommand
        {
            if (command.delay > 0f)
                yield return new WaitForSeconds(command.delay);

            onStart(command);
            yield return onPlayCoroutine();
            onFinish(command);
            command.Dispose();
        }
    }
}