using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FE_PlayerDetectedState : PlayerDetectedState
{
    FlyingEye enemy;
    public FE_PlayerDetectedState(Entity entity, MonsterStateMachine stateMachine, string animBoolName, D_PlayerDetectedState stateData, FlyingEye enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (performCloseRangeAction)
        {
            if (Time.time >= enemy.dodgeState.startTime + enemy.dodgeStateData.dodgeCooldown)
            {
                stateMachine.ChangeState(enemy.dodgeState);
            }
        }
        else if (performLongRangeAction)
        {
            stateMachine.ChangeState(enemy.rangedAttackState);
        }
        else if (!isPlayerInMaxAgroRange)
        {
            entity.GetPlayerRelativePosition();
            enemy.lastKnownPlayerPosition = enemy.playerTransform.position;
            stateMachine.ChangeState(enemy.lookForPlayerState);
        }

    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
