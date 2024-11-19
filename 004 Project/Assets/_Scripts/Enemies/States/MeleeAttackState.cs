using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : AttackState
{
   // AnimationToAttackCheck attackCheck;
    AnimationToPlayerDashCheck playerDashCheck;
    private AnimationToAttackCheck meleeAttackCheck;

    public MeleeAttackState(Entity entity, MonsterStateMachine stateMachine, string animBoolName, D_MeleeAttackState stateData) : base(entity, stateMachine, animBoolName)
    {
        meleeAttackCheck = entity.transform.GetComponentInChildren<AnimationToAttackCheck>();

        playerDashCheck = entity.transform.GetComponentInChildren<AnimationToPlayerDashCheck>();
        enemyStats = entity.transform.GetComponentInChildren<EnemyStats>();
        this.stateData = stateData;
      //  attackCheck.OnPlayerHit += HandleAttack;
    }


    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        meleeAttackCheck.OnPlayerHit += HandleAttack;

    }

    public override void Exit()
    {
        base.Exit();
        meleeAttackCheck.OnPlayerHit -= HandleAttack;

    }

    public override void LogicUpdate()
    {
        //�÷��̾ ���� ������ �ִ����� üũ�ϰ�, ���� trigger�� �Ͼ�� ���� ���� ���� ���� ����� ī��Ʈ ����
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();

        meleeAttackCheck.TriggerAttack();
    }
    public override void FinishAttack()
    {
        base.FinishAttack();

        meleeAttackCheck.FinishAttack();
    }

    public override void TriggerCheck()
    {
        base.TriggerCheck();

        playerDashCheck.TriggerCheck();
    }

    public override void FinishCheck()
    {
        base.FinishCheck();

        playerDashCheck.FinishCheck();
    }
    public override void HandleAttack(Collider2D collision)
    {
        base.HandleAttack(collision);
    }
}
