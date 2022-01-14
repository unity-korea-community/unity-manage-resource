using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UNKO.Utils;

namespace UNKO.ManageUGUI
{
    public interface ICanvasManager
    {
#pragma warning disable IDE1006
        string name { get; }
        Transform transform { get; }
        GameObject gameObject { get; }
#pragma warning restore IDE1006

        bool IsShow(string canvasName);
        ICanvas Show(string canvasName);
        ICanvas Show(ICanvas canvas);
        void Hide(string canvasName);
        void Hide(ICanvas canvas);
    }

    public class CanvasManager<TDerived, TCanvasName> : SingletonComponentBase<TDerived>, ICanvasManager
        where TDerived : CanvasManager<TDerived, TCanvasName>
        where TCanvasName : struct, System.Enum
    {
        public struct Command
        {
            public TCanvasName CanvasName { get; private set; }
            public CanvasWrapper CanvasWrapper { get; private set; }
            public bool IsShow { get; private set; }

            public Command(TCanvasName canvasName, CanvasWrapper canvasWrapper, bool isShow)
            {
                CanvasName = canvasName;
                CanvasWrapper = canvasWrapper;
                IsShow = isShow;
            }
        }

        private Dictionary<TCanvasName, List<CanvasWrapper>> _canvasInstance = new Dictionary<TCanvasName, List<CanvasWrapper>>();
        private Dictionary<ICanvas, (TCanvasName name, CanvasWrapper wrapper)> _instanceMatch = new Dictionary<ICanvas, (TCanvasName, CanvasWrapper)>();
        List<Command> _commandQueue = new List<Command>();

        private SimplePool<CanvasWrapper> _canvasWrapperPool = new SimplePool<CanvasWrapper>(new CanvasWrapper(), 10);
        public bool IsReady { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            if (IsReady)
            {
                return;
            }

            ICanvas[] canvasArray = GetComponentsInChildren<ICanvas>(true);
            foreach (var canvas in canvasArray)
            {
                if (System.Enum.TryParse(canvas.name, out TCanvasName canvasName))
                    AddCanvasInstance(canvasName, canvas);
            }
            Init(false);

            IsReady = true;
        }

        protected void Update()
        {
            while (_commandQueue.Count > 0)
            {
                Command command = _commandQueue.Dequeue();
                TCanvasName canvasName = command.CanvasName;
                CanvasWrapper canvasWrapper = command.CanvasWrapper;
                if (canvasWrapper == null)
                {
                    Debug.LogError($"canvas wrapper is null: {canvasName}, isShow: {command.IsShow}");
                    continue;
                }
                if (command.IsShow)
                {
                    canvasWrapper.SetActive(true);
                    StartCoroutine(canvasWrapper.OnShowCanvasCoroutine());
                    OnShowCanvas(canvasName, canvasWrapper.CanvasInstance);
                }
                else
                {
                    canvasWrapper.SetStatus(CanvasStatus.HideAnimationPlaying);
                    OnHideCanvas(canvasName, canvasWrapper.CanvasInstance);
                    StartCoroutine(canvasWrapper.OnHideCanvasCoroutine, () =>
                    {
                        canvasWrapper.SetActive(false);
                    });
                }
            }
        }

        public void Init(bool active)
        {
            foreach (var canvasList in _canvasInstance.Values)
            {
                foreach (CanvasWrapper canvasValue in canvasList)
                    canvasValue.SetActive(active);
            }
        }

        public CanvasWrapper AddCanvasInstance(TCanvasName canvasName, ICanvas canvasInstance)
        {
            if (_instanceMatch.TryGetValue(canvasInstance, out var wrapper))
            {
                return wrapper.wrapper;
            }

            if (_canvasInstance.TryGetValue(canvasName, out var list) == false)
            {
                list = new List<CanvasWrapper>();
                _canvasInstance.Add(canvasName, list);
            }

            wrapper = (canvasName, _canvasWrapperPool.Spawn().Init(canvasInstance));
            _instanceMatch.Add(canvasInstance, wrapper);
            list.Add(wrapper.wrapper);

            canvasInstance.canvasManager = this;
            wrapper.wrapper.Awake();

            return wrapper.wrapper;
        }

        public ICanvas Show(string canvasName)
        {
            if (System.Enum.TryParse(canvasName, out TCanvasName canvasNameEnum) == false)
            {
                Debug.LogError($"{name} not found canvasInstance:{canvasName}", this);
                return null;
            }

            return Show(canvasNameEnum);
        }

        public ICanvas Show(TCanvasName canvasName)
        {
            CanvasWrapper canvasWrapper = GetCanvasWrapper(canvasName, canvas => canvas.IsShow == false);
            if (canvasWrapper == null)
                canvasWrapper = OnCreateInstance(canvasName);

            return ProcessShow(canvasName, canvasWrapper);
        }

        public void Show(params TCanvasName[] canvasNames)
        {
            canvasNames.ForEach((canvasName) => this.Show(canvasName));
        }

        public ICanvas Show(ICanvas canvas)
        {
            if (canvas.gameObject == null)
            {
                Debug.LogError($"CanvasManager({name}) canvas.gameObject == nul", this);
                return canvas;
            }

            if (_instanceMatch.TryGetValue(canvas, out var canvasWrapper) == false)
            {
                Debug.LogError($"CanvasManager({name}) not found canvasInstance:{canvas.gameObject.name}", this);
                return canvas;
            }

            string canvasName = canvas.gameObject.name;
            if (System.Enum.TryParse(canvasName, out TCanvasName canvasNameEnum) == false)
            {
                Debug.LogError($"{name} not found canvasInstance:{canvasName}", this);
                return canvas;
            }

            return ProcessShow(canvasNameEnum, canvasWrapper.wrapper);
        }

        public void Hide(string canvasName)
        {
            if (System.Enum.TryParse(canvasName, out TCanvasName canvasNameEnum) == false)
            {
                Debug.LogError($"{name} not found canvasInstance:{canvasName}", this);
                return;
            }

            Hide(canvasNameEnum);
        }

        public void Hide(params TCanvasName[] canvasNames)
        {
            canvasNames.ForEach(this.Hide);
        }

        public void Hide(TCanvasName canvasName)
        {
            CanvasWrapper canvasWrapper = GetCanvasWrapper(canvasName);
            ProcessHide(canvasName, canvasWrapper);
        }

        public void Hide(ICanvas canvas)
        {
            if (_instanceMatch.TryGetValue(canvas, out var canvasWrapper) == false)
            {
                Debug.LogError($"CanvasManager({name}) not found canvasInstance:{canvas.gameObject.name}", this);
                return;
            }

            ProcessHide(canvasWrapper.name, canvasWrapper.wrapper);
        }

        public void HideAll()
        {
            foreach (var canvasName in _canvasInstance.Keys)
                Hide(canvasName);
        }

        public void HideAll(params TCanvasName[] except)
        {
            foreach (var canvasName in _canvasInstance.Keys.Where(canvas => except.Contains(canvas) == false))
                Hide(canvasName);
        }

        public bool IsShow(string canvasName)
        {
            if (System.Enum.TryParse(canvasName, out TCanvasName canvasNameEnum) == false)
            {
                Debug.LogError($"{name} not found canvasInstance:{canvasName}", this);
                return false;
            }

            CanvasWrapper canvasWrapper = GetCanvasWrapper(canvasNameEnum);
            return canvasWrapper.IsShow;
        }


        public bool IsShow(TCanvasName canvasName)
        {
            CanvasWrapper canvasWrapper = GetCanvasWrapper(canvasName);
            if (canvasWrapper == null)
            {
                return false;
            }

            return canvasWrapper.IsShow;
        }

        public ICanvas GetCanvas(TCanvasName canvasName) => GetCanvasWrapper(canvasName).CanvasInstance;

        public void DisposeCanvas(TCanvasName canvasName)
        {
            if (_canvasInstance.TryGetValue(canvasName, out var canvasWrapperList))
            {
                _canvasInstance.Remove(canvasName);

                CollectionExtension.ForEach(canvasWrapperList, canvas => canvas.Dispose());
                canvasWrapperList.Clear();
            }
        }

        protected virtual ICanvas OnRequireCanvasInstance(TCanvasName canvasName)
        {
            return null;
        }

        protected virtual void OnShowCanvas(TCanvasName canvasName, ICanvas canvas)
        {
        }

        protected virtual void OnHideCanvas(TCanvasName canvasName, ICanvas canvas)
        {
        }

        protected CanvasWrapper GetCanvasWrapper(TCanvasName canvasName, System.Func<CanvasWrapper, bool> OnMatch = null)
        {
            if (_canvasInstance.TryGetValue(canvasName, out var canvasWrapperList) == false)
            {
                return null;
            }

            if (OnMatch == null)
            {
                return canvasWrapperList.FirstOrDefault();
            }

            return canvasWrapperList.FirstOrDefault(canvas => OnMatch(canvas));
        }

        private CanvasWrapper OnCreateInstance(TCanvasName canvasName)
        {
            ICanvas canvasInstance = OnRequireCanvasInstance(canvasName);
            if (canvasInstance == null)
            {
                Debug.LogError($"CanvasManager({name}) GetCanvasInstance(canvasName:{canvasName}) canvasInstance == null", this);
                return null;
            }

            return AddCanvasInstance(canvasName, canvasInstance);
        }

        private ICanvas ProcessShow(TCanvasName canvasName, CanvasWrapper canvasWrapper)
        {
            if (canvasWrapper != null)
            {
                _commandQueue.RemoveAll(command => command.CanvasName.Equals(canvasName) && command.IsShow == false);
                _commandQueue.Add(new Command(canvasName, canvasWrapper, true));
            }

            return canvasWrapper.CanvasInstance;
        }

        private void ProcessHide(TCanvasName canvasName, CanvasWrapper canvasWrapper)
        {
            _commandQueue.RemoveAll(command => command.CanvasName.Equals(canvasName) && command.IsShow);
            _commandQueue.Add(new Command(canvasName, canvasWrapper, false));
        }

        void StartCoroutine(System.Func<IEnumerator> coroutine, System.Action OnFinish)
        {
            StartCoroutine(InvokeAfterCoroutine(coroutine, OnFinish));
        }

        IEnumerator InvokeAfterCoroutine(System.Func<IEnumerator> coroutine, System.Action OnFinish)
        {
            yield return StartCoroutine(coroutine());

            OnFinish?.Invoke();
        }
    }
}
