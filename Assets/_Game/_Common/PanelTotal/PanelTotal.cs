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
    [SerializeField] CanvasGroup canvasGroup;
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

        txtCurrentLevel.text = "1";
    }

    void Start()
    {

        currentLevel = 0;
        ChangeLevel();
        ChangeExp();
    }

}
