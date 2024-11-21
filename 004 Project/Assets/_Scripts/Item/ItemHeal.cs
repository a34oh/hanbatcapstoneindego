using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="ItemEft/Consumable/Health")]
public class ItemHeal : Use_Effect
{
    public int healingPoint = 0;
    public override bool ExecuteRole()
    {
        GameManager.PlayerManager.Player.GetComponentInChildren<CharacterStats<PlayerStatsData>>().CurHp = 
            Mathf.Clamp(GameManager.PlayerManager.Player.GetComponentInChildren<CharacterStats<PlayerStatsData>>().CurHp + healingPoint, 0, GameManager.PlayerManager.Player.GetComponentInChildren<CharacterStats<PlayerStatsData>>().MaxHp);
        Debug.Log("Player Hp Add" + healingPoint);

        return true;
    }
}
