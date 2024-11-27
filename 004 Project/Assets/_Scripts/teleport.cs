using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class teleport : MonoBehaviour
{
    public GameObject player;
    public GameObject portal;

    void Start()
    {
    }
    public void tele()
    {
        
        player = GameObject.Find("Player");
        portal = GameObject.Find("Portal(Clone)");
        
        if(player ==null || portal==null)
        {
            Debug.Log(1);
        }
        player.transform.position = portal.transform.position;
        EventSystem.current.SetSelectedGameObject(null);
    }

}
