using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoneEffect : IElementalEffect
{
    public void ApplyEffect(ElementalComponent targetComponent, ElementalComponent selfComponent, float attackerAttackStat)
    {
    }


    public float CalculateDamage(float baseDamage, float attackerAttackStat)
    {
        return baseDamage;
    }

    public void ApplyPassiveEffect(ElementalComponent component)
    {

    }
    public void RemovePassiveEffect(ElementalComponent component)
    {
    }

    public void UpdateEffectValues(float value1, float value2)
    {

    }

    public float GetColorAlpha()
    {
        return -1;
    }
}
public class FireEffect : IElementalEffect
{
    private float dotDamage;
    private float duration;
    private int maxStacks;

    private int level;
    // Ÿ�ٺ� �����͸� �����ϱ� ���� ��ųʸ�
    private Dictionary<ElementalComponent, int> currentStacks = new Dictionary<ElementalComponent, int>();
    private Dictionary<ElementalComponent, Coroutine> dotCoroutines = new Dictionary<ElementalComponent, Coroutine>();


    public void ApplyEffect(ElementalComponent attackerComponent, ElementalComponent defenderComponent, float attackerAttackStat)
    {
      //  Debug.Log($"attackerAttackStat {attackerAttackStat} , dotDamage  {dotDamage}");
        int dotDamageValue = Mathf.RoundToInt(attackerAttackStat * dotDamage);

        // currentStacks�� �����ڰ� ������ �ʱ�ȭ
        if (!currentStacks.ContainsKey(defenderComponent))
        {
            currentStacks[defenderComponent] = 0;
        }
        // ���� �ڷ�ƾ�� ������ ����
        if (dotCoroutines.ContainsKey(defenderComponent))
        {
            defenderComponent.StopEffectCoroutine(dotCoroutines[defenderComponent]);
            dotCoroutines.Remove(defenderComponent);
        }

        currentStacks[defenderComponent] = Mathf.Min(currentStacks[defenderComponent] + 1, maxStacks);
        Debug.Log($"currentFireStacks for {defenderComponent.name}: {currentStacks[defenderComponent]}");


        if (currentStacks[defenderComponent] >= maxStacks)
        {
            // �ִ� ���ÿ� �����ϸ� ���� ���� ����
            ApplyExplosionDamage(defenderComponent, attackerAttackStat);
            currentStacks[defenderComponent] = 0;
        }
        else
        {
            // DOT ������ �ڷ�ƾ ����
            Coroutine newCoroutine = defenderComponent.StartEffectCoroutine(DotDamageOverTime(defenderComponent, dotDamageValue));
            dotCoroutines[defenderComponent] = newCoroutine;
        }
    }

    public void UpdateEffectValues(float newDotDamage, float newDuration, int newMaxStacks, int level)
    {
        this.level = level;
  //      Debug.Log($"������Ʈ �� dotDamage : {dotDamage}");
  //      Debug.Log($"������Ʈ �� duration : {duration}");
  //      Debug.Log($"������Ʈ �� maxStacks : {maxStacks}");

        dotDamage = newDotDamage;
        duration = newDuration;
        maxStacks = newMaxStacks;
   //     Debug.Log($"������Ʈ �� dotDamage : {dotDamage}");
    //    Debug.Log($"������Ʈ �� duration : {duration}");
   //     Debug.Log($"������Ʈ �� maxStacks : {maxStacks}");
    }

    public float CalculateDamage(float baseDamage, float attackerAttackStat)
    {
        return baseDamage + attackerAttackStat * 0.2f;
    }

