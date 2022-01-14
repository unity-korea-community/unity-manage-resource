// using System.Runtime.InteropServices;
// using System.Reflection;
// using System.Collections;
// using System.Collections.Generic;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;

// public class CanvasManagerTests
// {
//     public class CanvasManagerTester : CanvasManager<CanvasManagerTester, CanvasManagerTester.CanvasName>
//     {
//         public enum CanvasName
//         {
//             CanvasSingle,
//         }
//     }

//     public abstract class TesterCanvasBase : MonoBehaviour, ICanvas
//     {
//         public CanvasManager canvasManager { get; set; }

//         public void Init() { }
//         public abstract IEnumerator OnShowCanvasCoroutine();
//         public abstract IEnumerator OnHideCanvasCoroutine();
//     }

//     public class CanvasTester : TesterCanvasBase
//     {
//         public bool isShow { get; private set; } = false;

//         public override IEnumerator OnShowCanvasCoroutine()
//         {
//             isShow = true;
//             yield break;
//         }

//         public override IEnumerator OnHideCanvasCoroutine()
//         {
//             isShow = false;
//             yield break;
//         }
//     }

//     [Test]
//     public void Show하면_Canvas의_ShowCoroutine이_실행됩니다()
//     {
//         CanvasManagerTester manager = new GameObject().AddComponent<CanvasManagerTester>();
//         CanvasTester canvasInstance = new GameObject().AddComponent<CanvasTester>();
//         manager.AddCanvasInstance(CanvasManagerTester.CanvasName.CanvasSingle, canvasInstance);

//         canvasInstance.isShow.Should().BeFalse();
//         manager.Show(CanvasManagerTester.CanvasName.CanvasSingle);
//         canvasInstance.isShow.Should().BeTrue();
//     }


//     public class CanvasManagerMultipleTester : CanvasManager<CanvasManagerMultipleTester, CanvasManagerMultipleTester.CanvasName>
//     {
//         public enum CanvasName
//         {
//             CanvasMultiple,
//         }

//         int _instanceCount = 0;

//         protected override ICanvas OnRequireCanvasInstance(CanvasName canvasName)
//         {
//             if (canvasName == CanvasName.CanvasMultiple)
//                 return new GameObject($"{canvasName}_{_instanceCount++}").AddComponent<CanvasTester>();

//             return base.OnRequireCanvasInstance(canvasName);
//         }
//     }

//     [UnityTest]
//     public IEnumerator OnRequireCanvasInstance를_재정의하면_Instance가여러개일수_있습니다()
//     {
//         CanvasManagerMultipleTester manager = new GameObject().AddComponent<CanvasManagerMultipleTester>();
//         int showCount = Random.Range(2, 5);

//         HashSet<CanvasTester> canvasInstance = new HashSet<CanvasTester>();
//         for (int i = 0; i < showCount; i++)
//         {
//             CanvasTester item = manager.Show(CanvasManagerMultipleTester.CanvasName.CanvasMultiple) as CanvasTester;
//             canvasInstance.Add(item);
//         }

//         // Assert
//         canvasInstance.Count.Should().Be(showCount);
//         foreach (var item in canvasInstance)
//         {
//             item.isShow.Should().BeTrue();
//             manager.Hide(item);
//             item.isShow.Should().BeFalse();
//         }

//         yield break;
//     }
// }
