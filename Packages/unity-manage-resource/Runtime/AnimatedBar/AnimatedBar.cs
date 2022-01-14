using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UNKO.ManageUGUI.AnimatedBarLogic;

namespace UNKO.ManageUGUI
{
    /// <summary>
    /// 체력바, 경험치바 등에 사용하는 애니메이션 바.
    /// <para>주의) 이 객체는 바로 사용할 수 없습니다.</para>
    /// <para><see cref="Init"/>를 통해 로직을 Add하여 사용해야 합니다.</para>
    /// </summary>
    public class AnimatedBar : MonoBehaviour
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public enum EDirection
        {
            None,

            /// <summary>
            /// 증가했을 때
            /// </summary>
            Increase = 1 << 0,

            /// <summary>
            /// 감소했을 때
            /// </summary>
            Decrease = 1 << 1,

            /// <summary>
            /// 증가 및 감소 둘다 해당됐을 때
            /// </summary>
            Both = Increase + Decrease,
        }

        /* public - Field declaration            */

        public delegate void delegate_OnChange_FillAmount(float beforeFillAmount_0_1, float afterFillAmount_0_1, EDirection direction);

        /// <summary>
        /// 이 Bar의 FillAmount가 변경될 때
        /// </summary>
        public event delegate_OnChange_FillAmount OnChangeFillAmount;

        [SerializeField]
        Image fillImage = null;

        /* protected & private - Field declaration         */

        List<IAnimatedBarLogic> _OnIncrease_Logics = new List<IAnimatedBarLogic>();
        List<IAnimatedBarLogic> _OnDecrease_Logics = new List<IAnimatedBarLogic>();

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        /// <summary>
        /// 동작할 Logic을 Add합니다. 로직은 namespace <see cref="IAnimatedBarLogic"/>를 참고바랍니다.
        /// </summary>
        public void Init(AnimatedBarLogicFactory logicFactory)
        {
            _OnIncrease_Logics.Clear();
            _OnDecrease_Logics.Clear();

            foreach (var item in logicFactory._logicContainer)
            {
                switch (item.Key)
                {
                    case EDirection.Increase: _OnIncrease_Logics.AddRange(item.Value); break;
                    case EDirection.Decrease: _OnDecrease_Logics.AddRange(item.Value); break;

                    case EDirection.Both:
                        _OnIncrease_Logics.AddRange(item.Value);
                        _OnDecrease_Logics.AddRange(item.Value);
                        break;
                }
            }
        }

        /// <summary>
        /// Fill Image를 세팅합니다.
        /// </summary>
        /// <param name="fillImage"></param>
        public void Set_FillImage(Image fillImage)
        {
            this.fillImage = fillImage;
        }

        /// <summary>
        /// Fill Amount를 애니메이션 없이 세팅합니다.
        /// </summary>
        public void Set_BarFill(float fill_0_1)
        {
            fillImage.fillAmount = fill_0_1;
        }

        /// <summary>
        /// Fill Amount를 세팅과 동시에 등록된 효과를 실행합니다.
        /// </summary>
        public void SetBarFill_And_PlayAnimation(float fill_0_1)
        {
            if (fillImage.fillAmount < fill_0_1)
                SetFillAmount_OnIncrease(fill_0_1);
            else if (fillImage.fillAmount > fill_0_1)
                SetFillAmount_OnDecrease(fill_0_1);
        }

        /// <summary>
        /// 이 Bar의 FillAmount가 변경될 때 이벤트를 모두 지웁니다.
        /// </summary>
        public void AllClearListener_OnChangeFillAmount()
        {
            OnChangeFillAmount = null;
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        void Update()
        {
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < _OnIncrease_Logics.Count; i++)
                _OnIncrease_Logics[i].IAnimatedBarLogic_OnUpdate(deltaTime);

            for (int i = 0; i < _OnDecrease_Logics.Count; i++)
                _OnDecrease_Logics[i].IAnimatedBarLogic_OnUpdate(deltaTime);
        }


        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        void SetFillAmount_OnIncrease(float fillAmount_0_1)
        {
            float beforeFill = fillImage.fillAmount;
            fillImage.fillAmount = fillAmount_0_1;

            for (int i = 0; i < _OnIncrease_Logics.Count; i++)
                _OnIncrease_Logics[i].IAnimatedBarLogic_OnStartAnimation(beforeFill, fillAmount_0_1, EDirection.Increase);

            OnChangeFillAmount?.Invoke(beforeFill, fillAmount_0_1, EDirection.Increase);
        }

        void SetFillAmount_OnDecrease(float fillAmount_0_1)
        {
            float beforeFill = fillImage.fillAmount;
            fillImage.fillAmount = fillAmount_0_1;

            for (int i = 0; i < _OnDecrease_Logics.Count; i++)
                _OnDecrease_Logics[i].IAnimatedBarLogic_OnStartAnimation(beforeFill, fillAmount_0_1, EDirection.Decrease);

            OnChangeFillAmount?.Invoke(beforeFill, fillAmount_0_1, EDirection.Decrease);
        }

        #endregion Private
    }
}
