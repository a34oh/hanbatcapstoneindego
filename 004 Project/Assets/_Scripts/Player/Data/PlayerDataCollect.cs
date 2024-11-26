using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class PlayerDataCollect
{
    public Dictionary<string, int> actionData = new Dictionary<string, int>()
    {
        { PlayerDataCollectName.ParryAttempt, 0 },
        { PlayerDataCollectName.ParrySuccess, 0 },
        { PlayerDataCollectName.AttackAttempt, 0 },
        { PlayerDataCollectName.AttackSuccess, 0 },
        { PlayerDataCollectName.DashAttempt, 0 },
        { PlayerDataCollectName.DashSuccess, 0 },
        { PlayerDataCollectName.DashFailure, 0 },
        { PlayerDataCollectName.RunAttempt, 0 },
        { PlayerDataCollectName.RunSuccess, 0 },

        { PlayerDataCollectName.DefenceAttempt, 0 },
        { PlayerDataCollectName.DefenceSuccess, 0 },
    };

    public void RecordAction(string actionType)
    {
        if (actionData.ContainsKey(actionType))
        {
            actionData[actionType]++;
            PlayerDataAnalyze.Instance.AnalyzePlayerData(actionData);

           Debug.Log($"{actionType} count : {actionData[actionType]}");
        }
    }

    public int GetActionData(string actionType)
    {
        if (actionData.TryGetValue(actionType, out int value))
        {
            return value;
        }
        else
        {
            Debug.LogError($"{actionType} is not in actionData. ");
            return 0;
        }
    }

    public IReadOnlyDictionary<string, int> GetAllData()
    {
        return new ReadOnlyDictionary<string, int>(actionData);
    }


}
