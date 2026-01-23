public class GameManager : Singleton<GameManager>
{
    public bool playing = false;
    public CameraManager cameraManager;
    public AudioManager audioManager;
    public CakeManager cakeManager;
    public ObjectPooling objectPooling;
    public ItemManager itemManager;
    public LightManager lightManager;
    private void Start()
    {
        Play();
    }

    public void Play()
    {
        audioManager.PlaySoundEffect(SoundId.SFX_UIButton);
        cameraManager.FirstCamera();
        cameraManager.OpenMainCamera();
        PlayGame();
    }
    public void PlayGame()
    {
        cakeManager.PlayGame();
        playing = true;
    }

    public float GetDefaultCakeProfit(int cakeID, int level, bool booster = false)
    {
        return (ConstantValue.VAL_DEFAULT_EXP + ConstantValue.VAL_DEFAULT_CAKE_ID * cakeID + ConstantValue.VAL_DEFAULT_CAKE_LEVEL * (level - 1))
            * (booster ? (ProfileManager.Instance.playerData.playerResourseSave.HasX2Booster() ? 2 : 1) : 1);
    }

    public void GetLevelUpReward(bool getLevelCake = true)
    {
        if(getLevelCake) 
            GetLevelCake();
    }

    void GetLevelCake()
    {
        int level = ProfileManager.Instance.playerData.playerResourseSave.currentLevel;
        ItemData firstCake = new();
        firstCake.ItemType = ItemType.Cake;
        int newCakeID = ProfileManager.Instance.dataConfig.levelDataConfig.GetCakeID(level - 1);
        firstCake.amount = 1;
        if (newCakeID != -1)
        {
            ProfileManager.Instance.playerData.cakeSaveData.AddCake(newCakeID);
            ProfileManager.Instance.playerData.cakeSaveData.UseCake(newCakeID);
        }
        firstCake.subId = newCakeID;
        AddItem(firstCake);
    }
    
    public void AddItem(ItemData item)
    {
        if (item.ItemType != ItemType.Cake)
            ProfileManager.Instance.playerData.playerResourseSave.AddItem(item);
        else
            ProfileManager.Instance.playerData.cakeSaveData.AddCakeCard(item.subId, (int)item.amount);
    }
}