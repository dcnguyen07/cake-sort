using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public Transform groupCakeParents;
    public List<GroupCake> groupCakes = new List<GroupCake>();
    public GroupCake groupCakePref;

    public GroupCake GetGroupCake() {
        for (int i = groupCakes.Count - 1; i >= 0; i--)
        {
            if (groupCakes[i] == null)
            {
                groupCakes.RemoveAt(i);
                continue;
            }
            if (!groupCakes[i].gameObject.activeSelf) return groupCakes[i];
        }
        GroupCake newGroupCake = Instantiate(groupCakePref, groupCakeParents);
        groupCakes.Add(newGroupCake);
        return newGroupCake;
    }

    public void CheckGroupCake() {
        for (int i = groupCakes.Count - 1; i >= 0; i--) {
            if (groupCakes[i] == null)
            {
                groupCakes.RemoveAt(i);
                continue;
            }
            groupCakes[i].CheckGroupDone();
        }
    }
}
