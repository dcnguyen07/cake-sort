using UnityEngine;

public class ConstantValue
{

    public static float VAL_DEFAULT_EXP = 10f;
    public static float VAL_DEFAULT_CAKE_LEVEL = 2f;
    public static float VAL_DEFAULT_CAKE_ID = 2f;
    public static float VAL_X2BOOSTER_TIME = 2.5f;
    public static float VAL_MAX_PIGGY = 2000f;

    // Animation

    // IE
    public static WaitForSeconds WAIT_SEC01 = new WaitForSeconds(0.1f);
    public static WaitForSeconds WAIT_SEC025 = new WaitForSeconds(0.25f);
    public static WaitForSeconds WAIT_SEC05 = new WaitForSeconds(0.5f);
    public static WaitForSeconds WAIT_SEC1 = new WaitForSeconds(1f);
    public static WaitForSeconds WAIT_SEC2 = new WaitForSeconds(2f);

    // UnlockLevel
    public static int skeleton_unlock_level = 5;
    public static int muscle_unlock_level = 10;
}

public enum VersionStatus
{
    Publish,
    Cheat,
    NoCheat,
    Normal
}

public enum SoundId
{
    None,
    SFX_Base,
    SFX_CoinCube,
    SFX_LevelUp,
    SFX_TapCube,
    SFX_UIButton,
    SFX_UIClick,
    SFX_Warning
}
