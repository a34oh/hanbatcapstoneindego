using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : MonsterState
{
    protected Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }

    private Movement movement;
    private CollisionSenses collisionSenses;
    protected CharacterStats<EnemyStatsData> enemyStats;
    protected D_MoveState stateData;

    protected bool isDetectingWall;
    protected bool isDetectingLedge;
    protected bool isPlayerInMinAgroRange;
    protected bool isPlayerInMaxAgroRange;

    public MoveState(Entity entity, MonsterStateMachine stateMachine, string animBoolName, D_MoveState stateData) : base(entity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
        enemyStats = entity.transform.GetComponentInChildren<EnemyStats>();

    }
    public override void DoChecks()
    {
        base.DoChecks();

        isDetectingWall = CollisionSenses.WallFront;
        isDetectingLedge = CollisionSenses.LedgeVertical;
        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
        isPlayerInMaxAgroRange = entity.CheckPlayerInMaxAgroRange();
        
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

        Movement?.SetVelocityX(stateData.movementSpeed * Movement.FacingDirection * enemyStats.MoveSpeed);

        if (entity.stunState.stun)
            stateMachine.ChangeState(entity.stunState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
