using System;
using System.Collections;
using UnityEngine;
using UNKO.Utils;

namespace UNKO.ManageResource
{
    [Serializable]
    public class SoundPlayCommand : ResourcePlayCommandBase<ISoundSlot>, ISoundSlot
    {
        // NOTE inspector 노출용 - 유니티 에디터는 제네릭 클래스를 인스펙터에 노출을 못하기 때문에
        [Serializable]
        public class Pool : SimplePool<SoundPlayCommand>
        {
            public Pool(SoundPlayCommand originItem) : base(originItem) { }
            public Pool(Func<SoundPlayCommand> onCreateInstance) : base(onCreateInstance) { }
        }

        public AudioClip Clip => ResourcePlayer.Clip;
        public float GlobalVolume { get => ResourcePlayer.GlobalVolume; set => ResourcePlayer.GlobalVolume = value; }
        public float LocalVolume { get => ResourcePlayer.LocalVolume; set => ResourcePlayer.LocalVolume = value; }
        public string SoundCategory => ResourcePlayer.SoundCategory;
        public string Soundkey => ResourcePlayer.Soundkey;

        public void SetMute(bool mute)
            => ResourcePlayer.SetMute(mute);

        public void InitSlot(AudioClip clip, string soundCategory, string soundKey)
            => ResourcePlayer.InitSlot(clip, soundCategory, soundKey);

        public float GetCurrentVolume()
            => ResourcePlayer.GetCurrentVolume();
    }
}