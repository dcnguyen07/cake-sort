using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PanelLevelComplete : UIPanel
{
    [SerializeField] Button btnExit;
    [SerializeField] Transform panelWrapTrs;
    [SerializeField] CanvasGroup bgCanvanGroup;
    [SerializeField] GameObject objLooseGame;
    [SerializeField] SheetAnimation loseSheetAnimation;

    public override void Awake()
    {
        panelType = UIPanelType.PanelLevelComplete;
        base.Awake();
    }

    private void OnEnable()
    {
        objLooseGame.SetActive(true);
        panelWrapTrs.DOScale(1, 0.35f).From(0);
        bgCanvanGroup.DOFade(1, 0.35f).From(0);
        transform.SetAsLastSibling();
    }

    void OnClose()
    {
        panelWrapTrs.DOScale(0, 0.35f).From(1).SetEase(Ease.InOutBack);
        bgCanvanGroup.DOFade(0, 0.35f).From(1).OnComplete(ClosePanel);
    }

    void ClosePanel()
    {
        UIManager.instance.ClosePanelLevelComplete();
        //UIManager.instance.ShowPanelLeaderBoard();
    }

    public void ShowPanel(bool isWinGame)
    {
        objLooseGame.SetActive(!isWinGame);
    }
}
