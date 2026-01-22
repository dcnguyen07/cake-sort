using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace UIAnimation
{
    public static class UIAnimationController {
        static Vector3 vectorDefault = Vector3.one;
        static Vector3 vectorRotate3 = new Vector3(0, 0, 3);
        static Vector3 vectorRotate0 = Vector3.zero;
        static Vector3 vectorScaleTo09 = new Vector3(.9f, .9f, .9f);
        static Vector3 vectorMove;
        static Vector3 vectorStart;
        public static void BtnAnimType1(Transform trsDoAnim, float duration, UnityAction actioncallBack = null)
        {
            Sequence mainSquence = DOTween.Sequence();
            Sequence scaleSequence = DOTween.Sequence();
            scaleSequence.Append(trsDoAnim.DOScale(vectorScaleTo09, duration / 2));
            scaleSequence.Join(trsDoAnim.DORotate(-vectorRotate3, duration / 2));
            scaleSequence.Append(trsDoAnim.DOScale(vectorDefault, duration / 2));
            scaleSequence.Join(trsDoAnim.DORotate(vectorRotate0, duration / 2));

            mainSquence.Append(scaleSequence);
            mainSquence.Play();
            mainSquence.OnComplete(() =>
            {
                if (actioncallBack != null)
                    actioncallBack();
            });
        }

        public static void BtnAnimZoomBasic(Transform trsDoAnim, float duration, UnityAction actioncallBack = null)
        {
            Sequence mainSquence = DOTween.Sequence();
            mainSquence.Append(trsDoAnim.DOScale(vectorScaleTo09, duration / 2));
            mainSquence.Append(trsDoAnim.DOScale(vectorDefault, duration / 2));
            mainSquence.Play();
            mainSquence.OnComplete(() =>
            {
                if (actioncallBack != null)
                    actioncallBack();
            });
        }
    }
}
