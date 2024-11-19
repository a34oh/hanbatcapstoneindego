using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGenerator : SkillGenerator
{
    protected override void InitializeSkillComponents(Skill skill, SkillDataEx data)
    {
        // Skill ������Ʈ �߰� �� �ʱ�ȭ
        SkillMovementData movementData = data.GetData<SkillMovementData>();
        if (movementData != null)
        {
            skill.gameObject.GetOrAddComponent<SkillMovement>().Init();
        }

        SkillDamageData damageData = data.GetData<SkillDamageData>();
        if (damageData != null)
        {
            skill.gameObject.GetOrAddComponent<SkillDamage>().Init();
        }

        //Light���� Component�� Data�� �������� Init()
        SkillLightData lightData = data.GetData<SkillLightData>();

        if(lightData != null)
        {
            SkillLight light = skill.gameObject.GetOrAddComponent<SkillLight>();
            light.Init(data);
        }
        // �߰����� ��ų ������Ʈ �ʱ�ȭ ����
        // ...


    }
}
