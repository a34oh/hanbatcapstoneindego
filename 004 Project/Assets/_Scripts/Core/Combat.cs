using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : CoreComponent, IDamageable, IKnockbackable
{


    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private CollisionSenses CollisionSenses { get => collisionSenses ?? core.GetCoreComponent(ref collisionSenses); }

    private ParticleManager ParticleManager => particleManager ? particleManager : core.GetCoreComponent(ref particleManager);
    //  private ElementalComponent ElementalComponent { get => elementalComponent ?? core.GetCoreComponent(ref elementalComponent);}
    private ElementalComponent ElementalComponent => elementalComponent ? elementalComponent : core.GetCoreComponent(ref elementalComponent);



    private Movement movement;
    private CollisionSenses collisionSenses;
    private ICharacterStats stats;
    private ParticleManager particleManager;
    private ElementalComponent elementalComponent;


    [SerializeField] private float maxKnockbackTime = 0.2f;

    private bool isKnockbackActive;
    private float knockbackStartTime;

    protected override void Awake()
    {
        base.Awake();
        stats = transform.root.GetComponentInChildren<ICharacterStats>();
        if (stats == null)
            Debug.Log("stats ��");
    }

    public override void LogicUpdate()
    {
        CheckKnockback();
    }

    public void NonElementDamage(float amount, Transform defender)
    {
        bool isAlive = stats?.DecreaseHealth(amount) ?? false;
        if(isAlive)
        {
            defender.root.GetComponent<CharacterAudio>().PlayHitSound();
        }
        ParticlesWithRandomRotation(ElementalComponent.damageParticles[0], defender);
    }
    public void SkillDamage(float baseDamage, Element attackerElement, float attackerAttackStat, GameObject attacker, Transform defender)
    {
        ApplyDamage(baseDamage, attackerElement, attackerAttackStat, attacker, defender, ElementalComponent.damageParticles[(int)attackerElement]);
    }

    public void Damage(float baseDamage, Element attackerElement, float attackerAttackStat, GameObject attacker, Transform defender)
    {
        ApplyDamage(baseDamage, attackerElement, attackerAttackStat, attacker, defender, ElementalComponent.damageParticles[0]);
    }

    public void ParticlesWithRandomRotation(GameObject particle, Transform defender)
    {
        ParticleManager?.StartParticlesWithRandomRotation(particle, defender);
    }
    private void ApplyDamage(float baseDamage, Element attackerElement, float attackerAttackStat, GameObject attacker, Transform defender, GameObject particle)
    {
        float multiplier = ElementalComponent.GetDamageMultiplier(attackerElement);
        float calculatedDamage = ElementalComponent.CalculateDamage(attackerElement, baseDamage, attackerAttackStat);
        // Debug.Log("�Ӽ� �߰� ������ ���� : " + multiplier);
        //  Debug.Log("������ ������ : " + calculatedDamage);
        float finalDamage = calculatedDamage * multiplier;
        Debug.Log("���� ������ : " + finalDamage);
        //     Debug.Log("(int)attackerElement : " + (int)attackerElement);

        // ���޵� ��ƼŬ�� ����Ͽ� ��ƼŬ ȿ�� ����
        ParticlesWithRandomRotation(particle, defender);

        bool isAlive = stats?.DecreaseHealth(finalDamage) ?? false;
        if (isAlive)
        {
            var attackerComponent = attacker.GetComponent<ElementalComponent>();
            ElementalComponent.ApplyElementalEffect(attackerElement, gameObject, attackerAttackStat, attackerComponent);
            defender.root.GetComponent<CharacterAudio>().PlayHitSound();
        }
    }


    public void Knockback(Vector2 angle, float strength, int direction)
    {
        Movement?.SetVelocity(angle, strength, direction);
        Movement.CanSetVelocity = false;
        isKnockbackActive = true;
        knockbackStartTime = Time.time;
    }
    public void ResetKnockback()
    {
        Movement.CanSetVelocity = true;
        Movement?.SetVelocityZero();
        isKnockbackActive = false;
    }

    private void CheckKnockback()
    {
        if (isKnockbackActive
            && ((Movement?.CurrentVelocity.y <= 0.01f && CollisionSenses.Ground)
                || Time.time >= knockbackStartTime + maxKnockbackTime))
        {
            isKnockbackActive = false;
            Movement.CanSetVelocity = true;
        }
    }

    public void DamageWithShield(float amount, Transform defender)
    {
        NonElementDamage(amount / 2, defender);
    }

    public void KnockbackWithShield(Vector2 angle, float strength, int direction)
    {
        Knockback(angle, strength / 2, direction);
    }
}

