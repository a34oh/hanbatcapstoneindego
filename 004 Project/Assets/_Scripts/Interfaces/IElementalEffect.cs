using UnityEngine;

public interface IElementalEffect
{
    void ApplyEffect(ElementalComponent attackerComponent, ElementalComponent defenderComponent, float attackerAttackStat);
    float CalculateDamage(float baseDamage, float attackerAttackStat);
    //void UpdateEffectValues(float value1, float value2); // �߰�
    void ApplyPassiveEffect(ElementalComponent component);
    void RemovePassiveEffect(ElementalComponent component);

    float GetColorAlpha();
}
