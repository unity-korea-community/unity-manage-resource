using System.Collections;
using UnityEngine;

namespace UNKO.ManageResource
{
    public class NullEffectPlayerComponent : EffectPlayerComponentBase
    {
        bool _isPlaying;
        [SerializeField] // NOTE: 이게 없으면 instantiate를 통해 copy를 못함
        float _duration;

        public NullEffectPlayerComponent SetDuration(float duration)
        {
            _duration = duration;

            return this;
        }

        public override bool IsPlayingResource() => _isPlaying;

        public override IEnumerator PlayCoroutine()
        {
            IEnumerator routine = DummyPlay();
            StartCoroutine(routine);

            return routine;
        }

        IEnumerator DummyPlay()
        {
            _isPlaying = true;
            float remainTime = _duration;
            while (remainTime > 0f)
            {
                remainTime -= Time.deltaTime;
                yield return null;
            }

            _isPlaying = false;
        }

        public override void Reset()
        {
            _isPlaying = false;
        }

        public override void SetLoop(bool isLoop)
        {
        }

        public override void Stop()
        {
            _isPlaying = false;
        }
    }
}