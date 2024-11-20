using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_MeleeAttackState : MeleeAttackState
{
    private Enemy2 enemy;

    private static int MaxAttackCount = 2;
    public static int CurrentAttackCount = 0;

    private Combat Combat { get => combat ?? core.GetCoreComponent(ref combat); }

    private Combat combat;
    public E2_MeleeAttackState(Entity etity, MonsterStateMachine stateMachine, string animBoolName, D_MeleeAttackState stateData, Enemy2 enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        CurrentAttackCount++;
        base.Enter();
        if (!Movement.CanSetVelocity)
            Combat.ResetKnockback();
        entity.IsKnockbackable = false;
    }

    public override void Exit()
    {
        base.Exit();
        entity.IsKnockbackable = true;
    }

    public override void FinishAttack()
    {
        base.FinishAttack();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            if (CurrentAttackCount == MaxAttackCount)
            {
                stateMachine.ChangeState(enemy.dodgeState);
                CurrentAttackCount = 0;
            }
            else if (isPlayerInMinAgroRange)
            {
                stateMachine.ChangeState(enemy.playerDetectedState);
            }
            else if (!isPlayerInMinAgroRange)
            {
                stateMachine.ChangeState(enemy.lookForPlayerState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
    }

   /* public override void HandleAttack(Collider2D collision)
    {
        if (stateData == null)
            stateData = new D_AttackState();
        this.collision = collision;

        if (collision != null)
        {

            IDamageable damageable = collision.GetComponentInChildren<IDamageable>();

            DefensiveWeapon defensiveWeapon = collision.GetComponentInChildren<DefensiveWeapon>();
            //패링 성공 시 패링 키 홀드하고 있으면 어떻게 할지 고민하기.
            if (defensiveWeapon != null && defensiveWeapon.isDefending && IsShieldBlockingAttack(collision.transform, defensiveWeapon.transform, defensiveWeapon.GetPlayerShieldState().Movement.FacingDirection))
            {
                // Debug.Log("방패로 막음!");
                defensiveWeapon.isDefending = false;
                defensiveWeapon.CheckShield(entity.gameObject, enemyStats.AttackDamage, stateData.knockbackAngle, stateData.knockbackStrength, Movement.FacingDirection);
                return;
            }
            else
            {
                Debug.Log("GameManager.SharedCombatDataManager.IsPlayerNotEnterHitState : " + GameManager.SharedCombatDataManager.IsPlayerNotEnterHitState);
                if (!GameManager.SharedCombatDataManager.IsPlayerNotEnterHitState)
                    GameManager.SharedCombatDataManager.SetPlayerHit(true);

                if (damageable != null)
                {
                    damageable.NonElementDamage(enemyStats.AttackDamage, collision.transform);
                }

            }
            //SharedCombatDataManager.Instance.SetPlayerHit(true);

        }

    }
   */
}
