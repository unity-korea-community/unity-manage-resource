using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UNKO.ManageResource
{
    public interface ISoundSlot : IResourcePlayer
    {
        AudioClip clip { get; set; }
        float globalVolume { get; set; }
        float localVolume { get; set; }
    }

    public static class SoundSlotHelper
    {
        public static T SetGlobalVolume<T>(this T slot, float volume_0_1)
            where T : ISoundSlot
        {
            slot.globalVolume = volume_0_1;
            return slot;
        }

        public static T SetLocalVolume<T>(this T slot, float volume_0_1)
            where T : ISoundSlot
        {
            slot.localVolume = volume_0_1;
            return slot;
        }
    }
}
