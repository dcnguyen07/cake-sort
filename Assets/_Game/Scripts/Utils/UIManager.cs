using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager instance;
    [SerializeField] Transform mainCanvas;
    Dictionary<UIPanelType, GameObject> listPanel = new Dictionary<UIPanelType, GameObject>();
    public bool isHasPopupOnScene = false;
    [SerializeField] RectTransform myRect;
    [SerializeField] GameObject ingameDebugConsole;
    [SerializeField] GameObject objBlock;
  
    public PanelTotal panelTotal;

    [SerializeField] GameObject objSpawnPanel;
    [SerializeField] GameObject objEffect;

    void Awake() {
        instance = this;
    }

    public void TurnBlock(bool active) {
        objBlock.SetActive(active);
    }
    private void Start()
    {
        ShowPanelTotal();
    }
    public void RegisterPanel(UIPanelType type, GameObject obj)
    {
        GameObject go = null;
        if (!listPanel.TryGetValue(type, out go))
        {
            listPanel.Add(type, obj);
        }
        obj.SetActive(false);
    }
    public bool IsHavePanel(UIPanelType type) {
        GameObject panel = null;
        return listPanel.TryGetValue(type, out panel);
    }
    public GameObject GetPanel(UIPanelType type) {
        GameObject panel = null;
        if (!listPanel.TryGetValue(type, out panel)) {
            switch (type) {
                case UIPanelType.PanelTotal:
                    panel = Instantiate(Resources.Load("UI/PanelTotal") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelCakeReward:
                    panel = Instantiate(Resources.Load("UI/PanelCakeReward") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelLevelComplete:
                    panel = Instantiate(Resources.Load("UI/PanelLevelComplete") as GameObject, mainCanvas);
                    break;
            }
            if (panel) panel.SetActive(true);
            return panel;
        }
        return listPanel[type];
    }
    
    public void ShowPanelTotal()
    {
        GameObject go = GetPanel(UIPanelType.PanelTotal);
        go.SetActive(true);
        if(panelTotal ==  null)
        {
            panelTotal = go.GetComponent<PanelTotal>();
        }
    }
    public void ShowPanelCakeReward()
    {
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelCakeReward);
        go.SetActive(true);
    }
    public void ClosePanelCakeReward()
    {
        isHasPopupOnScene = false;
        GameManager.Instance.cakeManager.SetOnMove(false);
        GameObject go = GetPanel(UIPanelType.PanelCakeReward);
        go.SetActive(false);
    }
    PanelLevelComplete panelLevelComplete;
    public void ShowPanelLevelComplete(bool isWinGame)
    {
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelLevelComplete);
        go.SetActive(true);
        if (panelLevelComplete == null) { panelLevelComplete = go.GetComponent<PanelLevelComplete>(); }
        panelLevelComplete.ShowPanel(isWinGame);
    }
    public void ClosePanelLevelComplete()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelLevelComplete);
        go.SetActive(false);
    }


    public void ShowPanelUsingItem()
    {
        GameObject go = GetPanel(UIPanelType.PanelUsingItem);
        go.SetActive(true);
    }
}
