public enum ItemType
{
    Cake = -1,
    None = 0,
    Trophy = 1,
    Coin = 2,
    Swap = 3,
    Hammer = 4,
    ReRoll = 5,
    Bomb = 6,
    FillUp = 7,
    Revive = 8
}

[System.Serializable]
public class ItemData
{
    public ItemType ItemType;
    public int subId;
    public float amount;
}
