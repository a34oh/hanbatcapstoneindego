using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_MoveState : MoveState
{
    private Enemy2 enemy;

    public E2_MoveState(Entity etity, MonsterStateMachine stateMachine, string animBoolName, D_MoveState stateData, Enemy2 enemy) : base(etity, stateMachine, animBoolName, stateData)
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

        if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        else if (enemy.lastKnownPlayerPosition != null)
        {
            float direction = Mathf.Sign(enemy.lastKnownPlayerPosition.Value.x - enemy.transform.position.x);
            Movement?.SetVelocityX(stateData.movementSpeed * direction * enemyStats.MoveSpeed);

            // 마지막 위치에 도달했을 경우
            if (enemy.IsAtLastKnownPlayerPosition())
            {
                enemy.ClearLastKnownPlayerPosition();
                stateMachine.ChangeState(enemy.lookForPlayerState);
            }
        }
        else if(isDetectingWall || !isDetectingLedge)
        {
            enemy.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
