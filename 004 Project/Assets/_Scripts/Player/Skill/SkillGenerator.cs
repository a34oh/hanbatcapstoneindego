using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillGenerator
{
    public GameObject skillPrefab;

    public virtual void InitializeSkill(string skillName, Skill skill, SkillDataEx data, GameObject collisionTarget = null)
    {
        skill.SetData(data);
        skill.transform.GetChild(0).GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"Animations/Player/Skill/{skillName}/{skillName}Anim_Base");
        skill.transform.GetChild(1).GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"Animations/Player/Skill/{skillName}/{skillName}Anim_Weapon");

        // �ʿ��� ��� CollisionHandler�� �������� �Ҵ�.
        if (collisionTarget != null)
        {
            InitializeCollisionHandler(collisionTarget);
        }
        else
        {
            //�ӽ�
            InitializeCollisionHandler(skill.transform.GetChild(1).gameObject);
        }

        // �ʿ��� ��ų ������Ʈ �߰� �� �ʱ�ȭ
        InitializeSkillComponents(skill, data);

       

    }

    // �� ��ų���� �ʿ��� ������Ʈ �߰� �� �ʱ�ȭ   
    protected abstract void InitializeSkillComponents(Skill skill, SkillDataEx data);

    private void InitializeCollisionHandler(GameObject target)
    {
        var collisionHandler = target.GetOrAddComponent<CollisionHandler>();
    }
}