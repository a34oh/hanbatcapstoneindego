using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour,IPointerUpHandler
{
    public int slotNum;
    public Item item;
    public Image itemIcon;
    public void UpdateSlotUI()
    {
        itemIcon.sprite = item.itemImage;
        itemIcon.gameObject.SetActive(true);
    }
    public void RemoveSlotUI()
    {
        item = null;
        itemIcon.gameObject.SetActive(false);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        bool isUse;
        if(this.item != null)
        {
            isUse = item.Use();
            if(isUse)
            {
                itemIcon.sprite = null;
                itemIcon.gameObject.SetActive(false);
                Inventory.instance.RemoveItem(slotNum);
            }
        }
            
    }
}
