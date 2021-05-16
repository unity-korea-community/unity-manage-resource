using UNKO.ManageResource;

namespace UNKO.ManageResource
{
    public static class SoundSystem
    {
        public static ISoundManager manager { get; private set; }

        public static void Init(ISoundManager manager)
        {
            SoundSystem.manager = manager;
        }
    }
}
