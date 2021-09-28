using System;
using System.Collections;
using UnityEngine;

namespace UNKO.ManageResource
{
    public interface IResourcePlayCommand : IResourcePlayer, IDisposable
    {
        event Action<IResourcePlayer> OnPlayStart;
        event Action<IResourcePlayer> OnPlayFinish;

        float Delay { get; set; }
        IResourcePlayer GetPlayer();
    }

    public static class ResourcePlayCommandHelper
    {
        public static T SetDelayResource<T>(this T command, float delay)
            where T : IResourcePlayCommand
        {
            command.Delay = delay;
            return command;
        }
        public static T PlayResource<T>(this T command)
            where T : IResourcePlayCommand
        {
            command.PlayCoroutine();
            return command;
        }

        public static T Stop<T>(this T command)
            where T : IResourcePlayCommand
        {
            IResourcePlayer resourcePlayer = command.GetPlayer();
            resourcePlayer.Stop();
            return command;
        }

        public static T SetOnStart<T>(this T command, Action<IResourcePlayer> onStart)
            where T : IResourcePlayCommand
        {
            command.OnPlayStart -= onStart;
            command.OnPlayStart += onStart;
            return command;
        }

        public static T SetOnFinish<T>(this T command, Action<IResourcePlayer> onFinish)
            where T : IResourcePlayCommand
        {
            command.OnPlayFinish -= onFinish;
            command.OnPlayFinish += onFinish;
            return command;
        }

        public static IEnumerator PlayCoroutine<T>(this T command, IResourcePlayer player, Action<IResourcePlayer> onStart, Action<IResourcePlayer> onFinish)
            where T : IResourcePlayCommand
        {
            float waitSeconds = command.Delay;
            while (waitSeconds > 0f)
            {
                yield return null;
                waitSeconds -= Time.deltaTime;
                // UnityEngine.Debug.Log($"PlayCoroutine - waiting.. waitSecond:{waitSeconds}, deltaTime: {Time.deltaTime}");
            }
            // UnityEngine.Debug.Log($"PlayCoroutine - waiting done waitSecond:{waitSeconds}, deltaTime: {Time.deltaTime}");

            onStart(player);
            yield return player.PlayCoroutine();
            onFinish(player);
            command.Dispose();
        }

        public static T AddTo<T>(this T command, ResourcePlayCommands commands)
            where T : IResourcePlayCommand
        {
            commands.AddCommand(command);
            return command;
        }
    }
}
