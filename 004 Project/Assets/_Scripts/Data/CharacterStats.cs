using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // UI 관련 네임스페이스 추가
using System;

public abstract class CharacterStats<T> : MonoBehaviour, ICharacterStats where T : CharacterStatsData
{
    public event Action OnHealthZero;

    [SerializeField] protected int id;
    [SerializeField] protected int curHp;
    [SerializeField] protected int maxHp;
    [SerializeField] protected float attackDamage;
    [SerializeField] protected float basedamage;
    [SerializeField] protected float addAttackDamage;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float defense;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected Element element;
    protected Animator animator;

    public int Id { get => id; set => id = value; }

    public int CurHp { get => curHp; set => curHp = value; }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public float AttackDamage { get => attackDamage; }
    public float AddAttackDamage { get => addAttackDamage; set { addAttackDamage = value; ChangeDamage(); } }
    public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
    public float Defense { get => defense; set => defense = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }


    // 속성 레벨
    protected int fireLevel = 1;
    protected int iceLevel = 1;
    protected int landLevel = 1;
    protected int lightLevel = 1;
    protected float baseAttackSpeed;
    protected float baseMoveSpeed;

    protected float slowEffectMultiplier = 1f;
    protected float attackSpeedModifier = 1f;
    protected float moveSpeedModifier = 1f;
    protected float attackSpeedSlowMultiplier = 1f;
    protected float moveSpeedSlowMultiplier = 1f;
    protected float adjustStatsAttackSpeed = 1f;
    protected float adjustStatsMoveSpeed = 1f;
    protected float passiveMultiplier = 1f;

    protected float effectAttackSpeed;
    protected float effectMoveSpeed;
    protected bool OnsetStats;
    public float TotalMoveSpeedMultiplier => moveSpeedModifier * moveSpeedSlowMultiplier;
    public bool isDead { get; set; }

    public Element Element { get => element; set => element = value; }

    private ElementalComponent elementalComponent;



    protected virtual void Awake()
    {
        Element = Element.None;

        animator = transform.root.GetComponent<Animator>();
        elementalComponent = transform.parent.GetComponentInChildren<ElementalComponent>();

        OnsetStats = false;


    }

    protected abstract void SetStat();

    protected virtual void SetStatsData(T stats)
    {
        curHp = stats.curHp;
        maxHp = stats.maxHp;
        attackDamage = stats.attackDamage + addAttackDamage;
        attackSpeed = stats.attackSpeed;
        defense = stats.defense;
        moveSpeed = stats.moveSpeed;

        basedamage = stats.attackDamage;
        baseMoveSpeed = moveSpeed;
        baseAttackSpeed = attackSpeed;

        OnsetStats = true;

        ChangeDamage();
    }

    public bool DecreaseHealth(float amount)
    {
        curHp -= Mathf.RoundToInt(amount);
        if (curHp <= 0)
        {
            curHp = 0;
            OnHealthZero?.Invoke();
            return false;
        }

        return true;
    }

    public void IncreaseHealth(float amount)
    {
        curHp = Mathf.Clamp(curHp + Mathf.RoundToInt(amount), 0, maxHp);
    }


    public bool IsHpMax(float amount)
    {
        return CurHp >= MaxHp;
    }

    public void ChangeDamage(float currentDamage)
    {
        passiveMultiplier += currentDamage;
        ChangeDamage();
        //attackDamage *= (1 + currentDamage);
    }

    public void ChangeDamage()
    {
        attackDamage = (basedamage + addAttackDamage) * passiveMultiplier;
    }
    public void ReturnDamage()
    {
        passiveMultiplier = 1f;
        attackDamage = basedamage + addAttackDamage;
    }

    private void UpdateAttackSpeed()
    {
        attackSpeed = baseAttackSpeed * attackSpeedModifier * adjustStatsAttackSpeed * attackSpeedSlowMultiplier;
        UpdateAnimatorAttackSpeed();
    }

    private void UpdateMoveSpeed()
    {
        moveSpeed = baseMoveSpeed * moveSpeedModifier * adjustStatsMoveSpeed * moveSpeedSlowMultiplier;
        UpdateAnimatorMoveSpeed();
    }

    public void ModifyAttackSpeed(float multiplier)
    {
        attackSpeedModifier *= multiplier;
        UpdateAttackSpeed();
    }

    public void ApplyAttackSpeedSlow(float multiplier)
    {
        attackSpeedSlowMultiplier *= multiplier;
        UpdateAttackSpeed();
    }

    public void ResetAttackSpeedSlow()
    {
        attackSpeedSlowMultiplier = 1f;
        UpdateAttackSpeed();
    }

    public void ModifyMoveSpeed(float multiplier)
    {
        moveSpeedModifier *= multiplier;
        UpdateMoveSpeed();
    }

    public void ApplyMoveSpeedSlow(float multiplier, ElementalComponent component)
    {
        moveSpeedSlowMultiplier *= multiplier;
        UpdateMoveSpeed();

        var movement = component.GetMovement();
        if (movement != null)
        {
            movement.SetVelocityXEffect(TotalMoveSpeedMultiplier);
        }
    }

    public void ResetMoveSpeedSlow(ElementalComponent component)
    {
        moveSpeedSlowMultiplier = 1f;
        UpdateMoveSpeed();

        var movement = component.GetMovement();
        if (movement != null)
        {
            movement.SetVelocityZeroEffect();
        }
    }

    protected void SetAdjustStatsAttackSpeed(float multiplier)
    {
        adjustStatsAttackSpeed *= multiplier;
        UpdateAttackSpeed();
    }

    protected void SetAdjustStatsMoveSpeed(float multiplier)
    {
        adjustStatsMoveSpeed *= multiplier;
        UpdateMoveSpeed();
    }

    public void ChangeAttackSpeed(float newMultiplier)
    {
        slowEffectMultiplier *= newMultiplier;
        attackSpeed = baseAttackSpeed * slowEffectMultiplier;
        UpdateAnimatorAttackSpeed();
    }

    public void ReturnAttackSpeed()
    {
        attackSpeedModifier = 1f;
        UpdateAttackSpeed();
    }

    public void ChangeElement(Element newElement, int level = 0)
    {
        Element = newElement;
        elementalComponent?.ChangeElement(newElement, level);

    }


    public void UpdateElementalEffect(Element element, int level)
    {
        elementalComponent?.UpdateEffectValues(element, level);
    }

    public void ResetStatsToBaseValues()
    {
        adjustStatsAttackSpeed = 1f;
        adjustStatsMoveSpeed = 1f;

        UpdateAttackSpeed();
        UpdateMoveSpeed();
    }

    protected virtual void UpdateAnimatorSpeed()
    {
        UpdateAnimatorMoveSpeed();
        UpdateAnimatorAttackSpeed();
    }

    protected virtual void UpdateAnimatorMoveSpeed()
    {
        if (animator != null)
        {
            animator.SetFloat("MoveSpeed", moveSpeed);
        }
    }

    protected virtual void UpdateAnimatorAttackSpeed()
    {
        if (animator != null)
        {
            animator.SetFloat("AttackSpeed", attackSpeed);
        }
    }
}