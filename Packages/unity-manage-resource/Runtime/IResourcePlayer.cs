using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNKO.ManageResource
{
    /// <summary>
    /// 플레이 가능한 리소스의 인터페이스
    /// 리소스는 코루틴으로 플레이하고, 반복 유무와 위치를 세팅할 수 있고, 멈출 수 있어야 한다.
    /// </summary>
    public interface IResourcePlayAble
    {
        void Reset();

        /// <summary>
        /// 코루틴을 실행 후 실행한 코루틴을 리턴
        /// NOTE IEnumerator로 변경한 후부터 유니티 에디터가 자주 꺼짐, 이게 원인인지 의심
        /// </summary>
        /// <returns></returns>
        IEnumerator PlayCoroutine();

        void Stop();
        void SetLoop(bool isLoop);
        void SetWorldPosition(Vector3 position);
        void SetLocalPosition(Vector3 position);
        void SetParentTransform(Transform parent);
    }

    /// <summary>
    /// 리소스를 실제로 플레이하는 객체
    /// </summary>
    public interface IResourcePlayer : IResourcePlayAble
    {
        bool IsPlayingResource();
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(IEnumerator routine);
    }

    public static class IResourcePlayAbleHelper
    {
        public static T ResetResource<T>(this T command)
            where T : IResourcePlayAble
        {
            command.Reset();
            return command;
        }

        public static T SetLoopResource<T>(this T command, bool isLoop)
            where T : IResourcePlayAble
        {
            command.SetLoop(isLoop);
            return command;
        }

        public static T SetWorldPositionResource<T>(this T command, Vector3 position)
            where T : IResourcePlayAble
        {
            command.SetWorldPosition(position);
            return command;
        }

        public static T SetLocalPositionResource<T>(this T command, Vector3 position)
            where T : IResourcePlayAble
        {
            command.SetLocalPosition(position);
            return command;
        }

        public static T SetParentTransformResource<T>(this T command, Transform parent)
            where T : IResourcePlayAble
        {
            command.SetParentTransform(parent);
            return command;
        }
    }
}