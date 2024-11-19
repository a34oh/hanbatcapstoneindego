using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandGenerator : SkillGenerator
{
    protected override void InitializeSkillComponents(Skill skill, SkillDataEx data)
    {
        // ����: SkillMovement ������Ʈ �߰� �� �ʱ�ȭ
        SkillMovementData movementData = skill.Data.GetData<SkillMovementData>();
        if (movementData != null)
        {
            skill.gameObject.GetOrAddComponent<SkillMovement>().Init();
        }

        SkillDamageData damageData = skill.Data.GetData<SkillDamageData>();
        if (damageData != null)
        {
            skill.gameObject.GetOrAddComponent<SkillDamage>().Init();
        }


        //  SkillFireData fireData = skill.Data.GetData<SkillFireData>();
        //    if (fireData != null)
        //    {
        //        skill.gameObject.GetOrAddComponent<SkillFire>().Init();
        //    }

        // �߰����� ��ų ������Ʈ �ʱ�ȭ ����
        // ...


    }
}