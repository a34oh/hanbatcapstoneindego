using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//�޴����� ���� �� �˹�� ���õ� ������ ���� ���� ����.
public static class ShieldStateName
{
    public const string Enter = "Enter";
    public const string Exit = "Exit";
    public const string Hold = "Hold";
    public const string Parry = "Parry";
}
public class DefensiveWeapon : Weapon
{
    [SerializeField] protected Shield_WeaponData weaponData;

    ShieldStateMachine stateMachine;

    public ShieldEnterState ShieldEnterState { get; private set; }
    public ShieldExitState ShieldExitState { get; private set; }
    public ShieldHoldState ShieldHoldState { get; private set; }
    public ShieldParryState ShieldParryState { get; private set; }
    public bool isDefending;


    private void Awake()
    {
        stateMachine = new ShieldStateMachine();
    }
    protected override void Start()
    {
        base.Start();
        //Start���� �ʱ�ȭ �ϰų� Player���� �ʱ�ȭ
        ShieldEnterState = new ShieldEnterState(this, shieldState, stateMachine, weaponData, ShieldStateName.Enter);
        ShieldExitState = new ShieldExitState(this, shieldState,  stateMachine, weaponData, ShieldStateName.Exit);
        ShieldHoldState = new ShieldHoldState(this, shieldState, stateMachine, weaponData, ShieldStateName.Hold);
        ShieldParryState = new ShieldParryState(this, shieldState, stateMachine, weaponData, ShieldStateName.Parry, playerStats);

        weaponAnimationToWeapon.OnFinish += AnimationFinishTrigger;

        isDefending = false;
    }

    private void Update()
    {
        stateMachine.CurrentState.LogicUpdate();
    }
    public override void EnterWeapon()
    {
        base.EnterWeapon();
        stateMachine.Initialize(ShieldEnterState);
        isDefending = true;


    }

    public override void ExitWeapon()
    {
        base.ExitWeapon();
    }
 

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        stateMachine.CurrentState.AnimationFinishTrigger();
    }
   

    public void CheckShield(GameObject go, float attackDamage, Vector2 knockbackAngle, float knockbackStrength, int facingDirection)
    {
        isDefending = true;
        // ���� ���� Ȯ��
        if (stateMachine.CurrentState is ShieldEnterState)
            stateMachine.ChangeState(ShieldParryState);

        stateMachine.CurrentState.HandleShieldCollision(go, attackDamage, knockbackAngle, knockbackStrength, facingDirection);
       // ShieldWeaponHitboxToWeapon.ResetAlreadyHit();
    }

    public override void HandleCollision(Collider2D collision)
    {
        isDefending = true;
    }
}

public class ShieldStateMachine
{
    public ShieldState CurrentState { get; private set; }
    public void Initialize(ShieldState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(ShieldState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }


}

public class ShieldState
{
    protected DefensiveWeapon weapon;
    protected ShieldStateMachine stateMachine;
    protected Shield_WeaponData weaponData;
    protected PlayerShieldState playerShieldState;
    protected string animBoolName;
    protected bool isExitingState;
    public ShieldState(DefensiveWeapon weapon, PlayerShieldState playerShieldState, ShieldStateMachine stateMachine, Shield_WeaponData weaponData, string animBoolName)
    {
        this.weapon = weapon;
        this.playerShieldState = playerShieldState;
        this.stateMachine = stateMachine;
        this.weaponData = weaponData;
        this.animBoolName = animBoolName;
    }
    public virtual void Enter()
    {
        weapon.baseAnimator.SetBool(animBoolName, true);
        weapon.weaponAnimator.SetBool(animBoolName, true);
        isExitingState = false;
        //Debug.Log(this.ToString() + "���� ����");
    }
    public virtual void Exit()
    {
        weapon.weaponAnimator.SetBool(animBoolName, false);
        weapon.baseAnimator.SetBool(animBoolName, false);
        isExitingState = true;
    }

    public virtual void LogicUpdate()
    {

    }


    public virtual void AnimationFinishTrigger()
    {

    }

    public virtual void HandleShieldCollision(GameObject collision, float attackDamage, Vector2 knockbackAngle, float knockbackStrength, int facingDirection) { }

}
public class ShieldEnterState : ShieldState
{
    private bool isDone;

