using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class AggressiveWeaponHitboxToWeapon : MonoBehaviour
{
    private AggressiveWeapon weapon;
    private bool isAlreadyHit;

    private void Awake()
    {
        weapon = GetComponentInParent<AggressiveWeapon>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer).Equals(LayerMasks.Enemy) && !isAlreadyHit)
        {
            isAlreadyHit = true;
            //�굵 AggressiveWeapon�� �ϴ°� �ƴ϶�, �׳� Interface�� �޾ƿ���, ���������� collider ���� �� ������ �Լ��� �����Ų��.
            weapon.CheckAttack(collision);
        }
    }

    public void resetAlreadyHit()
    {
        isAlreadyHit = false;
    }
}

*/