    private IEnumerator DotDamageOverTime(ElementalComponent target, int dotDamageValue)
    {
        float elapsedTime = 0f;
        float damageMultiplier = 1f + (currentStacks[target] - 1) * 0.1f;
        Debug.Log($" {duration} �ʰ� {dotDamageValue} ��Ʈ ������");

        while (elapsedTime < duration)
        {
            target.GetStats().DecreaseHealth(dotDamageValue * damageMultiplier);
            target.GetParticle().StartParticlesWithDesignatedRotation(target.damageParticles[(int) ElementParticles.FireDot], new Vector3(0,-0.5f,0), target.transform);
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }
        currentStacks[target] = 0; // DOT ���� �� ���� �ʱ�ȭ
        dotCoroutines.Remove(target); // �ڷ�ƾ ���� ����
    }
    private void ApplyExplosionDamage(ElementalComponent component, float attackerAttackStat)
    {
        float explosionDamage = attackerAttackStat * (maxStacks * 0.5f); 
        Debug.Log($"Explosion damage: {explosionDamage}");
        component.GetStats().DecreaseHealth(explosionDamage);
        // ���� ����Ʈ �߰�
        component.GetParticle().StartParticles(component.damageParticles[(int)ElementParticles.FireBoom], component.transform);
    }

    public void ApplyPassiveEffect(ElementalComponent component)
    {
        component.transform.root.Find("ShowElement").GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, GetColorAlpha());


    }
    public void RemovePassiveEffect(ElementalComponent component)
    {
        component.transform.root.Find("ShowElement").GetComponent<SpriteRenderer>().color = Color.white;
    }

    public float GetColorAlpha()
    {
        float alpha;
        switch (level)
        {
            case 1:
                alpha = 0.325f;
                break;
            case 2:
                alpha = 0.717f;
                break;
            case 3:
                alpha = 1f;
                break;
            default:
                alpha = 1f;
                break;
        }

        return alpha;
    }
}


public class IceEffect : IElementalEffect
{
    private float slowEffect;
    private float duration;
    private int maxSlowStacks;
    private float healthRegenRate;
    private float stunTime = 3.0f;
    private float cooldown = 5.0f;
    private int level;
    // Ÿ�ٺ� �����͸� �����ϱ� ���� ��ųʸ�
    private Dictionary<ElementalComponent, int> currentSlowStacks = new Dictionary<ElementalComponent, int>();
    private Dictionary<ElementalComponent, Coroutine> slowCoroutines = new Dictionary<ElementalComponent, Coroutine>();
    private Dictionary<ElementalComponent, Coroutine> cooldowns = new Dictionary<ElementalComponent, Coroutine>();
    private Dictionary<ElementalComponent, GameObject> debuffEffects = new Dictionary<ElementalComponent, GameObject>();
    private Dictionary<ElementalComponent, Coroutine> regenCoroutines = new Dictionary<ElementalComponent, Coroutine>();


    //    private Dictionary<ElementalComponent, Coroutine> cooldowns = new Dictionary<ElementalComponent, Coroutine>();
    private Dictionary<ElementalComponent, GameObject> freezeEffectFronts = new Dictionary<ElementalComponent, GameObject>();
    private Dictionary<ElementalComponent, GameObject> freezeEffectBacks = new Dictionary<ElementalComponent, GameObject>();


