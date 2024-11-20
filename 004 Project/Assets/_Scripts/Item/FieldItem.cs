using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    public Item item;
    public SpriteRenderer image;
    public void SetItem(Item _item)
    {
        item.itemName = _item.itemName;
        item.itemImage = _item.itemImage;
        item.Type = _item.Type;
        item.effects = _item.effects;

        image.sprite = item.itemImage;
    }
    public Item GetItem()
    {
        return item;
    }
    public void DestroyItem()
    {
        Destroy(this.gameObject);
    }
    public bool AddItem(Item _item)
    {
        if(Inventory.instance.Inventory_count < 16)
        {
            Inventory.instance.items.Add(_item);
            Inventory.instance.Inventory_count++;
            InventoryUI.instance.Check_Inventory();
            return true;
        }
        return false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(AddItem(this.GetItem()))
            {
                this.DestroyItem();
            }
        }
    }
}
