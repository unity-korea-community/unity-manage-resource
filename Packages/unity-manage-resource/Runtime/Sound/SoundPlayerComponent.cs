using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UNKO.ManageResource
{
    public class SoundPlayerComponent : MonoBehaviour
    {
        public enum EUnityEvent
        {
            OnAwake,
            OnEnable,
            OnDisable,
        }

        [System.Serializable]
        public class PlayInfo
        {
            public string soundKey;
            public EUnityEvent when;
            public float delay;
            public bool isLoop;
            public bool is3DSound;
        }

        static ISoundManager s_soundManager;

        [SerializeField]
        private List<PlayInfo> _playInfoList = new List<PlayInfo>();
        Dictionary<EUnityEvent, List<PlayInfo>> _playInfoDictionary;

        public static void Init(ISoundManager soundManager)
        {
            s_soundManager = soundManager;
        }

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
            if (_playInfoDictionary.TryGetValue(when, out List<PlayInfo> list) == false)
                return;

            foreach (PlayInfo playInfo in list)
            {
                s_soundManager.GetSlot(playInfo.soundKey)
                    .SetDelay(playInfo.delay);
            }
        }

    }
}
