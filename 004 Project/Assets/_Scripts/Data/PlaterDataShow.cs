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
        GUI.Label(new Rect(400,100,Screen.width*0.5f,Screen.height*0.25f),"PlayerType : " + PlayerDataAnalyze.Instance.playerType,myStyle);
        myStyle.fontSize = 30;
        GUI.Label(new Rect(400,150,Screen.width*0.5f,Screen.height*0.25f),"parryratio : " + PlayerDataAnalyze.Instance.parryRatio.ToString("F2"),myStyle);
        GUI.Label(new Rect(400,200,Screen.width*0.5f,Screen.height*0.25f),"runratio : " + PlayerDataAnalyze.Instance.runRatio.ToString("F2"),myStyle);
        GUI.Label(new Rect(400,250,Screen.width*0.5f,Screen.height*0.25f),"dashratio : " + PlayerDataAnalyze.Instance.dashRatio.ToString("F2"),myStyle);
    }
}
