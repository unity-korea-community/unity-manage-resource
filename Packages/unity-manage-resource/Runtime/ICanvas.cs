using System.Collections;
using UnityEngine;
using UNKO.ManageUGUI;

namespace UNKO.ManageUGUI
{
    public interface ICanvas
    {
#pragma warning disable IDE1006
        string name { get; }
        Transform transform { get; }
        GameObject gameObject { get; }
        ICanvasManager canvasManager { get; set; }
#pragma warning restore IDE1006

        void Init();
        IEnumerator OnShowCanvasCoroutine();
        IEnumerator OnHideCanvasCoroutine();
    }
}

public static class ICanvasHelper
{
    public static T Cast<T>(this ICanvas canvas)
        where T : class, ICanvas
    {
        return canvas as T;
    }

    public static void Show(this ICanvas canvas)
    {
        canvas.canvasManager.Show(canvas);
    }

    public static void Hide(this ICanvas canvas)
    {
        canvas.canvasManager.Hide(canvas);
    }
}