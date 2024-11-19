using System.Collections.Generic;
using UnityEngine;

public class SkillManager
{
    private Dictionary<string, SkillDataEx> skillDataDict = new Dictionary<string, SkillDataEx>();
    private Dictionary<string, Skill> skills = new Dictionary<string, Skill>();
    private Dictionary<string, SkillInitializer> initializers = new Dictionary<string, SkillInitializer>();
    //��ų�� �����Ѵٸ� ���� ��ų�� �޾ƿ��� ���� ���
    private string currentSkillName;
    private Skill currentSkill;

    // ��ų �����͸� �̸� �ε�
    public void LoadSkillData(SkillDataManager skillDataManager)
    {
        foreach (var skillName in skillDataManager.GetAllSkillNames())
        {
            var data = skillDataManager.GetSkillData(skillName);
            if (data != null)
            {
                skillDataDict[skillName] = data;
            }
        }
    }

    public void RegisterSkill(string skillName, Skill skill, SkillInitializer initializer)
    {
        skills[skillName] = skill;
        initializers[skillName] = initializer;
    }

    public void InitializeSkill(string skillName)
    {
        if (initializers.TryGetValue(skillName, out var initializer) && skillDataDict.TryGetValue(skillName, out var data))
        {
            initializer.Initialize(data, skillName);
            currentSkill = skills[skillName];
            currentSkillName = skillName;
        }
        else
        {
            Debug.LogError($"Initializer or Skill data not found for skill: {skillName}");
        }
    }

    public Skill GetSkill(string skillName)
    {
        if (skills.TryGetValue(skillName, out var skill))
        {
            return skill;
        }

        return null;
    }

    public void ChangeSkill(string newSkillName)
    {
        if (skillDataDict.TryGetValue(newSkillName, out var skillData) && initializers.TryGetValue(newSkillName, out var initializer))
        {
        //    Debug.Log("newSkillName : " +newSkillName);
            // ���� �̺�Ʈ ����
            if (currentSkill != null && initializers.TryGetValue(currentSkillName, out var currentInitializer))
            {
                currentInitializer.UnregisterEvents();
              //  Debug.Log("���� ��ų ����");
            }

            // ���� ������Ʈ ���� �� ������ �ʱ�ȭ
            currentSkill.ClearComponents();
            initializer.Initialize(skillData, newSkillName);  // ���ο� ��ų �ʱ�ȭ
         //   Debug.Log("���ο� ��ų ���� �Ϸ�");

            // ��ų �̸� ����
            currentSkill.gameObject.name = newSkillName;

            // ���� ��ų ���� ������Ʈ
            currentSkillName = newSkillName;
        }
    }

    public string GetCurrentSkillName()
    {
        return currentSkillName;
    }
}

/*
public void InitializeSkills()
{
    foreach (var skillName in skills.Keys)
    {
        if (initializers.TryGetValue(skillName, out var initializer))
        {
            initializer.Initialize(skillDataManager);
        }
    }
}
*/


