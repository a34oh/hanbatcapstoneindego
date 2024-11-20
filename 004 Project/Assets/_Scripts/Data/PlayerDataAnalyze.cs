using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataAnalyze : MonoBehaviour
{
    public string playerType;
    public float  parryRatio;
    public float  dashRatio;
    public float  runRatio;
    public bool changePlayerType;
    private string currentPlayerType;
    void Start()
    {
        currentPlayerType = "";
        changePlayerType = false;
        // Analyze Playerdata
        // AnalyzePlayerData(GameManager.PlayerManager.PlayerDataCollect.actionData);
    }


    public void AnalyzePlayerData(Dictionary<string, int> actionData)
    {
        //  Debug.Log($"parryAttempt : { actionData["ParryAttempt"]}");
        // calculaye total Action
        int totalActions = actionData["ParryAttempt"] + actionData["DashAttempt"] + actionData["RunSuccess"];

        // Logistic
        parryRatio = LogisticFunction((float)actionData["ParryAttempt"] / totalActions);
        dashRatio = LogisticFunction((float)actionData["DashAttempt"] / totalActions);
        runRatio = LogisticFunction((float)actionData["RunSuccess"] / totalActions);

        // Ratio normalize
        float ratioSum = parryRatio + dashRatio + runRatio;
        parryRatio /= ratioSum;
        dashRatio /= ratioSum;
        runRatio /= ratioSum;

        string newPlayerType = ClassifyPlayer(parryRatio, dashRatio, runRatio);
        if (newPlayerType != currentPlayerType)
        {
            changePlayerType = true;
            currentPlayerType = newPlayerType;
        }
        else
        {
            changePlayerType = false;
        }
        playerType = newPlayerType;
        // print result
        // Debug.Log($"Parry Ratio = {parryRatio:F4}, Dodge Ratio = {dashRatio:F4}, Run Ratio = {runRatio:F4}, Play Style = {playStyle}");
    }


    float LogisticFunction(float x)
    {
        return 1f / (1f + Mathf.Exp(-x));
    }


    public string ClassifyPlayer(float parryRatio, float dashRatio, float runRatio)
    {
        
        Dictionary<string, float> ratios = new Dictionary<string, float>
        {
            { "parry", parryRatio },
            { "dash", dashRatio },
            { "run", runRatio }
        };


        string detectedType = "Balanced";
        float maxRatio = -1f;
        foreach (var entry in ratios)
        {
            if (entry.Value > maxRatio)
            {
                maxRatio = entry.Value;
                detectedType = entry.Key;

            }
        }
         if (maxRatio > 0.5f) // High threshold
        {
            return $"High_{detectedType}";
        }
        else if (maxRatio > 0.4f) // Medium threshold
        {
            return detectedType;
        }
        else
        {
            return "Balanced";
        }
    }
}