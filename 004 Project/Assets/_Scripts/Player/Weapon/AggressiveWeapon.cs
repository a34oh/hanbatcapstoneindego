using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveWeapon : Weapon, IWeapon
{
    [SerializeField] protected Sword_WeaponData weaponData;

    private bool resetCounter;
    private Coroutine checkAttackReInputCor;

    protected int CurrentAttackCounter { get => currentAttackCounter; set => currentAttackCounter = value >= weaponData.numberOfAttacks ? 0 : value; }
    private int currentAttackCounter;
    private bool setAttackSpeed = false;

    protected CoroutineHandler coroutineHandler;

    protected override void Start()
    {
        base.Start();

        //SetWeaponData(so_weapondata);
        coroutineHandler = GetComponentInParent<CoroutineHandler>();


        weaponAnimationToWeapon.OnAction += AnimationActionTrigger;
        weaponAnimationToWeapon.OnFinish += AnimationFinishTrigger;
        weaponAnimationToWeapon.OnStartMovement += AnimationStartMovementTrigger;
        weaponAnimationToWeapon.OnStopMovement += AnimationStopMovementTrigger;
        weaponAnimationToWeapon.OnTurnOnFlip += AnimationTurnOnFlipTrigger;
        weaponAnimationToWeapon.OnTurnOffFlip += AnimationTurnOffFlipTrigger;
    }//+=������ -=�� �������.

    public override void EnterWeapon()
    {
        base.EnterWeapon();

        baseAnimator.SetInteger("Counter", CurrentAttackCounter);
        weaponAnimator.SetInteger("Counter", CurrentAttackCounter);

        resetCounter = false;

        GameManager.PlayerManager.PlayerDataCollect.RecordAction(PlayerDataCollectName.AttackAttempt);

        CheckAttackReInput(weaponData.reInputTime);
    }

    private void CheckAttackReInput(float reInputTime)
    {
        if (checkAttackReInputCor != null)
            coroutineHandler.StopCoroutine(checkAttackReInputCor);

        // ���ݼӵ��� 1.0���� ũ�� reInputTime�� �����ϰ�, 1.0���� ������ ����
        // ������ ��ȭ ���� �����ؼ� �ʹ� �ް��� ��ȭ�� ������
        float minAttackSpeed = 0.2f; // ���� �ӵ��� �ʹ� �������� �ʵ��� ����
        float maxAttackSpeed = 3.0f; // ���� �ӵ��� �ʹ� �������� �ʵ��� ����

        // playerStats.AttackSpeed ���� Ŭ����(�ּҿ� �ִ밪 ���̷� ����)
        float clampedAttackSpeed = Mathf.Clamp(playerStats.AttackSpeed, minAttackSpeed, maxAttackSpeed);

        // ���ο� reInputTime ���: ���� �ӵ��� �������� �ð��� �پ���, �������� ����
        float adjustedReInputTime = reInputTime * (1 + (1 - clampedAttackSpeed) + 0.15f);

        // �ڷ�ƾ ����
        checkAttackReInputCor = coroutineHandler.StartManagedCoroutine(CheckAttackReInputCoroutine(adjustedReInputTime), ResetAttackCounter);
    }


    private IEnumerator CheckAttackReInputCoroutine(float reInputTime)
    {
        float currentTime = 0f;
        while (currentTime < reInputTime)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
    }

    private void ResetAttackCounter()
    {
     //   Debug.Log("ResetAttackCounter");
        resetCounter = true;
        CurrentAttackCounter = 0;

    }

    public override void ExitWeapon()
    {
        base.ExitWeapon();
        if (!resetCounter)
        {
            CurrentAttackCounter++;
        }
    }

    public override void AnimationActionTrigger()
    {
        base.AnimationActionTrigger();
        attackState.Movement?.SetVelocityZero();
      //  aggressiveWeaponHitboxToWeapon.resetAlreadyHit();

    }
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        attackState.AnimationFinishTrigger();
    }
    public void AnimationStartMovementTrigger()
    {
        attackState.Movement?.SetVelocityX(weaponData.movementSpeed[CurrentAttackCounter] * attackState.Movement.FacingDirection);
    }
    public void AnimationStopMovementTrigger()
    {
        attackState.Movement?.SetVelocityX(0);
    }

    public void AnimationTurnOnFlipTrigger()
    {
        attackState.SetFilpCheck(true);

        //movement���� ���������ʿ�..
    }
    public void AnimationTurnOffFlipTrigger()
    {
        attackState.SetFilpCheck(false);
    }

    public override void HandleCollision(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponentInChildren<IDamageable>();

        if (damageable != null)
        {
            float baseDamage = weaponData.attackDamage[CurrentAttackCounter] * playerStats.AttackDamage;
            Element attackerElement = playerStats.Element; // �÷��̾��� �Ӽ�
            float attackerAttackStat = playerStats.AttackDamage;
            damageable.Damage(baseDamage, attackerElement, attackerAttackStat, gameObject, collision.transform); // �⺻ �������� �������� �Ӽ� �� ���ݷ� ���� ����
            Debug.Log("������ : " + weaponData.attackDamage[CurrentAttackCounter] * playerStats.AttackDamage);
            //detectedDamageable.Add(damageable);
            GameManager.PlayerManager.PlayerDataCollect.RecordAction(PlayerDataCollectName.AttackSuccess);
        }
        IKnockbackable knockbackable = collision.GetComponentInChildren<IKnockbackable>();

        if (knockbackable != null && collision.GetComponent<Entity>().IsKnockbackable)
        {
            knockbackable.Knockback(weaponData.knockbackAngle, weaponData.knockbackStrength, attackState.Movement.FacingDirection); // ���� ü�޿� ���� �˹� ����
        }
    }

    private void OnEnable()
    {
        if (setAttackSpeed)
        {
            baseAnimator.SetFloat("AttackSpeed", playerStats.AttackSpeed);
            weaponAnimator.SetFloat("AttackSpeed", playerStats.AttackSpeed);
        }
        setAttackSpeed = true;
    }
}
