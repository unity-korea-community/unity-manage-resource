using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UNKO.ManageResource;

[Category("UNKO")]
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

        [System.Obsolete("AudioClip.Create is Obsolete")]
        public AudioClip GetAudioClip(ISoundManager manager)
        {
            return AudioClip.Create("dummyTest", 1, 1, 1001, false, false);
        }

        public float GetVolume_0_1(ISoundManager manager) => _volume_0_1;

        string ISoundData.GetSoundCategory() => "";
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
    public void PoolingTest()
    {
        SoundManager manager = GetManager() as SoundManager;
        int randomCount = Random.Range(3, 5);
        for (int i = 0; i < randomCount; i++)
        {
            var command = manager.PlaySound(SoundDataKey.sound1.ToString());
            command.Dispose();
        }

        Assert.AreEqual(0, manager.SlotPool.Use.Count);
        Assert.AreEqual(0, manager.CommandPool.Use.Count);

        Assert.AreEqual(1, manager.SlotPool.AllInstance.Count);
        Assert.AreEqual(1, manager.CommandPool.AllInstance.Count);
    }

    [UnityTest]
    public IEnumerator PoolingTestCoroutine()
    {
        SoundManager manager = GetManager() as SoundManager;

        int randomCount = Random.Range(3, 5);
        List<IEnumerator> coroutineList = new List<IEnumerator>();
        List<IResourcePlayCommand> commandList = new List<IResourcePlayCommand>();
        for (int i = 0; i < randomCount; i++)
        {
            var command = manager.PlaySound(SoundDataKey.sound1.ToString());
            commandList.Add(command);
            coroutineList.Add(command.PlayCoroutine());
        }

        foreach (var command in commandList)
        {
            Assert.IsTrue(command.IsPlayingResource());
        }

        Assert.AreEqual(randomCount, manager.SlotPool.Use.Count);
        Assert.AreEqual(randomCount, manager.CommandPool.Use.Count);

        Assert.AreEqual(randomCount, manager.SlotPool.AllInstance.Count);
        Assert.AreEqual(randomCount, manager.CommandPool.AllInstance.Count);

        yield return coroutineList;
        yield return new WaitForSeconds(0.03f);

        foreach (var command in commandList)
        {
            Assert.IsFalse(command.IsPlayingResource());
        }

        Assert.AreEqual(0, manager.CommandPool.Use.Count);
        Assert.AreEqual(randomCount, manager.CommandPool.AllInstance.Count);
    }

    [Test]
    public void PlayStopTest()
    {
        ISoundManager manager = GetManager();

        var command = manager.PlaySound(SoundDataKey.sound1.ToString());
        Assert.IsTrue(command.IsPlayingResource());

        command.Stop();
        Assert.IsFalse(command.IsPlayingResource());
    }

    [UnityTest]
    public IEnumerator SetDelayTest()
    {
        // Arrange
        ISoundManager manager = GetManager();
        float delay = 0.2f;

        // Act
        var command = manager.GetSlot(SoundDataKey.sound1.ToString())
            .SetDelayResource(delay)
            .PlayResource();

        Assert.IsFalse(command.IsPlayingResource());
        float waitSeconds = 0f;
        while (waitSeconds < delay)
        {
            waitSeconds += Time.deltaTime;
            yield return null;
        }
        yield return null;

        // Act & Assert Stop
        Assert.IsTrue(command.IsPlayingResource());

        command.Stop();
        Assert.IsFalse(command.IsPlayingResource());
    }

    [UnityTest]
    public IEnumerator HookEventTest()
    {
        // Arrange
        ISoundManager manager = GetManager();
        float delay = 0.2f;
        bool isStarted = false;
        bool isFinished = false;

        // Act
        var command = manager.GetSlot(SoundDataKey.sound1.ToString())
            .SetDelayResource(delay)
            .SetOnStart((resourcePlayer) => isStarted = true)
            .SetOnFinish((resourcePlayer) => isFinished = true)
            .PlayResource();

        Assert.IsFalse(isStarted);
        Assert.IsFalse(isFinished);
        Assert.IsFalse(command.IsPlayingResource());

        float waitSecond = 0f;
        while (waitSecond < delay)
        {
            waitSecond += Time.deltaTime;
            yield return null;
        }
        yield return null;

        // Assert Wait done and Play
        Assert.IsTrue(command.IsPlayingResource());
        Assert.IsTrue(isStarted);

        yield return new WaitForSeconds(0.03f);
        Assert.IsTrue(isFinished);

        // Act & Assert Stop
        command.Stop();
        Assert.IsFalse(command.IsPlayingResource());
    }

    [Test]
    public void SetVolume()
    {
        // Arrange
        ISoundManager manager = GetManager();
        float localVolume = Random.Range(0.1f, 1f);
        float globalVolume = Random.Range(0.1f, 1f);

        // Act
        var command = manager.GetSlot(SoundDataKey.sound1.ToString())
            .PlayResource()
            .SetLocalVolume(localVolume);

        Assert.AreEqual(localVolume, command.ResourcePlayer.LocalVolume);

        // Assert
        manager.SetGlobalVolume(globalVolume);
        Assert.AreEqual(globalVolume, command.ResourcePlayer.GlobalVolume);
    }

    [Test]
    public void SetMute()
    {
        // Arrange
        ISoundManager manager = GetManager();
        float localVolume = Random.Range(0.1f, 1f);
        string soundKey = SoundDataKey.sound1.ToString();
        float calculateVolume = localVolume * manager.GetGlobalVolume();
        var command = manager.GetSlot(soundKey)
            .PlayResource()
            .SetLocalVolume(localVolume);

        // Act & Assert MuteAll true
        manager.SetMuteAll(true);
        Assert.AreEqual(command.GetCurrentVolume(), 0f);

        // Act & Assert MuteAll false
        manager.SetMuteAll(false);
        Assert.AreEqual(command.GetCurrentVolume(), calculateVolume);

        // Act & Assert MuteBy soundKey true
        manager.SetMuteBySoundKey(soundKey, true);
        Assert.AreEqual(command.GetCurrentVolume(), 0f);

        // Act & Assert MuteBy soundKey false
        manager.SetMuteBySoundKey(soundKey, false);
        Assert.AreEqual(command.GetCurrentVolume(), calculateVolume);

        // Act & Assert MuteBy soundCategory true
        manager.SetMuteBySoundCategory(string.Empty, true);
        Assert.AreEqual(command.GetCurrentVolume(), 0f);

        // Act & Assert MuteBy soundCategory false
        manager.SetMuteBySoundCategory(string.Empty, false);
        Assert.AreEqual(command.GetCurrentVolume(), calculateVolume);
    }

    private ISoundManager GetManager()
    {
        CoroutineExecutor coroutineExecutor = new GameObject().AddComponent<CoroutineExecutor>();
        var manager = new SoundManager(coroutineExecutor, () => new GameObject().AddComponent<SoundSlotForTest>().SetDuration(0.03f))
            .AddData(dummyData);

        return manager;
    }
}
