using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    void CheckAttack(Collider2D collision);
    //skill �Ӹ� �ƴ϶� �ٸ��ʿ��� ����
}