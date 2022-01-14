// using System.Reflection;
// using System.Collections;
// using System.Collections.Generic;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;
// using UnityEngine.UI;

// public class TabButtonLogicTests
// {
//     enum ButtonName
//     {
//         A, B, C
//     }

//     [UnityTest]
//     public IEnumerator 사용예시()
//     {
//         // arrange
//         Button buttonA = new GameObject().AddComponent<Button>();
//         Button buttonB = new GameObject().AddComponent<Button>();
//         Button buttonC = new GameObject().AddComponent<Button>();
//         Dictionary<ButtonName, Button> buttonObject = new Dictionary<ButtonName, Button>()
//         {
//             { ButtonName.A, buttonA }, { ButtonName.B, buttonB }, { ButtonName.C, buttonC }
//         };

//         GameObject objectA = new GameObject();
//         GameObject objectB = new GameObject();
//         GameObject objectC = new GameObject();
//         Dictionary<ButtonName, GameObject> tabObject = new Dictionary<ButtonName, GameObject>()
//         {
//             { ButtonName.A, objectA }, { ButtonName.B, objectB }, { ButtonName.C, objectC }
//         };


//         TabButtonLogic<ButtonName> logic = new TabButtonLogic<ButtonName>();
//         logic.AddObjectRange(buttonObject)
//             .AddShowLogic(key => tabObject[key].SetActive(true))
//             .AddHideLogic(key => tabObject[key].SetActive(false));

//         // act && assert - case A
//         logic.ShowObject(ButtonName.A);
//         yield return null;
//         objectA.activeSelf.Should().BeTrue();
//         objectB.activeSelf.Should().BeFalse();
//         objectC.activeSelf.Should().BeFalse();

//         // act && assert - case B
//         logic.ShowObject(ButtonName.B);
//         yield return null;
//         objectA.activeSelf.Should().BeFalse();
//         objectB.activeSelf.Should().BeTrue();
//         objectC.activeSelf.Should().BeFalse();
//     }
// }