    public void ApplyEffect(ElementalComponent attackerComponent, ElementalComponent defenderComponent, float attackerAttackStat)
    {
        if (cooldowns.ContainsKey(defenderComponent))
        {
            Debug.Log("Ice effect ��ٿ�");
            return;
        }

        // currentSlowStacks�� �����ڰ� ������ �ʱ�ȭ
        if (!currentSlowStacks.ContainsKey(defenderComponent))
        {
            currentSlowStacks[defenderComponent] = 0;
        }

        // ���� ���ο� �ڷ�ƾ�� ������ ����
        if (slowCoroutines.ContainsKey(defenderComponent))
        {
            defenderComponent.StopEffectCoroutine(slowCoroutines[defenderComponent]);
            slowCoroutines.Remove(defenderComponent);
        }


        if (currentSlowStacks[defenderComponent] == 0)
        {
            GameObject debuffEffect = defenderComponent.GetParticle().StartParticlesWithDesignatedRotation(defenderComponent.damageParticles[(int)ElementParticles.IceDot], new Vector3(0, -0.5f, 0), defenderComponent.transform);
            debuffEffects[defenderComponent] = debuffEffect;
        }

        currentSlowStacks[defenderComponent] = Mathf.Min(currentSlowStacks[defenderComponent] + 1, maxSlowStacks);
        Debug.Log($"currentSlowStacks for {defenderComponent.name}: {currentSlowStacks[defenderComponent]}");
        
        var defenderStats = defenderComponent.GetStats();
        var defenderMovement = defenderComponent.GetMovement();

        if (currentSlowStacks[defenderComponent] >= maxSlowStacks)
        {
            // ������ ȿ�� ����
            Debug.Log($"Ice effect: Freezing target for {stunTime} second");
            var enemy = defenderComponent.transform.root.GetComponent<Entity>();
            enemy.stunState.SetFreezeStunTime(stunTime);
            enemy.stunState.stun = true;
            freezeEffectFronts[defenderComponent] = defenderComponent.GetParticle().StartParticlesWithDesignatedRotationAndDestroy(defenderComponent.damageParticles[(int)ElementParticles.IceFreezeStartFront], new Vector3(0, -0.5f, 0), defenderComponent.transform, stunTime);
            freezeEffectBacks[defenderComponent] = defenderComponent.GetParticle().StartParticlesWithDesignatedRotationAndDestroy(defenderComponent.damageParticles[(int)ElementParticles.IceFreezeStartBack], new Vector3(0, -0.5f, 0), defenderComponent.transform, stunTime);

            defenderComponent.StartEffectCoroutine(StartIceFreezeEnd(defenderComponent, stunTime));
          //  enemy.stateMachine.ChangeState(enemy.stunState);

            // ���� �ʱ�ȭ �� ���ο� ȿ�� ����
            currentSlowStacks[defenderComponent] = 0;

            if (defenderStats != null)
            {
                defenderStats.ResetAttackSpeedSlow();
                defenderStats.ResetMoveSpeedSlow(defenderComponent);
            }

            if (slowCoroutines.ContainsKey(defenderComponent))
            {
                defenderComponent.StopEffectCoroutine(slowCoroutines[defenderComponent]);
                slowCoroutines.Remove(defenderComponent);
            }

            if (debuffEffects.ContainsKey(defenderComponent))
            {
                defenderComponent.DestroyObj(debuffEffects[defenderComponent]);
                debuffEffects.Remove(defenderComponent);
            }
            cooldowns[defenderComponent] = defenderComponent.StartEffectCoroutine(CooldownCoroutine(defenderComponent));

        }
        else
        {

            // ��ȭ ȿ�� ����
            float slowMultiplier;
            if (currentSlowStacks[defenderComponent] == 1)
            {
                slowMultiplier = 1f - slowEffect; // ù ��° ������ slowEffect��ŭ ����
            }
            else
            {
                slowMultiplier = 0.9f; // ���� ������ 10%�� �߰� ����
            }

            if (defenderStats != null)
            {
                defenderStats.ApplyAttackSpeedSlow(slowMultiplier);
                defenderStats.ApplyMoveSpeedSlow(slowMultiplier, defenderComponent);
            }

            Coroutine slowCoroutine = defenderComponent.StartEffectCoroutine(SlowEffectCoroutine(defenderComponent, defenderMovement, defenderStats));
            slowCoroutines[defenderComponent] = slowCoroutine;
        }
    }

    public void UpdateEffectValues(float newSlowEffect, float newDuration, float newHealthRegenRate, int newMaxSlowStacks, int level)
    {
        this.level = level;
   //     Debug.Log($"������Ʈ �� slowEffect : {slowEffect}");
        slowEffect = newSlowEffect;
        duration = newDuration;
        healthRegenRate = newHealthRegenRate;
        maxSlowStacks = newMaxSlowStacks;
   //     Debug.Log($"������Ʈ �� slowEffect : {slowEffect}");

    }

