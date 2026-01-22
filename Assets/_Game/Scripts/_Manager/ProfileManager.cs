public class ProfileManager : Singleton<ProfileManager>
{
    public VersionStatus versionStatus;
    public PlayerData playerData;
    public DataConfig dataConfig;
    protected override void Awake()
    {
        base.Awake();
        playerData.LoadData();
    }
    private void Update()
    {
        playerData.Update();
    }
}
