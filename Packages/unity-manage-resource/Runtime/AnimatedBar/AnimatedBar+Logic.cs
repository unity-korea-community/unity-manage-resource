using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UNKO.ManageUGUI.AnimatedBarLogic;
using static UNKO.ManageUGUI.AnimatedBar;

namespace UNKO.ManageUGUI
{
    public enum EAnimatedBarLogicName
    {
        /// <summary>
        /// 바가 깜빡이는 로직
        /// </summary>
        AnimatedBarLogic_Blink_Image,

        /// <summary>
        /// 증가, 감소를 스무스하게 연출해주는 로직
        /// </summary>
        AnimatedBarLogic_Shrink,
    }

    public class AnimatedBarLogicFactory
    {
        public Dictionary<EDirection, List<IAnimatedBarLogic>> _logicContainer = new Dictionary<EDirection, List<IAnimatedBarLogic>>();

        public IAnimatedBarLogic DoCreate_LibraryLogic(EDirection whenDirection, EAnimatedBarLogicName logicName, Image targetImage)
        {
            IAnimatedBarLogic logic;
            switch (logicName)
            {
                case EAnimatedBarLogicName.AnimatedBarLogic_Blink_Image: logic = new AnimatedBarLogic_Blink_Image(); break;
                case EAnimatedBarLogicName.AnimatedBarLogic_Shrink: logic = new AnimatedBarLogic_Shrink(); break;

                default: Debug.LogError($"Not Found LogicName{logicName}"); return null;
            }
            logic.IAnimatedBarLogic_OnAwake(targetImage);

            if (_logicContainer.ContainsKey(whenDirection) == false)
                _logicContainer.Add(whenDirection, new List<IAnimatedBarLogic>());
            _logicContainer[whenDirection].Add(logic);

            return logic;
        }
    }


    namespace AnimatedBarLogic
    {
        /// <summary>
        /// <see cref="AnimatedBar"/>의 애니메이션 로직.
        /// </summary>
        public interface IAnimatedBarLogic
        {
            void IAnimatedBarLogic_OnAwake(Image image);
            void IAnimatedBarLogic_OnStartAnimation(float beforeFillAmount_0_1, float afterFillAmount_0_1, AnimatedBar.EDirection direction);
            void IAnimatedBarLogic_OnUpdate(float deltaTime);
        }


        /// <summary>
        /// 타겟 이미지가 붉은색으로 깜빡입니다.
        /// </summary>
        [System.Serializable]
        public class AnimatedBarLogic_Blink_Image : IAnimatedBarLogic
        {
            public Color animateColor = Color.red;
            public float duration = 1f;

            Image _image;
            Color _currentColor;
            float _remainTime;

            public void IAnimatedBarLogic_OnAwake(Image image)
            {
                _image = image;
                _image.gameObject.SetActive(false);
            }

            public void IAnimatedBarLogic_OnStartAnimation(float beforeFillAmount_0_1, float AfterfillAmount_0_1, AnimatedBar.EDirection direction)
            {
                if (direction == AnimatedBar.EDirection.Increase)
                    return;

                if (_remainTime <= 0f)
                    _image.fillAmount = beforeFillAmount_0_1;

                _remainTime = duration;

                _currentColor = animateColor;

                _image.gameObject.SetActive(true);
                _image.color = _currentColor;
            }

            public void IAnimatedBarLogic_OnUpdate(float fDeltaTime)
            {
                if (_remainTime <= 0f)
                    return;
                _remainTime -= fDeltaTime;

                if (_remainTime > 0f)
                {
                    _currentColor.a -= fDeltaTime / _remainTime;
                    _image.color = _currentColor;
                }
                else
                {
                    _image.gameObject.SetActive(false);
                }
            }
        }

        [System.Serializable]
        public class AnimatedBarLogic_Shrink : IAnimatedBarLogic
        {
            public float duration = 1f;

            Image _image;
            float _offsetAmount;
            float _remainTime;

            public void IAnimatedBarLogic_OnAwake(Image image)
            {
                _image = image;
            }

            public void IAnimatedBarLogic_OnStartAnimation(float beforeFillAmount_0_1, float afterFillAmount_0_1, AnimatedBar.EDirection direction)
            {
                _remainTime = duration;
                _offsetAmount = afterFillAmount_0_1 - beforeFillAmount_0_1;

                _image.enabled = true;
                _image.fillAmount = beforeFillAmount_0_1;
            }

            public void IAnimatedBarLogic_OnUpdate(float deltaTime)
            {
                if (_remainTime <= 0f)
                    return;
                _remainTime -= deltaTime;

                if (_remainTime > 0f)
                {
                    _image.fillAmount += _offsetAmount * (deltaTime / _remainTime);
                }
                else
                {
                    _image.enabled = false;
                }
            }
        }
    }
}
