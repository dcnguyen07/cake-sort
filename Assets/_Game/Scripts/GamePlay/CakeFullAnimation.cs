using DG.Tweening;
using UnityEngine;

public class CakeFullAnimation : MonoBehaviour
{
    [SerializeField] float timeDoScale;
    [SerializeField] Vector3 scaleTo;
    [SerializeField] AnimationCurve curveScale;
    PanelTotal panelTotal;
    public void AnimDoneCake() {
        if (panelTotal == null)
        {
            panelTotal = UIManager.instance.GetPanel(UIPanelType.PanelTotal).GetComponent<PanelTotal>();
        }
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(scaleTo, timeDoScale).SetEase(curveScale));
        sequence.Append(DOVirtual.DelayedCall(timeDoScale, () => { }));
        sequence.Append(transform.DOScale(Vector3.zero, timeDoScale));
        sequence.OnComplete(() => {
            EventManager.TriggerEvent(EventName.ChangeExp.ToString());
            Destroy(gameObject);
        });
    }
}
