using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PlayerStatsData : CharacterStatsData
{
    public int level;
    public int MaxExp;
    // �߰����� �ʵ尡 �ִٸ� ���⿡ ����
}

[Serializable]
public class PlayerStatsDataJson : ILoader<int, PlayerStatsData>
{
    public List<PlayerStatsEntry> playerStats;

    [Serializable]
    public class PlayerStatsEntry
    {
        public int key;
        public PlayerStatsData value;
    }

    public Dictionary<int, PlayerStatsData> MakeDict()
    {
        Dictionary<int, PlayerStatsData> dict = new Dictionary<int, PlayerStatsData>();
        foreach (PlayerStatsEntry entry in playerStats)
        {
            dict.Add(entry.key, entry.value);
        }
        return dict;
    }
}
