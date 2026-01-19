using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelTotal : UIPanel
{
    public RectTransform subTopRect;
    [SerializeField] UIPanelShowUp uiPanelShowUp;
    public Transform Transform;
    [SerializeField] Button playBtn;
    
    [SerializeField] GameObject mainSceneContent;
    [SerializeField] GameObject mainMenuContent;
    [SerializeField] TextMeshProUGUI txtCurrentLevel;
    [SerializeField] TextMeshProUGUI txtCurrentExp;
    [SerializeField] Slider sliderLevelExp;
    [SerializeField] Slider sliderQuickTimeEvent;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] CanvasGroup quickEventCanvasGroup;

    [SerializeField] GameObject objQuickTimeEvents;

    [SerializeField] TextMeshProUGUI txtCountCake;
    [SerializeField] TextMeshProUGUI txtTime;

    int showingCake = -1;
    public override void Awake()
    {
        panelType = UIPanelType.PanelTotal;
        base.Awake();
        EventManager.AddListener(EventName.ChangeExp.ToString(), ChangeExp);
        //backGround = UIManager.instance.backGround;
        CheckSubScreenObstacleBase();
    }

    void CheckSubScreenObstacleBase()
    {
        if (subTopRect == null)
        {
            return;
        }
        float screenRatio = (float)Screen.height / (float)Screen.width;
        if (screenRatio > 2.1f) // Now we got problem 
        {
            subTopRect.sizeDelta = new Vector2(0, -100);
            subTopRect.anchoredPosition = new Vector2(0, -50);
        }
        else
        {
            subTopRect.sizeDelta = new Vector2(0, 0);
            subTopRect.anchoredPosition = new Vector2(0, 0);
        }
    }

    float currentExp = 0;
    float currentValue = 0;
    int currentLevel;
    bool isChangeLevel;
    private void ChangeExp()
    {
        if (currentLevel != ProfileManager.Instance.playerData.playerResourseSave.currentLevel)
        {
            currentExp = sliderLevelExp.maxValue;
            isChangeLevel = true;
        }
        else
        {
            isChangeLevel = false;
            currentExp = ProfileManager.Instance.playerData.playerResourseSave.currentExp;
            sliderLevelExp.maxValue = ProfileManager.Instance.playerData.playerResourseSave.GetMaxExp();
        }

        currentValue = sliderLevelExp.value;
        DOVirtual.Float(currentValue, currentExp, 1f, (value) => {
            sliderLevelExp.value = value;
            txtCurrentExp.text = (int)value + "/" + sliderLevelExp.maxValue;
        }).OnComplete(() => {
            if (isChangeLevel)
            {
                sliderLevelExp.value = 0;
                sliderLevelExp.maxValue = ProfileManager.Instance.playerData.playerResourseSave.GetMaxExp();
                txtCurrentExp.text = 0 + "/" + sliderLevelExp.maxValue;
                currentLevel = ProfileManager.Instance.playerData.playerResourseSave.currentLevel;
                ChangeLevel();
            }
        });
    }
    LevelData levelData;
    private void ChangeLevel()
    {

        txtCurrentLevel.text = ProfileManager.Instance.playerData.playerResourseSave.currentLevel.ToString();
    }

    void Start()
    {
        playBtn.onClick.AddListener(PlayGame);

        currentLevel = ProfileManager.Instance.playerData.playerResourseSave.currentLevel;
        ChangeLevel();
        ChangeExp();
    }

    public void ShowMainSceneContent(bool show)
    {
        mainSceneContent.gameObject.SetActive(show);
    }

    void PlayGame()
    {
        GameManager.Instance.audioManager.PlaySoundEffect(SoundId.SFX_UIButton);
        GameManager.Instance.cameraManager.FirstCamera();
        GameManager.Instance.cameraManager.OpenMainCamera();
        //GameManager.Instance.PlayGame();
        mainMenuContent.SetActive(false);
        UIManager.instance.ShowPanelLoading();
        DOVirtual.DelayedCall(2.5f, GameManager.Instance.PlayGame);
    }

    public void BackToMenu()
    {
        mainMenuContent.SetActive(true);
    }

    
    public Transform GetPointSlider() {
        return sliderLevelExp.handleRect.transform;
    }

    private void Update()
    {
        showCakeCounter += Time.deltaTime;
        if (showCakeCounter > showCakeCoolDown)
        {
            showCakeCounter = 0;
        }

        if (onQuickTimeEvent) UpdateTime();
    }

    float showCakeCoolDown = 3 * 60;
    [SerializeField] float showCakeCounter = 0;
    void InitCakeDecor()
    {
        if (GameManager.Instance.playing) return;
        int newShow = ProfileManager.Instance.playerData.cakeSaveData.GetRandomOwnedCake();
        while (newShow == showingCake)
        {
            newShow = ProfileManager.Instance.playerData.cakeSaveData.GetRandomOwnedCake();
        }
        showingCake = newShow;
    }


    #region Quick Time Event
    public float currentCakeDone;
    float currentTime;
    bool onQuickTimeEvent;

    public void ShowQuickTimeEvent(float cakeNeedDoneOnEvent, float timeMaxEvent) {
        objQuickTimeEvents.SetActive(true);
        quickEventCanvasGroup.DOFade(1, .25f).From(0).SetEase(Ease.InOutSine);
        objQuickTimeEvents.transform.DOScale(1, .25f).From(0).SetEase(Ease.OutBack);
        sliderQuickTimeEvent.maxValue = cakeNeedDoneOnEvent;
        sliderQuickTimeEvent.value = 0;
        currentCakeDone = 0;
        onQuickTimeEvent = true;
        currentTime = timeMaxEvent;
        txtCountCake.text = "0/" + cakeNeedDoneOnEvent;
        txtTime.text = TimeUtil.ConvertFloatToString(timeMaxEvent);
    }

    public void OutTimeEvent() {
        objQuickTimeEvents.SetActive(false);
        currentCakeDone = 0;
        currentTime = 0;
        onQuickTimeEvent = false;
        GameManager.Instance.quickTimeEventManager.EndQuickTimeEvent();
    }

    public void UpdateQuickTimeEvent()
    {
        if (sliderQuickTimeEvent.maxValue == 0)
            return;
        currentCakeDone++;
        txtCountCake.text = currentCakeDone + "/" + sliderQuickTimeEvent.maxValue;
        sliderQuickTimeEvent.value = currentCakeDone;
        if (currentCakeDone >= sliderQuickTimeEvent.maxValue &&
            GameManager.Instance.quickTimeEventManager.onQuickTimeEvent)
        { 
            UIManager.instance.panelTotal.OutTimeEvent();
            GameManager.Instance.RandonReward();
            UIManager.instance.ShowPanelSelectReward();
        }
    }

    void UpdateTime() {
        currentTime -= Time.deltaTime;
        if (currentTime >= 0f) txtTime.text = TimeUtil.TimeToString(currentTime, TimeFommat.Keyword);
        if (currentTime <= 0f)
            OutTimeEvent();
    }
    #endregion
}
