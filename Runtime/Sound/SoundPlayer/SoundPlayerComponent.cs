using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UNKO.ManageResource
{
    public class SoundPlayerComponent : MonoBehaviour
    {
        [SerializeField]
        private SoundPlayInfo _playInfo = null;

        public void PlaySound()
        {
            SoundSystem.Manager
                .GetSlot(_playInfo.soundKey)
                .SetDelayResource(_playInfo.delay)
                .PlayResource();
        }
    }
}
