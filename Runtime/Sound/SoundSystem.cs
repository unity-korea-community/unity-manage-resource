using UNKO.ManageResource;

namespace UNKO.ManageResource
{
    public static class SoundSystem
    {
        public static ISoundManager Manager { get; private set; }

        public static void Init(ISoundManager manager)
        {
            SoundSystem.Manager = manager;
        }
    }
}
