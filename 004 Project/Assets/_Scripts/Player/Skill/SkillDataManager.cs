using System.Collections.Generic;
using UnityEngine;

public class SkillDataManager
{
    private Dictionary<string, SkillDataEx> skillDataDict = new Dictionary<string, SkillDataEx>();

    public void Initialize()
    {
        if (GameManager.Data.SkillDict != null)
        {
            InitializeSkillData(GameManager.Data.SkillDict);
        }
        else
        {
            Debug.LogError("Failed to load skill data from JSON");
        }
    }

    private void InitializeSkillData(Dictionary<string, SkillDataJson> skillsJsonDict)
    {
        foreach (var skillEntry in skillsJsonDict)
        {
            SkillDataEx data = new SkillDataEx();
            var skillJson = skillEntry.Value;

            if (skillJson.damageData != null)
            {
                data.AddData(skillJson.damageData);
            }
            if (skillJson.cooldownData != null)
                data.AddData(skillJson.cooldownData);

            if (skillJson.movementData != null)
            {
                data.AddData(skillJson.movementData);
            }
            if (skillJson.lightData != null)
            {
                data.AddData(skillJson.lightData);
            }

            if (skillJson.fireData != null)
                data.AddData(skillJson.fireData);
            // �ٸ� ��ų �����͸� �ҷ����� �޼��� �߰�
            skillDataDict[skillEntry.Key] = data;
        }
    }
    public SkillDataEx GetSkillData(string skillName)
    {
        if (!skillDataDict.TryGetValue(skillName, out var data))
        {
            Debug.LogError($"Skill data not found for skill: {skillName}");
            return null;
        }
        return data;
    }

    public List<string> GetAllSkillNames()
    {
        return new List<string>(skillDataDict.Keys);
    }
}