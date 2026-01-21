using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour {
    public static UIManager instance;
    [SerializeField] Transform mainCanvas;
    //public GameObject backGround;
    Dictionary<UIPanelType, GameObject> listPanel = new Dictionary<UIPanelType, GameObject>();
    public bool isHasPopupOnScene = false;
    public bool isHasTutorial = false;
    public bool isHasShopPanel = false;
    public bool isHasPanelIgnoreTutorial = false;
    [SerializeField] RectTransform myRect;
    [SerializeField] GameObject ingameDebugConsole;
    [SerializeField] GameObject objBlock;
  
    public Camera mainCamera;
    public PanelTotal panelTotal;
    public PanelPlayGame panelGamePlay;
    PanelBakery panelBakery;

    [SerializeField] GameObject objSpawnPanel;
    [SerializeField] GameObject objEffect;
    [SerializeField] GameObject objCheat;
    [SerializeField] Button btnCheat;
    [SerializeField] Button btnShowUI;
    [SerializeField] Button btnTurnOffUI;

    // Start is called before the first frame update
    void Awake() {
        instance = this;
    }

    public void TurnBlock(bool active) {
        objBlock.SetActive(active);
    }

    void OffUI() {
        objSpawnPanel.SetActive(false);
        objEffect.SetActive(false);
        objCheat.SetActive(false);
        btnShowUI.gameObject.SetActive(true);
    }

    void ShowUI() {
        objSpawnPanel.SetActive(true);
        objEffect.SetActive(true);
        objCheat.SetActive(true);
        btnShowUI.gameObject.SetActive(false);
    }

    public void ShowPanelCheat() {
        isHasPopupOnScene = true;
        objCheat.SetActive(false);
        GameObject go = GetPanel(UIPanelType.PanelCheat);
        go.transform.SetAsLastSibling();
        go.SetActive(true);
    }
    public void ClosePanelCheat() {
        isHasPopupOnScene = false;
        objCheat.SetActive(true);
        GameObject go = GetPanel(UIPanelType.PanelCheat);
        go.SetActive(false);
    }
    private void Start()
    {
        //uiPooling.FirstPooling();
        //StartCoroutine(WaitToSpawnUIPool());
        // if(ingameDebugConsole != null)
        // ingameDebugConsole.SetActive(ProfileManager.Instance.IsShowDebug());
        ShowPanelTotal();
        // ShowPanelLoading();
        // UnityEngine.iOS.Device.RequestStoreReview();

    }

    public bool isDoneFirstLoadPanel = false;
    bool isFirstLoad;
    public void FirstLoadPanel() {
        //LoadSceneManager.Instance.AddTotalProgress(30);
       
        isDoneFirstLoadPanel = true;
    }

    public void CloseAllPopUp(bool isFirstLoad = false) {
        isHasPopupOnScene = false;
        isHasShopPanel = false;
        foreach (KeyValuePair<UIPanelType, GameObject> panel in listPanel)
        {
            panel.Value.gameObject.SetActive(false);
        }
    }
    public bool IsHavePopUpOnScene() { return isHasPopupOnScene; }
    public bool IsHaveTutorial() { return isHasTutorial; }
    public void RegisterPanel(UIPanelType type, GameObject obj)
    {
        GameObject go = null;
        if (!listPanel.TryGetValue(type, out go))
        {
            //Debug.Log("RegisterPanel " + type.ToString());
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
                case UIPanelType.PanelBase:
                    panel = Instantiate(Resources.Load("UI/PanelBase") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelTotal:
                    panel = Instantiate(Resources.Load("UI/PanelTotal") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelSpin:
                    panel = Instantiate(Resources.Load("UI/PanelSpin") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelDailyReward:
                    panel = Instantiate(Resources.Load("UI/PanelDailyReward") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelItemsReward:
                    panel = Instantiate(Resources.Load("UI/PanelItemsReward") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelBakery:
                    panel = Instantiate(Resources.Load("UI/PanelBakery") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelPlayGame:
                    panel = Instantiate(Resources.Load("UI/PanelPlayGame") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelSetting:
                    panel = Instantiate(Resources.Load("UI/PanelSetting") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelDecorations:
                    panel = Instantiate(Resources.Load("UI/PanelDecorations") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelCakeReward:
                    panel = Instantiate(Resources.Load("UI/PanelCakeReward") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelLevelComplete:
                    panel = Instantiate(Resources.Load("UI/PanelLevelComplete") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelLoading:
                    panel = Instantiate(Resources.Load("UI/PanelLoading") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelShop:
                    panel = Instantiate(Resources.Load("UI/PanelShop") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelUsingItem:
                    panel = Instantiate(Resources.Load("UI/PanelUsingItem") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelLeaderBoard:
                    panel = Instantiate(Resources.Load("UI/PanelLeaderBoard") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelTopUp:
                    panel = Instantiate(Resources.Load("UI/PanelTopUp") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelQuickIAP:
                    panel = Instantiate(Resources.Load("UI/PanelQuickIAP") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelSelectReward:
                    panel = Instantiate(Resources.Load("UI/PanelSelectReward") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelHint:
                    panel = Instantiate(Resources.Load("UI/PanelHint") as GameObject, mainCanvas);
                    break;  
                case UIPanelType.PanelQuickTimeEvent:
                    panel = Instantiate(Resources.Load("UI/PanelQuickTimeEvent") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelDailyQuest:
                    panel = Instantiate(Resources.Load("UI/PanelDailyQuest") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelPreAds:
                    panel = Instantiate(Resources.Load("UI/PanelPreAds") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelCheat:
                    panel = Instantiate(Resources.Load("UI/CheatPanel") as GameObject, mainCanvas);
                    break;
                case UIPanelType.PanelTutorial:
                    panel = Instantiate(Resources.Load("UI/PanelTutorial") as GameObject, mainCanvas);
                    break;
            }
            if (panel) panel.SetActive(true);
            return panel;
        }
        return listPanel[type];
    }

    public void ShowPanelBase()
    {
        GameObject go = GetPanel(UIPanelType.PanelBase);
        go.SetActive(true);
    }

    public void ClosePanelBase() {
        GameObject go = GetPanel(UIPanelType.PanelBase);
        go.SetActive(false);
    }

    public void ShowPanelTutorial()
    {
        GameObject go = GetPanel(UIPanelType.PanelTutorial);
        go.SetActive(true);
    }

    public void ClosePanelTutorial()
    {
        GameObject go = GetPanel(UIPanelType.PanelTutorial);
        go.SetActive(false);
    }

    public void ShowPanelHint(ItemType itemType)
    {
        GameObject go = GetPanel(UIPanelType.PanelHint);
        go.SetActive(true);
        go.GetComponent<PanelHint>().ShowComponent(itemType);
        go.transform.SetAsLastSibling();
    }

    public void ClosePanelHint() {
        GameObject go = GetPanel(UIPanelType.PanelHint);
        go.SetActive(false);
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

    public void ClosePanelTotal()
    {
        GameObject go = GetPanel(UIPanelType.PanelTotal);
        go.SetActive(false);
    }
    public void ShowPanelTotalContent()
    {
        CloseOtherMenu(UIPanelType.PanelTotal);
        if (panelTotal == null)
        {
            GameObject go = GetPanel(UIPanelType.PanelTotal);
            go.SetActive(true);
            panelTotal = go.GetComponent<PanelTotal>();
        }
        panelTotal.ShowMainSceneContent(true);
    }

    public void ShowPanelPlayGame()
    {
        if (panelGamePlay == null)
        {
            GameObject go = GetPanel(UIPanelType.PanelPlayGame);
            panelGamePlay = go.GetComponent<PanelPlayGame>();
        }
        panelGamePlay.gameObject.SetActive(true);
        panelTotal.Transform.SetAsLastSibling();
    }
    public void ClosePanelPlayGame()
    {
        GameObject go = GetPanel(UIPanelType.PanelPlayGame);
        go.SetActive(false);
    }


    public void ShowPanelSpin()
    {
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelSpin);
        go.SetActive(true);
    }
    public void ClosePanelSpin()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelSpin);
        go.SetActive(false);
    }
    public void ShowPanelDailyReward()
    {
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelDailyReward);
        go.SetActive(true);
    }
    public void ClosePanelDailyReward()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelDailyReward);
        go.SetActive(false);
    }

    public void ShowPanelItemsReward()
    {
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelItemsReward);
        go.SetActive(true);
    }
    public void ClosePanelItemsReward()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelItemsReward);
        go.SetActive(false);
    }

    public void ShowPanelBakery()
    {
        CloseOtherMenu(UIPanelType.PanelBakery);
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelBakery);
        go.SetActive(true);
        panelTotal.ShowMainSceneContent(false);
        panelTotal.Transform.SetAsLastSibling();
        if(panelBakery == null)
        {
            panelBakery = go.GetComponent<PanelBakery>();
        }
    }
    public void ClosePanelBakery()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelBakery);
        go.SetActive(false);
    }

    void CloseOtherMenu(UIPanelType ignorePanel)
    {
        if(panelBakery != null && ignorePanel != UIPanelType.PanelBakery)
        {
            if(panelBakery.gameObject.activeSelf)
            {
                panelBakery.OnClose();
            }
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
        //isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelUsingItem);
        go.SetActive(true);
    }
    public void ClosePanelUsingItem()
    {
        //isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelUsingItem);
        go.SetActive(false);
    }

    public void ShowPanelQuickTimeEvent() {
        isHasPopupOnScene = true;
        GameObject go = GetPanel(UIPanelType.PanelQuickTimeEvent);
        go.SetActive(true);
    }

    public void ClosePanelQuickTimeEvent()
    {
        isHasPopupOnScene = false;
        GameObject go = GetPanel(UIPanelType.PanelQuickTimeEvent);
        go.SetActive(false);
    }

    public void ShowPanelPreAds() {
        if (ProfileManager.Instance.versionStatus == VersionStatus.Cheat) return;
        GameObject go = GetPanel(UIPanelType.PanelPreAds);
        go.SetActive(true);
    }

    public void ClosePanelPreAds()
    {
        GameObject go = GetPanel(UIPanelType.PanelPreAds);
        go.SetActive(false);
    }


}
