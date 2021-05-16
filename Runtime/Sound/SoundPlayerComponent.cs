using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UNKO.ManageResource
{
    public class SoundPlayerComponent : MonoBehaviour
    {
        [SerializeField]
        private SoundPlayInfo _playInfo;

        public void PlaySound()
        {
            SoundSystem.manager
                .GetSlot(_playInfo.soundKey)
                .SetDelay(_playInfo.delay)
                .PlayResource();
        }
    }
}
