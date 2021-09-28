using UNKO.Utils;

namespace UNKO.ManageResource
{
    [System.Serializable]
    public class EffectPlayCommand : ResourcePlayCommandBase<IEffectPlayer>
    {
        // NOTE inspector 노출용 - 유니티 에디터는 제네릭 클래스를 인스펙터에 노출을 못하기 때문에
        [System.Serializable]
        public class Pool : SimplePool<EffectPlayCommand>
        {
            public Pool(EffectPlayCommand originItem) : base(originItem) { }
            public Pool(System.Func<EffectPlayCommand> onCreateInstance) : base(onCreateInstance) { }
        }

        public string GetEffectID() => ResourcePlayer.GetEffectID();
    }
}