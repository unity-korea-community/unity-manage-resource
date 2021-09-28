using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UNKO.ManageResource;

[Category("UNKO")]
public class EffectManagerTests
{
    public class EffectData : IEffectData
    {
        string _effectID;
        EffectPlayerComponentBase _effectPlayerComponent;

        public EffectData(string effectID)
        {
            _effectID = effectID;
            _effectPlayerComponent = new GameObject()
                .AddComponent<NullEffectPlayerComponent>()
                .SetDuration(0.03f);
        }

        public string GetEffectID() => _effectID;
        public EffectPlayerComponentBase GetEffectPlayer() => _effectPlayerComponent;
    }

    public enum EffectDataKey
    {
        effect1,
        effect2,
    }

    EffectData[] dummyData = new EffectData[] {
        new EffectData(EffectDataKey.effect1.ToString()),
        new EffectData(EffectDataKey.effect2.ToString())
    };

    [Test]
    public void PoolingTest()
    {
        EffectManager manager = GetManager();
        int randomCount = Random.Range(3, 5);
        for (int i = 0; i < randomCount; i++)
        {
            var command = manager.PlayEffect(EffectDataKey.effect1.ToString());
            command.Dispose();
        }

        Assert.AreEqual(0, manager.CommandPool.Use.Count);
        Assert.AreEqual(1, manager.CommandPool.AllInstance.Count);
    }

    [Test]
    public void PlayStopTest()
    {
        EffectManager manager = GetManager();

        var command = manager.PlayEffect(EffectDataKey.effect1.ToString());
        Assert.IsTrue(command.IsPlayingResource());

        command.Stop();
        Assert.IsFalse(command.IsPlayingResource());
    }

    [UnityTest]
    public IEnumerator SetDelayTest()
    {
        // Arrange
        EffectManager manager = GetManager();
        float delay = 0.2f;

        // Act
        var command = manager.GetEffect(EffectDataKey.effect1.ToString())
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
        EffectManager manager = GetManager();
        float delay = 0.2f;
        bool isStarted = false;
        bool isFinished = false;

        // Act
        var command = manager.GetEffect(EffectDataKey.effect1.ToString())
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

    private EffectManager GetManager()
    {
        var manager = new EffectManager(null).AddData(dummyData);

        return manager;
    }
}
