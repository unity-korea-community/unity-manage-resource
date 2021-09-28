using UnityEngine;
using UnityEngine.UI;

namespace UNKO.ManageResource
{
    public class SoundPlayerButtonClickComponent : MonoBehaviour
    {
        [SerializeField]
        private SoundPlayInfo _playInfo = null;

        private void Awake()
        {
            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(PlaySound);
            }

            Toggle toggle = GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle.onValueChanged.AddListener((toggleValue) => PlaySound());
            }
        }

        public void PlaySound()
        {
            SoundSystem.Manager
                .GetSlot(_playInfo.soundKey)
                .SetDelayResource(_playInfo.delay)
                .PlayResource();
        }
    }
}
