using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Ctrl : MonoBehaviour
{
    public int start,end = 0;
    public bool isTrigger;


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && isTrigger == false)
        {
            isTrigger = true;
            for (int i = start;i<end;i++)
            {
                GameObject obj = ObjectPool.instance.monster[i];
                if (!obj.GetComponentInChildren<ICharacterStats>().isDead)
                    obj.SetActive(true);
                
                if (obj.activeSelf)
                {
                  //  obj.GetComponent<BoxCollider2D>().isTrigger = true;
                    // ������ �Ӽ� ����
                    EnemyStats enemyStats = obj.GetComponentInChildren<EnemyStats>();

                    if (enemyStats != null)
                    {
                        var playerStats = other.GetComponentInChildren<PlayerStats>();
                        var (monsterElement, elementLevel) = ElementRelations.GenerateMonsterElementAndLevel(playerStats.Element, playerStats.Level);

                        // ���� ���� �� �Ӽ��� �Ӽ� ���� ����
                        enemyStats.ChangeElement(monsterElement, elementLevel);

                    }
                }
            }
        }
    }
}
