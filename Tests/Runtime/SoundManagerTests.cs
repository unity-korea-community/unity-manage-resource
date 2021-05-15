using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UNKO.ManageResource;

public class SoundManagerTests
{
    public class SoundData : ISoundData
    {
        string _soundKey;
        float _volume_0_1;

        public SoundData(string soundKey, float volume_0_1)
        {
            this._soundKey = soundKey;
            this._volume_0_1 = volume_0_1;
        }

        public AudioClip GetAudioClip()
        {
            return null;
        }

        public string GetSoundKey() => _soundKey;
        public float GetVolume_0_1() => _volume_0_1;
    }

    public enum SoundDataKey
    {
        sound1,
        sound2,
    }

    SoundData[] dummyData = new SoundData[]{
        new SoundData(SoundDataKey.sound1.ToString(), 0.1f),
        new SoundData(SoundDataKey.sound2.ToString(), 0.1f)
    };

    [Test]
    public void WorkTest()
    {
        SoundManager manager = new SoundManager()
            .InitSoundSlot(() => new SoundSlotForTest())
            .AddData(dummyData);

        ISoundSlot slot = manager.PlaySound(SoundDataKey.sound1.ToString());
        Assert.AreEqual(slot.isPlaying, true);

        slot.Stop();
        Assert.AreEqual(slot.isPlaying, false);
    }
}
