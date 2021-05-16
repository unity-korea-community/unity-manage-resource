using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UNKO.ManageResource;

public class SoundManagerTests
{
    public class CoroutineExecutor : MonoBehaviour { }

    public class SoundData : ISoundData
    {
        string _soundKey;
        float _volume_0_1;

        public SoundData(string soundKey, float volume_0_1)
        {
            this._soundKey = soundKey;
            this._volume_0_1 = volume_0_1;
        }

        public string GetSoundKey() => _soundKey;
        public AudioClip GetAudioClip(ISoundManager manager)
        {
            return null;
        }

        public float GetVolume_0_1(ISoundManager manager) => _volume_0_1;
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

    [OneTimeSetUp]
    public void SetUp()
    {
        new GameObject("Listener", typeof(AudioListener));
    }

    [Test]
    public void PlayStopTest()
    {
        ISoundManager manager = GetManager();

        var command = manager.PlaySound(SoundDataKey.sound1.ToString());
        Assert.AreEqual(command.IsPlaying(), true);

        command.Stop();
        Assert.AreEqual(command.IsPlaying(), false);
    }

    [UnityTest]
    public IEnumerator SetDelayTest()
    {
        ISoundManager manager = GetManager();
        float delay = 0.2f;

        var command = manager.GetSlot(SoundDataKey.sound1.ToString())
            .SetDelay(delay)
            .PlayResource();

        Assert.AreEqual(command.IsPlaying(), false);
        yield return new WaitForSeconds(delay);
        Assert.AreEqual(command.IsPlaying(), true);

        command.Stop();
        Assert.AreEqual(command.IsPlaying(), false);
    }

    [UnityTest]
    public IEnumerator HookEventTest()
    {
        ISoundManager manager = GetManager();
        float delay = 0.2f;
        bool isStarted = false;
        bool isFinished = false;

        var command = manager.GetSlot(SoundDataKey.sound1.ToString())
            .SetDelay(delay)
            .SetOnStart((player) => isStarted = true)
            .SetOnFinish((player) => isFinished = true)
            .PlayResource();

        Assert.AreEqual(isStarted, false);
        Assert.AreEqual(isFinished, false);
        Assert.AreEqual(command.IsPlaying(), false);
        yield return new WaitForSeconds(delay);
        yield return null;

        Assert.AreEqual(command.IsPlaying(), true);
        Assert.AreEqual(isStarted, true);
        Assert.AreEqual(isFinished, true);

        command.Stop();
        Assert.AreEqual(command.IsPlaying(), false);
    }

    [Test]
    public void SetVolume()
    {
        ISoundManager manager = GetManager();
        float localVolume = Random.Range(0.1f, 1f);
        float globalVolume = Random.Range(0.1f, 1f);

        var command = manager.GetSlot(SoundDataKey.sound1.ToString())
            .PlayResource()
            .SetLocalVolume(localVolume);

        Assert.AreEqual(localVolume, command.soundSlot.localVolume);

        manager.SetVolume(globalVolume);
        Assert.AreEqual(globalVolume, command.soundSlot.globalVolume);
    }

    private ISoundManager GetManager()
    {
        CoroutineExecutor coroutineExecutor = new GameObject().AddComponent<CoroutineExecutor>();
        var manager = new SoundManager(coroutineExecutor);
        manager.InitSoundSlot(() => new GameObject().AddComponent<SoundSlotForTest>())
            .AddData(dummyData);

        return manager;
    }
}
