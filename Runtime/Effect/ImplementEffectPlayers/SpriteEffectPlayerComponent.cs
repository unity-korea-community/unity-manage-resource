using System.Collections;
using UnityEngine;

namespace UNKO.ManageResource
{
    [RequireComponent(typeof(SpriteAnimation))]
    public class SpriteEffectPlayerComponent : EffectPlayerComponentBase
    {
        [SerializeField]
        SpriteAnimation _spriteAnimation = null;

        void Awake()
        {
            if (_spriteAnimation == null)
            {
                _spriteAnimation = GetComponent<SpriteAnimation>();
                if (_spriteAnimation == null)
                {
                    Debug.LogError($"{name} _spriteAnimation == null", this);
                }
            }
        }

        public override bool IsPlayingResource()
            => _spriteAnimation.IsPlaying;

        public override IEnumerator PlayCoroutine()
            => _spriteAnimation.Play();

        public override void Reset()
        {
            _spriteAnimation.Stop();
        }

        public override void SetLoop(bool isLoop)
            => _spriteAnimation.SetIsLoop(isLoop);

        public override void Stop()
            => _spriteAnimation.Stop();
    }
}