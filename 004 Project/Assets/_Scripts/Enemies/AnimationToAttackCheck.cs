using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationToAttackCheck : MonoBehaviour
{
    public bool isAlreadyHit { get; private set; }

    public event Action<Collider2D> OnPlayerHit;

    private bool isPlayerInvincible = false;
    public float collisionCooldown = 0.25f; // �ߺ� �浹�� ������ �ð�

    protected CoroutineHandler coroutineHandler;
    private Coroutine checkInvincibilityCooldown;

    private void Start()
    {
        coroutineHandler = GetComponentInParent<CoroutineHandler>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾ �̹� ���� ���¶�� ���� ����
        if (isPlayerInvincible)
            return;

        // ���Ͱ� �÷��̾ �����ϴ� ���
        if ((1 << collision.gameObject.layer).Equals(LayerMasks.Player) && !isAlreadyHit)
        {
            isAlreadyHit = true;
            OnPlayerHit?.Invoke(collision); // �÷��̾ ���ݴ����� �� ȣ��Ǵ� �̺�Ʈ
            checkInvincibilityCooldown = coroutineHandler.StartManagedCoroutine(InvincibilityCooldown());
        }
    }
    // �÷��̾ ������ ���� �� ���� �ð� ���� ���°� �Ǵ� ����
    private IEnumerator InvincibilityCooldown()
    {
        isPlayerInvincible = true;
        yield return new WaitForSeconds(collisionCooldown);
        isPlayerInvincible = false;
    }

    public void TriggerAttack()
    {
        isAlreadyHit = false;

        if (checkInvincibilityCooldown != null)
            coroutineHandler.StopCoroutine(checkInvincibilityCooldown);
    }
    public void FinishAttack()
    {
        bool isPlayerDashing = GameManager.SharedCombatDataManager.IsPlayerDashing;

        //isAlreadyHit�� false�̰�, isWithinAttackRange�� true�� �� �÷��̾ ��ø� ����߾��ٸ�

        if (isPlayerDashing)
        {
            if (!isAlreadyHit)
            {
                GameManager.PlayerManager.PlayerDataCollect.RecordAction(PlayerDataCollectName.DashSuccess);
            }
            else
            {
                GameManager.PlayerManager.PlayerDataCollect.RecordAction(PlayerDataCollectName.DashFailure);
            }
            GameManager.SharedCombatDataManager.SetPlayerDashing(false);
        }
        else
        {
            if (!isAlreadyHit)
            {
                GameManager.PlayerManager.PlayerDataCollect.RecordAction(PlayerDataCollectName.RunSuccess);
            }
        }

    }
}
