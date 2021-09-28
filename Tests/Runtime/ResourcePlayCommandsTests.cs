using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UNKO.ManageResource;

[Category("UNKO")]
public class ResourcePlayCommandsTests
{
    public class DummyResourceCommand : ResourcePlayCommandBase<NullEffectPlayerComponent>
    {
    }

    [SetUp]
    public void Setup()
    {
        ResourcePlayCommands.Pool.DeSpawnAll();
    }

    [Test]
    public void PoolingTest()
    {
        int randomCount = Random.Range(3, 5);
        for (int i = 0; i < randomCount; i++)
        {
            var commands = ResourcePlayCommands.Create();
            commands.PlayCoroutine();
            commands.Dispose();
        }

        Assert.AreEqual(0, ResourcePlayCommands.UsedCommandCount);
        Assert.AreEqual(1, ResourcePlayCommands.CommandInstanceCount);
    }

    [Test]
    public void PlayStopTest()
    {
        // Arrange
        DummyResourceCommand command1 = CreateCommand();
        DummyResourceCommand command2 = CreateCommand();
        var commands = ResourcePlayCommands.Create(command1, command2);


        // Play Act & Assert
        Assert.IsFalse(command1.IsPlayingResource());
        Assert.IsFalse(command2.IsPlayingResource());
        Assert.IsFalse(commands.IsPlayingResource());
        commands.PlayCoroutine();
        Assert.IsTrue(command1.IsPlayingResource());
        Assert.IsTrue(command2.IsPlayingResource());
        Assert.IsTrue(commands.IsPlayingResource());

        // Stop Act & Assert
        commands.Stop();
        Assert.IsFalse(command1.IsPlayingResource());
        Assert.IsFalse(command2.IsPlayingResource());
        Assert.IsFalse(commands.IsPlayingResource());
    }

    private static DummyResourceCommand CreateCommand(float duration = 0.02f)
    {
        NullEffectPlayerComponent resourcePlayerInstance = new GameObject().AddComponent<NullEffectPlayerComponent>();
        resourcePlayerInstance.SetDuration(duration);

        DummyResourceCommand command = new DummyResourceCommand();
        command.Init(resourcePlayerInstance, (thisCommand) => { });

        return command;
    }
}
