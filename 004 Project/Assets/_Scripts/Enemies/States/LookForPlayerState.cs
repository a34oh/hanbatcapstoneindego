using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookForPlayerState : MonsterState
{
	private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
	private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }

	private Movement movement;
	private CollisionSenses collisionSenses;

	protected D_LookForPlayer stateData;

	protected bool turnImmediately;
	protected bool isPlayerInMinAgroRange;
	protected bool isAllTurnsDone;
	protected bool isAllTurnsTimeDone;

	protected float lastTurnTime;

	protected int amountOfTurnsDone;

	public LookForPlayerState(Entity etity, MonsterStateMachine stateMachine, string animBoolName, D_LookForPlayer stateData) : base(etity, stateMachine, animBoolName)
	{
		this.stateData = stateData;
	}

	public override void DoChecks()
	{
		base.DoChecks();

		isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
	}

	public override void Enter()
	{
		base.Enter();

		isAllTurnsDone = false;
		isAllTurnsTimeDone = false;

		lastTurnTime = startTime;
		amountOfTurnsDone = 0;

		Movement?.SetVelocityX(0f);
	}

	public override void Exit()
	{
		base.Exit();
	}

	public override void LogicUpdate()
	{
		base.LogicUpdate();

		Movement?.SetVelocityX(0f);

		if (turnImmediately)
		{
			Movement?.Flip();
			lastTurnTime = Time.time;
			amountOfTurnsDone++;
			turnImmediately = false;
		}
		else if (Time.time >= lastTurnTime + stateData.timeBetweenTurns && !isAllTurnsDone)
		{
			Movement?.Flip();
			lastTurnTime = Time.time;
			amountOfTurnsDone++;
		}

		if (amountOfTurnsDone >= stateData.amountOfTurns)
		{
			isAllTurnsDone = true;
		}

		if (Time.time >= lastTurnTime + stateData.timeBetweenTurns && isAllTurnsDone)
		{
			isAllTurnsTimeDone = true;
		}
		if (entity.stunState.stun)
			stateMachine.ChangeState(entity.stunState);
	}

	public override void PhysicsUpdate()
	{
		base.PhysicsUpdate();
	}

	public void SetTurnImmediately(bool flip)
	{
		turnImmediately = flip;
	}
}