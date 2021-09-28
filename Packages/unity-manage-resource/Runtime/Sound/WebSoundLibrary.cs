using System.Runtime.InteropServices;

public static class WebSoundLibrary
{
    [DllImport("__Internal")]
    private static extern void _PlaySound(string soundKey);

    [DllImport("__Internal")]
    private static extern void _SetMute(string soundKey, int mute);

    [DllImport("__Internal")]
    private static extern void _SetLoop(string soundKey, int loop);

    [DllImport("__Internal")]
    private static extern void _Stop(string soundKey);

    [DllImport("__Internal")]
    private static extern void _Pause(string soundKey);

    [DllImport("__Internal")]
    private static extern void _SetVolume(string soundKey, float volume);

    [DllImport("__Internal")]
    private static extern float _Duration(string soundKey);

    [DllImport("__Internal")]
    private static extern void _SetPitch(string soundKey, float pitch);

    public static void PlaySound(string soundKey)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        _PlaySound(soundKey);
#endif
    }

    public static void SetMute(string soundKey, bool mute)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        _SetMute(soundKey, mute ? 1 : 0);
#endif
    }

    public static void SetLoop(string soundKey, bool loop)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        _SetLoop(soundKey, loop ? 1 : 0);
#endif
    }

    public static void Stop(string soundKey)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        _Stop(soundKey);
#endif
    }

    public static void Pause(string soundKey)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        _Pause(soundKey);
#endif
    }

    public static void SetVolume(string soundKey, float volume)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        _SetVolume(soundKey, volume);
#endif
    }

    public static float GetDuration(string soundKey)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return _Duration(soundKey);
#else
        return -1f;
#endif
    }

    public static void SetPitch(string soundKey, float pitch)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        _SetPitch(soundKey, pitch);
#endif
    }
}