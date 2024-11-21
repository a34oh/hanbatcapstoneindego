using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : Entity
{   
    public E2_MoveState moveState { get; private set; }
    public E2_IdleState idleState { get; private set; }
    public E2_PlayerDetectedState playerDetectedState { get; private set; }
    public E2_MeleeAttackState meleeAttackState { get; private set; }
    public E2_LookForPlayerState lookForPlayerState { get; private set; }
    public E2_DodgeState dodgeState { get; private set; }
    public E2_RangedAttackState rangedAttackState { get; private set; }

    [SerializeField]
    private D_MoveState moveStateData;
    [SerializeField]
    private D_IdleState idleStateData;
    [SerializeField]
    private D_PlayerDetectedState playerDetectedStateData;
    [SerializeField]
    private D_MeleeAttackState meleeAttackStateData;
    [SerializeField]
    private D_LookForPlayer lookForPlayerStateData;
    [SerializeField]
    private D_StunState stunStateData;

    [SerializeField]
    public D_DodgeState dodgeStateData;
    [SerializeField]
    private D_RangeAttackState rangeAttackStateData;

    public Vector3? lastKnownPlayerPosition; // 플레이어의 마지막 위치 저장

    //  [SerializeField]
    //  private Transform meleeAttackPosition;
    [SerializeField]
    private Transform rangedAttackPosition;
    public override void Awake()
    {
        base.Awake();

        moveState = new E2_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new E2_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new E2_PlayerDetectedState(this, stateMachine, "playerDetected", playerDetectedStateData, this);

        lookForPlayerState = new E2_LookForPlayerState(this, stateMachine, "lookForPlayer", lookForPlayerStateData, this);
        stunState = new E2_StunState(this, stateMachine, "stun", stunStateData, this);
        dodgeState = new E2_DodgeState(this, stateMachine, "dodge", dodgeStateData, this);
        rangedAttackState = new E2_RangedAttackState(this, stateMachine, "rangedAttack", rangedAttackPosition, rangeAttackStateData, this);
        meleeAttackState = new E2_MeleeAttackState(this, stateMachine, "meleeAttack", meleeAttackStateData, this);

        maxParryStunStack = 5;
    }

    private void Start()
    {
        stateMachine.Initialize(moveState);
    }
    protected override void Update()
    {
        base.Update();
        if (PlayerDataAnalyze.Instance.changePlayerType)
        {
            StartCoroutine(GetComponentInChildren<EnemyStats>().AdjustStatsBasedOnPlayerType());
            PlayerDataAnalyze.Instance.changePlayerType = false;
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        stateMachine.Initialize(idleState);
    }

    public void ClearLastKnownPlayerPosition()
    {
        lastKnownPlayerPosition = null; // 위치 초기화
    }

    public bool IsAtLastKnownPlayerPosition()
    {
        if (lastKnownPlayerPosition == null) return true;

        // 마지막 위치와 현재 위치 간의 거리 계산
        return Mathf.Abs(transform.position.x - lastKnownPlayerPosition.Value.x) < 0.15f;
    }
}
