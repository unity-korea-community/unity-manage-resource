using System.Collections;
using UnityEngine;

namespace UNKO.ManageUGUI
{
    public class CanvasComponentBase : MonoBehaviour, ICanvas
    {
        public ICanvasManager canvasManager { get; set; }

        virtual public void Init() { }

        virtual public IEnumerator OnShowCanvasCoroutine()
        {
            yield break;
        }

        virtual public IEnumerator OnHideCanvasCoroutine()
        {
            yield break;
        }
    }
}