    public ShieldEnterState(DefensiveWeapon weapon, PlayerShieldState playerShieldState, ShieldStateMachine stateMachine, Shield_WeaponData weaponData, string animBoolName) : base(weapon, playerShieldState, stateMachine, weaponData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        isDone = false;
        playerShieldState.Movement?.SetVelocityZero();

        GameManager.PlayerManager.PlayerDataCollect.RecordAction(PlayerDataCollectName.ParryAttempt);
        GameManager.SharedCombatDataManager.IsPlayerNotEnterHitState = true;

    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            if (isDone)
            {
                //if (�и��Ѵٸ�) 
                //    stateMachine.ChangeState(weapon.ShieldParryState);
                if (playerShieldState.ShieldHoldInput)
                    stateMachine.ChangeState(weapon.ShieldHoldState);
                else if (playerShieldState.ShieldInput == false)
                    stateMachine.ChangeState(weapon.ShieldExitState);

            }
        }
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        isDone = true;
    }

}

public class ShieldExitState : ShieldState
{
    private bool isDone;
    public ShieldExitState(DefensiveWeapon weapon, PlayerShieldState playerShieldState, ShieldStateMachine stateMachine, Shield_WeaponData weaponData, string animBoolName) : base(weapon, playerShieldState,stateMachine, weaponData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        playerShieldState.Movement?.SetVelocityX(0);
        weapon.isDefending = false;
    }
    public override void Exit()
    {
        base.Exit();
        GameManager.SharedCombatDataManager.IsPlayerNotEnterHitState = false;
        playerShieldState.AnimationFinishTrigger();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!isExitingState)
        {
            playerShieldState.Movement?.SetVelocityX(0);
        }

    }
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        Exit();    
    }

}

public class ShieldHoldState : ShieldState
{
    public ShieldHoldState(DefensiveWeapon weapon, PlayerShieldState playerShieldState, ShieldStateMachine stateMachine, Shield_WeaponData weaponData, string animBoolName) : base(weapon, playerShieldState, stateMachine, weaponData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GameManager.PlayerManager.PlayerDataCollect.RecordAction(PlayerDataCollectName.DefenceAttempt);
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            playerShieldState.Movement?.SetVelocityX(0);
            if (playerShieldState.ShieldHoldInput == false)
            {
                stateMachine.ChangeState(weapon.ShieldExitState);
            }
        }

    }

    public override void HandleShieldCollision(GameObject go, float attackDamage, Vector2 knockbackAngle, float knockbackStrength, int facingDirection)
    {
        // Ȧ�� ������ ��

        // �÷��̾� ������
        IDamageable playerDamageable = weapon.transform.root.GetComponentInChildren<IDamageable>();
        if (playerDamageable != null)
        {
         //   Debug.Log("Ȧ�� ���� ����");
            playerDamageable.DamageWithShield(attackDamage, go.transform); // ���� ����
        }

        // �÷��̾� �˹�
        IKnockbackable playerKnockbackable = weapon.transform.root.GetComponentInChildren<IKnockbackable>();
        if (playerKnockbackable != null)
        {
            playerKnockbackable.KnockbackWithShield(knockbackAngle, knockbackStrength, facingDirection); // �˹� ����
        }

        GameManager.PlayerManager.PlayerDataCollect.RecordAction(PlayerDataCollectName.DefenceSuccess);
    }
}

public class ShieldParryState : ShieldState
{
    private bool isDone;
    public ShieldParryState(DefensiveWeapon weapon, PlayerShieldState playerShieldState, ShieldStateMachine stateMachine, Shield_WeaponData weaponData, string animBoolName, CharacterStats<PlayerStatsData> stats) : base(weapon, playerShieldState, stateMachine, weaponData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        isDone = false;
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isDone)
        {
            if (!isExitingState)
            {
                stateMachine.ChangeState(weapon.ShieldExitState);
            }
        }
    }

    public override void HandleShieldCollision(GameObject go, float attackDamage, Vector2 knockbackAngle, float knockbackStrength, int facingDirection)
    {
        base.HandleShieldCollision(go, attackDamage, knockbackAngle, knockbackStrength, facingDirection);
        
        //�и� ���� �� ���� ����
        Entity enemy = go.GetComponent<Entity>();

        enemy.AddcurrentParryStunStack(weaponData.parryStun);
  
        // �и� ������ ��
        // ������ ���� �ֱ� ���� �߰�
        IDamageable damageable = go.GetComponentInChildren<IDamageable>();
        if (damageable != null)
        {
            //Debug.Log("�и�");
            damageable.NonElementDamage(weaponData.parryDamage, go.transform);
        }
        //������ �˹�
        IKnockbackable knockbackable = go.GetComponentInChildren<IKnockbackable>();
        if (knockbackable != null && enemy.IsKnockbackable)
        {
            knockbackable.Knockback(weaponData.knockbackAngle, weaponData.knockbackStrength, playerShieldState.Movement.FacingDirection);
        }
        GameManager.PlayerManager.PlayerDataCollect.RecordAction(PlayerDataCollectName.ParrySuccess);
        playerShieldState.OnParrySuccess();

    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        isDone = true;

    }
}
