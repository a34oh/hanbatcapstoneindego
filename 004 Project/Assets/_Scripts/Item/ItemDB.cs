using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB : MonoBehaviour
{
    public static ItemDB instance;
    public GameObject fieldItemPrefab;
    public List<Item> itemDB = new List<Item>();
    void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        
    }
    public void Generate_Item(Vector3 pos)
    {
        GameObject go  = Instantiate(fieldItemPrefab,pos,Quaternion.identity);
        go.GetComponent<FieldItem>().SetItem(itemDB[Random.Range(0,itemDB.Count)]);
    }
}
