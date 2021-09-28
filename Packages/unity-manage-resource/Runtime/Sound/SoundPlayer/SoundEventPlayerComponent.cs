using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UNKO.ManageResource
{
    public class SoundEventPlayerComponent : MonoBehaviour
    {
        public enum EUnityEvent
        {
            OnAwake,
            OnEnable,
            OnDisable,
        }

        [System.Serializable]
        public class SoundPlayEventInfo : SoundPlayInfo
        {
            public EUnityEvent when;
        }

        [SerializeField]
        private List<SoundPlayEventInfo> _playInfoList = new List<SoundPlayEventInfo>();
        Dictionary<EUnityEvent, List<SoundPlayEventInfo>> _playInfoDictionary;

        void Awake()
        {
            _playInfoDictionary = _playInfoList
                .GroupBy(item => item.when)
                .ToDictionary(item => item.Key, item => item.ToList());

            PlaySound(EUnityEvent.OnAwake);
        }

        void OnEnable()
        {
            PlaySound(EUnityEvent.OnEnable);
        }

        void OnDisable()
        {
            PlaySound(EUnityEvent.OnDisable);
        }

        private void PlaySound(EUnityEvent when)
        {
            if (!_playInfoDictionary.TryGetValue(when, out List<SoundPlayEventInfo> list))
            {
                return;
            }

            ISoundManager manager = SoundSystem.Manager;
            foreach (SoundPlayInfo playInfo in list)
            {
                manager.GetSlot(playInfo.soundKey)
                    .SetDelayResource(playInfo.delay);
            }
        }

    }
}
