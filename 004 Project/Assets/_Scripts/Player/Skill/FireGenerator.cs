using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGenerator : SkillGenerator
{
    protected override void InitializeSkillComponents(Skill skill, SkillDataEx data)
    {
        // ����: SkillMovement ������Ʈ �߰� �� �ʱ�ȭ
        SkillMovementData movementData = data.GetData<SkillMovementData>();
        if (movementData != null)
        {
            skill.gameObject.GetOrAddComponent<SkillMovement>().Init();
        }

        SkillDamageData damageData = data.GetData<SkillDamageData>();
        if (damageData != null)
        {
            skill.gameObject.GetOrAddComponent<SkillDamage>().Init(data);
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