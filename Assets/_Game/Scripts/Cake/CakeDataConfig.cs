using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CakeDataConfig", menuName = "ScriptableObject/CakeDataConfig")]
public class CakeDataConfig : ScriptableObject
{
    public List<CakeData> cakeDatas;
    public List<CakeObjectByLevel> cakeObjectByLevels;
    
    public Mesh GetCakePieceMesh(int id, int level = 1)
    {
        for (int i = 0; i < cakeDatas.Count; i++)
        {
            if (cakeDatas[i].id == id)
            { return cakeDatas[i].pieces[level - 1]; }
        }
        return null;
    }
    
    public Material GetCakePieceMaterial(int id)
    {
        for (int i = 0; i < cakeDatas.Count; i++)
        {
            if (cakeDatas[i].id == id)
            { return cakeDatas[i].pieceMaterials[0]; }
        }
        return null;
    }

    public int GetRandomCake()
    {
        return cakeDatas[Random.Range(0, cakeDatas.Count)].id;
    }

    public GameObject GetCakePref(int cakeId)
    {
        int level = ProfileManager.Instance.playerData.cakeSaveData.GetOwnedCakeLevel(cakeId);
        if(level > 2) level = 2;
        for (int i = 0; i < cakeObjectByLevels.Count; i++)
        {
            if (cakeObjectByLevels[i].id == cakeId) 
                return cakeObjectByLevels[i].GetCakePref(level);
        }
        return null;
    }
}

[System.Serializable]
public class CakeData
{
    public int id;
    public List<Mesh> pieces;
    public List<Material> pieceMaterials;
}

[System.Serializable]
public class CakeObjectByLevel
{
    public int id;
    public List<GameObject> cakePref;
    public GameObject GetCakePref(int level)
    {
        return cakePref[level - 1];
    }
}
