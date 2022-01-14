using System;
using System.Collections;
using UnityEngine;

namespace UNKO.ManageUGUI
{
    /// <summary>
    /// 캔버스의 상태
    /// </summary>
    public enum CanvasStatus
    {
        NotInit,
        Awake,
        Show,
        HideAnimationPlaying,
        Hide,
    }

    public class CanvasWrapper : IDisposable
    {
        public CanvasStatus Status { get; private set; } = CanvasStatus.NotInit;
        public ICanvas CanvasInstance { get; private set; }

        public bool IsShow => Status == CanvasStatus.Show;

        public CanvasWrapper Init(ICanvas canvasInstance)
        {
            this.CanvasInstance = canvasInstance;

            return this;
        }

        public void Awake()
        {
            CanvasInstance.Init();
            Status = CanvasStatus.Awake;
        }

        public void SetActive(bool active)
        {
            CanvasInstance.gameObject.SetActive(active);
            Status = active ? CanvasStatus.Show : CanvasStatus.Hide;
        }

        public void SetStatus(CanvasStatus status)
        {
            this.Status = status;
        }

        public IEnumerator OnShowCanvasCoroutine() => CanvasInstance.OnShowCanvasCoroutine();
        public IEnumerator OnHideCanvasCoroutine() => CanvasInstance.OnHideCanvasCoroutine();

        public void Dispose()
        {
            GameObject.DestroyImmediate(CanvasInstance.gameObject);
        }
    }
}
