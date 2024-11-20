using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaterDataShow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnGUI()
    {
        GUIStyle myStyle = new GUIStyle(GUI.skin.label);
        myStyle.fontSize = 50;
        GUI.Label(new Rect(200,100,Screen.width*0.5f,Screen.height*0.25f),"PlayerType : " + GameManager.PlayerManager.DataAnalyze.playerType,myStyle);
        myStyle.fontSize = 30;
        GUI.Label(new Rect(200,150,Screen.width*0.5f,Screen.height*0.25f),"parryratio : " + GameManager.PlayerManager.DataAnalyze.parryRatio.ToString("F2"),myStyle);
        GUI.Label(new Rect(200,200,Screen.width*0.5f,Screen.height*0.25f),"runratio : " + GameManager.PlayerManager.DataAnalyze.runRatio.ToString("F2"),myStyle);
        GUI.Label(new Rect(200,250,Screen.width*0.5f,Screen.height*0.25f),"dashratio : " + GameManager.PlayerManager.DataAnalyze.dashRatio.ToString("F2"),myStyle);
    }
}
