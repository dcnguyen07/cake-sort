using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIAnimation;
using UnityEngine;
using UnityEngine.UI;

public class PanelPlayGame : UIPanel
{
    [SerializeField] GameObject x2BoosterBar;
    [SerializeField] Button x2BoosterBtn;
    [SerializeField] TextMeshProUGUI x2BoosterTimeTxt;
    [SerializeField] TextMeshProUGUI x2BoosterTimeRemainTxt;
    float x2BoosterTimeRemain;
    [SerializeField] Button coinBoosterBtn;
    [SerializeField] TextMeshProUGUI coinBoosterEarnTxt;
    [SerializeField] Button bakeryBtn;
    [SerializeField] Button questBtn;
    [SerializeField] GameObject questNoti;
    [SerializeField] GameObject bakeryNoti;
    [SerializeField] Transform coinPos;

    [SerializeField] BoosterItemButton btnHammer;
    [SerializeField] BoosterItemButton btnFillUp;
    [SerializeField] BoosterItemButton btnReroll;

    [SerializeField] List<TransitionUI> transitionUIList;
    public override void Awake()
    {
        panelType = UIPanelType.PanelPlayGame;
        base.Awake();
        coinBoosterEarnTxt.text = ConstantValue.VAL_COIN_BOOSTER.ToString();
        //x2BoosterTimeTxt.text = ConstantValue.VAL_X2BOOSTER_TIME.ToString() + ConstantValue.STR_MINUTE;
        x2BoosterTimeTxt.text = "x2";
        btnHammer.SetActionCallBack(UsingHammer);
        btnFillUp.SetActionCallBack(UsingItemFillUp);
        btnReroll.SetActionCallBack(UsingReroll);

        coinBoosterBtn.onClick.AddListener(CoinBoosterOnClick);
        x2BoosterBtn.onClick.AddListener(X2BoosterOnClick);

        bakeryBtn.onClick.AddListener(() => {
            GameManager.Instance.audioManager.PlaySoundEffect(SoundId.SFX_UIButton);
            UIManager.instance.ShowPanelBakery();
            UIManager.instance.ClosePanelPlayGame();
        });
        CheckBooster();
        EventManager.AddListener(EventName.AddItem.ToString(), CheckBooster);
    }

    void CheckBooster()
    {
        btnHammer.gameObject.SetActive(ProfileManager.Instance.playerData.playerResourseSave.currentLevel >= 1);
        btnFillUp.gameObject.SetActive(ProfileManager.Instance.playerData.playerResourseSave.currentLevel >= 2);
        btnReroll.gameObject.SetActive(ProfileManager.Instance.playerData.playerResourseSave.currentLevel >= 3);
    }

    void UsingHammer()
    {
        GameManager.Instance.audioManager.PlaySoundEffect(SoundId.SFX_UIButton);
        GameManager.Instance.itemManager.UsingItem(ItemType.Hammer);
        btnHammer.UpdateStatus();
    }

    void UsingItemFillUp() {
        GameManager.Instance.audioManager.PlaySoundEffect(SoundId.SFX_UIButton);
        GameManager.Instance.itemManager.UsingItem(ItemType.FillUp);
        btnFillUp.UpdateStatus();
    }

    void UsingReroll() {
        GameManager.Instance.audioManager.PlaySoundEffect(SoundId.SFX_UIButton);
        GameManager.Instance.itemManager.UsingItem(ItemType.ReRoll);
        btnReroll.UpdateStatus();
    }

    public void UsingItemMode()
    {
        for (int i = 0; i < transitionUIList.Count; i++)
        {
            transitionUIList[i].OnShow(false);
        }
    }

    public void OutItemMode()
    {
        for (int i = 0; i < transitionUIList.Count; i++)
        {
            transitionUIList[i].OnShow(true);
        }
    }

    void X2BoosterOnClick()
    {
        GameManager.Instance.audioManager.PlaySoundEffect(SoundId.SFX_UIButton);
        UIAnimationController.BtnAnimZoomBasic(x2BoosterBtn.transform, .1f);
    }

   
    void CoinBoosterOnClick()
    {
        GameManager.Instance.audioManager.PlaySoundEffect(SoundId.SFX_UIButton);
        UIAnimationController.BtnAnimZoomBasic(coinBoosterBtn.transform, .1f);
    }
    ItemData coinBoosterReward;
    List<ItemData> coinBoosterRewards;
    void CoinBoosterSuccess()
    {
        if(coinBoosterRewards == null)
        {
            coinBoosterRewards = new List<ItemData>();
            coinBoosterReward = new ItemData();
            coinBoosterReward.ItemType = ItemType.Coin;
            coinBoosterReward.amount = ConstantValue.VAL_COIN_BOOSTER;
            coinBoosterRewards.Add(coinBoosterReward);
        }
        GameManager.Instance.GetItemRewards(coinBoosterRewards);
        UIManager.instance.ShowPanelItemsReward();
        //ProfileManager.Instance.playerData.playerResourseSave.AddMoney(ConstantValue.VAL_COIN_BOOSTER);
        EventManager.TriggerEvent(EventName.ChangeCoin.ToString());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UsingItemMode();
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            OutItemMode();
        }
        if(x2BoosterTimeRemain > 0)
        {
            x2BoosterTimeRemain -= Time.deltaTime;
            if (x2BoosterTimeRemain <= 0)
            {
                x2BoosterTimeRemain = 0;
            }
            x2BoosterTimeRemainTxt.text = TimeUtil.TimeToString(x2BoosterTimeRemain, TimeFommat.Symbol);
        }
    }

    public Transform GetBoosterPos(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.NoAds:
                break;
            case ItemType.Cake:
                return bakeryBtn.transform;
            case ItemType.None:
                break;
            case ItemType.Trophy:
                break;
            case ItemType.Coin:
                return coinPos;
            case ItemType.Swap:
                break;
            case ItemType.Hammer:
                return btnHammer.transform;
            case ItemType.ReRoll:
                return  btnReroll.transform;
            case ItemType.FillUp:
                return btnFillUp.transform;
            default:
                break;
        }
        return null;
    }
}