    public float CalculateDamage(float baseDamage, float attackerAttackStat)
    {
        return baseDamage;
    }

    private IEnumerator StartIceFreezeEnd(ElementalComponent component ,float time)
    {
        yield return new WaitForSeconds(time);

        if (freezeEffectFronts.ContainsKey(component))
            component.DestroyObj(freezeEffectFronts[component]);
        if (freezeEffectBacks.ContainsKey(component))
            component.DestroyObj(freezeEffectBacks[component]);
        component.GetParticle().StartParticlesWithDesignatedRotation(component.damageParticles[(int)ElementParticles.IceFreezeEndFront], new Vector3(0, -0.5f, 0), component.transform);
        component.GetParticle().StartParticlesWithDesignatedRotation(component.damageParticles[(int)ElementParticles.IceFreezeEndBack], new Vector3(0, -0.5f, 0), component.transform);

    }
    private IEnumerator SlowEffectCoroutine(ElementalComponent defenderComponent, Movement defenderMovement, ICharacterStats defenderStats)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;

        }

        if (defenderStats != null)
        {
            defenderStats.ResetAttackSpeedSlow();
            defenderStats.ResetMoveSpeedSlow(defenderComponent);
        }

        if (debuffEffects.ContainsKey(defenderComponent))
        {
            defenderComponent.DestroyObj(debuffEffects[defenderComponent]);
            debuffEffects.Remove(defenderComponent);
        }
        currentSlowStacks[defenderComponent] = 0; // �ش� �������� ���ο� ���� �ʱ�ȭ
        slowCoroutines.Remove(defenderComponent); // �ڷ�ƾ ���� ����

        Debug.Log("Ice effect removed: slow effect ended for " + defenderComponent.name);
    }

    private IEnumerator CooldownCoroutine(ElementalComponent defenderComponent)
    {
        Debug.Log("Ice effect cooldown started for " + defenderComponent.name);
        yield return new WaitForSeconds(cooldown);
        cooldowns.Remove(defenderComponent);
        Debug.Log("Ice effect cooldown ended for " + defenderComponent.name);
    }

    public void ApplyPassiveEffect(ElementalComponent component)
    {
        if (!regenCoroutines.ContainsKey(component))
        {
            regenCoroutines[component] = component.StartEffectCoroutine(HealthRegenCoroutine(component));
        }
        component.transform.root.Find("ShowElement").GetComponent<SpriteRenderer>().color = new Color(0, 0, 1, GetColorAlpha());
    }

    public void RemovePassiveEffect(ElementalComponent component)
    {
        if (regenCoroutines.ContainsKey(component))
        {
            component.StopEffectCoroutine(regenCoroutines[component]);
            regenCoroutines.Remove(component);
        }
        component.transform.root.Find("ShowElement").GetComponent<SpriteRenderer>().color = Color.white;
    }

    private IEnumerator HealthRegenCoroutine(ElementalComponent component)
    {
        var stats = component.GetStats();
        while (true)
        {
            if (!stats.IsHpMax(healthRegenRate))
                component.GetParticle().StartParticlesWithDesignatedRotation(component.damageParticles[(int)ElementParticles.IcePassive], new Vector3(0, -0.85f, 0), component.transform);
            stats.IncreaseHealth(healthRegenRate);
            yield return new WaitForSeconds(1f);
        }
    }
    public float GetColorAlpha()
    {
        float alpha;
        switch (level)
        {
            case 1:
                alpha = 0.325f;
                break;
            case 2:
                alpha = 0.717f;
                break;
            case 3:
                alpha = 1f;
                break;
            default:
                alpha = 1f;
                break;
        }

        return alpha;
    }
}


public class LandEffect : IElementalEffect
{
    private float decreaseAttackSpeed;
    private float increaseAttackDamage;
    private int level;
    public void ApplyEffect(ElementalComponent attackerComponent, ElementalComponent defenderComponent, float attackerAttackStat)
    {
        // LandEffect�� �������� ApplyEffect�� ������ ����
    }

