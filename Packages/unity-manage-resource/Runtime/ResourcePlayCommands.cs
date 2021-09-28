using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UNKO.ManageResource;
using UNKO.Utils;

public class ResourcePlayCommands : IResourcePlayCommand
{
    public static SimplePool<ResourcePlayCommands> Pool { get; private set; } = new SimplePool<ResourcePlayCommands>(new ResourcePlayCommands());
    public static int UsedCommandCount => Pool.Use.Count;
    public static int CommandInstanceCount => Pool.AllInstance.Count;

    public event Action<IResourcePlayer> OnPlayStart;
    public event Action<IResourcePlayer> OnPlayFinish;

    List<IResourcePlayCommand> _commands = new List<IResourcePlayCommand>();

    public float Delay { get; set; }

    public static ResourcePlayCommands Create(params IResourcePlayCommand[] playCommands)
    {
        ResourcePlayCommands createCommands = Pool.Spawn();
        foreach (var command in playCommands)
        {
            createCommands.AddCommand(command);
        }

        return createCommands;
    }

    public IEnumerator PlayCoroutine()
    {
        List<IEnumerator> coroutineList = new List<IEnumerator>();
        foreach (var command in _commands)
        {
            coroutineList.Add(command.PlayCoroutine());
        }

        return coroutineList.GetEnumerator();
    }

    public void AddCommand(IResourcePlayCommand playCommand)
    {
        _commands.Add(playCommand);
    }

    public void RemoveCommand(IResourcePlayCommand playCommand)
    {
        _commands.Remove(playCommand);
    }

    public void Reset()
        => _commands.ForEach(command => command.Reset());

    public void SetLoop(bool isLoop)
        => _commands.ForEach(command => command.SetLoop(isLoop));

    public void Stop()
        => _commands.ForEach(command => command.Stop());

    public virtual void Dispose()
    {
        _commands.ForEach(command => command.Dispose());
        _commands.Clear();
        Pool.DeSpawn(this);
        GC.SuppressFinalize(this);
    }

    public IResourcePlayer GetPlayer()
        => _commands.First().GetPlayer();

    public bool IsPlayingResource()
        => _commands.Any(command => command.IsPlayingResource());

    public Coroutine StartCoroutine(IEnumerator routine)
        => _commands.First().StartCoroutine(routine);

    public void StopCoroutine(IEnumerator routine)
        => _commands.First().StopCoroutine(routine);

    public void SetWorldPosition(Vector3 position)
    {
        foreach (var command in _commands)
        {
            command.SetWorldPosition(position);
        }
    }

    public void SetLocalPosition(Vector3 position)
    {
        foreach (var command in _commands)
        {
            command.SetLocalPosition(position);
        }
    }

    public void SetParentTransform(Transform parent)
    {
        foreach (var command in _commands)
        {
            command.SetParentTransform(parent);
        }
    }
}