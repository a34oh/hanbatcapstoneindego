using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Equipment,
    Consumable,
    Etc
}

[System.Serializable]
public class Item
{
    public ItemType Type;
    public string itemName;
    public Sprite itemImage;
    public List<Use_Effect> effects;
    public bool Use()
    {
        bool isused = false;
        foreach(Use_Effect effect in  effects)
        {
            isused = effect.ExecuteRole();
        }
        return isused;
    }
}