    public void UpdateEffectValues(float newDecreaseAttackSpeed, float newIncreaseAttackDamage, int level)
    {
        this.level = level;
  //      Debug.Log($"������Ʈ �� decreaseAttackSpeed : {decreaseAttackSpeed}");
 //       Debug.Log($"������Ʈ �� increaseAttackDamage : {increaseAttackDamage}");

        decreaseAttackSpeed = newDecreaseAttackSpeed;
        increaseAttackDamage = newIncreaseAttackDamage;
   //     Debug.Log($"������Ʈ �� decreaseAttackSpeed : {decreaseAttackSpeed}");
   //     Debug.Log($"������Ʈ �� increaseAttackDamage : {increaseAttackDamage}");
    }

    public float CalculateDamage(float baseDamage, float attackerAttackStat)
    {
        return baseDamage;
    }

    public void ApplyPassiveEffect(ElementalComponent component)
    {
        var stats = component.GetStats();
        if (stats != null)
        {
            stats.ModifyAttackSpeed(1 - decreaseAttackSpeed);
            stats.ChangeDamage(increaseAttackDamage);
        }
        component.transform.root.Find("ShowElement").GetComponent<SpriteRenderer>().color = new Color(0.82f, 0.41f, 0.12f, GetColorAlpha());
    }

    public void RemovePassiveEffect(ElementalComponent component)
    {
  //      Debug.Log($"Land effect removed: restoring original attack speed and damage");
        var stats = component.GetStats();
        if (stats != null)
        {
            stats.ReturnAttackSpeed();
            stats.ReturnDamage();
        }
        component.transform.root.Find("ShowElement").GetComponent<SpriteRenderer>().color = Color.white;
    }
    public float GetColorAlpha()
    {
        float alpha;
        switch (level)
        {
            case 1:
                alpha = 0.325f;
                break;
            case 2:
                alpha = 0.717f;
                break;
            case 3:
                alpha = 1f;
                break;
            default:
                alpha = 1f;
                break;
        }

        return alpha;
    }

}


public class LightEffect : IElementalEffect
{
    private float increaseAttackSpeed;
    private float decreaseAttackDamage;
    private int level;
    public void ApplyEffect(ElementalComponent targetComponent, ElementalComponent selfComponent, float attackerAttackStat)
    {
        // LightEffect �������� ApplyEffect�� ������ ����

    }

    public void UpdateEffectValues(float newIncreaseAttackSpeed, float newDecreaseAttackDamage, int level)
    {
        this.level = level;
        increaseAttackSpeed = newIncreaseAttackSpeed;
        decreaseAttackDamage = newDecreaseAttackDamage;
    }

    public float CalculateDamage(float baseDamage, float attackerAttackStat)
    {
        return baseDamage;
    }

    public void ApplyPassiveEffect(ElementalComponent component)
    {
        var stats = component.GetStats();
        if (stats != null)
        {
            stats.ModifyAttackSpeed(1 + increaseAttackSpeed);
            stats.ChangeDamage(-decreaseAttackDamage);
        }
        component.transform.root.Find("ShowElement").GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, GetColorAlpha());
    }

    public void RemovePassiveEffect(ElementalComponent component)
    {
        var stats = component.GetStats();
        if (stats != null)
        {
            stats.ReturnAttackSpeed();
            stats.ReturnDamage();
        }
        component.transform.root.Find("ShowElement").GetComponent<SpriteRenderer>().color = Color.white;

    }
    public float GetColorAlpha()
    {
        float alpha;
        switch (level)
        {
            case 1:
                alpha = 0.325f;
                break;
            case 2:
                alpha = 0.717f;
                break;
            case 3:
                alpha = 1f;
                break;
            default:
                alpha = 1f;
                break;
        }

        return alpha;
    }
}
