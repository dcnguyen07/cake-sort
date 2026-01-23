using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerResourseSave : SaveBase
{
    public int coins;
    public List<ItemData> ownedItem;

    public int currentLevel;
    public float currentExp;

    float expMax;
    public override void LoadData()
    {
        SetStringSave("PlayerResourseSave");
        string jsonData = GetJsonData();
        if (!string.IsNullOrEmpty(jsonData))
        {
            PlayerResourseSave data = JsonUtility.FromJson<PlayerResourseSave>(jsonData);
            ownedItem = data.ownedItem;
            coins = data.coins;
            currentLevel = data.currentLevel;
            currentExp = data.currentExp;
        }
        else
        {
            IsMarkChangeData();
            SaveData();
        }
        expMax = ProfileManager.Instance.dataConfig.levelDataConfig.GetExpToNextLevel(currentLevel);
    }
    public bool IsHasEnoughMoney(float amount)
    {
        return coins >= amount;
    }

    public void AddMoney(float amount)
    {
        coins += (int)amount;
        IsMarkChangeData();
        SaveData();
        EventManager.TriggerEvent(EventName.ChangeCoin.ToString());
    }

    public void ConsumeMoney(float amount)
    {
        coins -= (int)amount;
        IsMarkChangeData();
        SaveData();
        EventManager.TriggerEvent(EventName.ChangeCoin.ToString());
    }

    public void SaveRecord()
    {
        IsMarkChangeData();
        SaveData();
    }

    public void AddItem(ItemData item)
    {
        if(item.ItemType == ItemType.Coin)
        {
            AddMoney(item.amount);
        }
        for (int i = 0; i < ownedItem.Count; i++)
        {
            if (ownedItem[i].ItemType == item.ItemType) {
                ownedItem[i].amount += item.amount;
                EventManager.TriggerEvent(EventName.AddItem.ToString());
                IsMarkChangeData();
                SaveData();
                return;
            }
        }
        ItemData data = new ItemData();
        data.ItemType = item.ItemType;
        data.amount = item.amount;
        ownedItem.Add(data);
        IsMarkChangeData();
        SaveData();
        EventManager.TriggerEvent(EventName.AddItem.ToString());
    }

    public float GetItemAmount(ItemType itemType)
    {
        for (int i = 0; i < ownedItem.Count; i++)
        {
            if (ownedItem[i].ItemType == itemType)
            {
                return ownedItem[i].amount;
            }
        }
        return 0;
    }

    public void UsingItem(ItemType itemType)
    {
        for (int i = 0; i < ownedItem.Count; i++)
        {
            if (ownedItem[i].ItemType == itemType)
            {
                Debug.Log("using item : "+itemType);
                ownedItem[i].amount--;
                EventManager.TriggerEvent(EventName.AddItem.ToString());
                break;
            }
        }
        IsMarkChangeData();
        SaveData();
        EventManager.TriggerEvent(EventName.AddItem.ToString());
    }

    public bool AddExp(float expAdd) {
        currentExp += expAdd;
        if (currentExp >= expMax)
        {
            currentExp = 0;
            LevelUp();
            IsMarkChangeData();
            SaveData();
            return true;
        }
        IsMarkChangeData();
        SaveData();
        return false;
    }

    public void LevelUp() {
        GameManager.Instance.audioManager.PlaySoundEffect(SoundId.SFX_LevelUp);
        currentLevel++;
        GameManager.Instance.GetLevelUpReward();
        EventManager.TriggerEvent(EventName.ChangeLevel.ToString());
        expMax = ProfileManager.Instance.dataConfig.levelDataConfig.GetExpToNextLevel(currentLevel);
    }

    public string GetCurrentExp()
    {
        return currentExp + "/" + expMax;
    }
    public float GetMaxExp()
    {
        return expMax;
    }

    public bool IsHaveItem(ItemType itemType)
    {
        if (itemType == ItemType.Revive) return true;
        for (int i = 0; i < ownedItem.Count; i++)
        {
            if (ownedItem[i].ItemType == itemType && ownedItem[i].amount > 0)
                return true;
        }
        return false;
    }

    public void SetItem(ItemType itemType)
    {
        for (int i = 0; i < ownedItem.Count; i++)
        {
            if (ownedItem[i].ItemType == itemType)
            {
                ownedItem[i].amount = 10000;
                IsMarkChangeData();
                SaveData();
                return;
            }
        }
        ItemData data = new ItemData();
        data.ItemType = itemType;
        data.amount = 10000;
        ownedItem.Add(data);
        IsMarkChangeData();
        SaveData();

    }
}
