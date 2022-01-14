using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UNKO.ManageUGUI
{
    public interface IHasButton<TButtonName>
        where TButtonName : struct, Enum
    {
#pragma warning disable IDE1006
        GameObject gameObject { get; }
        Dictionary<TButtonName, Button> buttons { get; }
#pragma warning restore IDE1006
    }

    public static class IHasButtonExtension
    {
        public static void Init_IHasButton<TButtonName>(this IHasButton<TButtonName> target)
            where TButtonName : struct, Enum
        {
            Dictionary<TButtonName, Button> targetButtons = target.buttons;
            if (targetButtons == null)
            {
                Debug.LogError($"{target.gameObject.name} - targetButtons == null");
                return;
            }

            Button[] buttonArray = target.gameObject.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttonArray)
            {
                if (Enum.TryParse(button.name, out TButtonName buttonName) == false)
                    continue;

                if (targetButtons.ContainsKey(buttonName))
                {
                    Debug.LogWarning($"[IHasButtonExtension.Init_IHasButton] {target.gameObject.name}.buttons.ContainsKey({buttonName}) == true");
                    continue;
                }

                targetButtons.Add(buttonName, button);
            }

            // validate
            TButtonName[] enumArray = Enum.GetNames(typeof(TButtonName))
                .Select(item => (TButtonName)Enum.Parse(typeof(TButtonName), item))
                .ToArray();

            foreach (var item in enumArray)
            {
                if (targetButtons.ContainsKey(item) == false)
                    Debug.LogError($"{target.gameObject.name} Init_IHasButton - targetButtons.ContainsKey({item}) == false", target.gameObject);
            }
        }

        public static Button GetButton<TButtonName>(this IHasButton<TButtonName> target, TButtonName buttonName)
            where TButtonName : struct, Enum
        {
            target.buttons.TryGetValue(buttonName, out Button button);

            return button;
        }
    }

}
