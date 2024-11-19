using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillState : PlayerAbilityState
{
    private Skill skill;


    public PlayerSkillState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string AnimBoolName, Skill skill) : base(player, stateMachine, playerData, AnimBoolName)
    {
        this.skill = skill;
        skill.OnExit += ExitHandler;
        skill.OnHold += HoldHandler;
    }

    public override void Enter()
    {
        base.Enter();
        skill.EnterSkill();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (GameManager.SharedCombatDataManager.IsPlayerHit)
        {
            skill.ExitSkill();
        }
    }
  
    private void HoldHandler()
    {
        if (player.InputHandler.SkillHoldInput)
        {
            skill.hold = true;
        }
        else
            skill.hold = false;
    }
    private void ExitHandler()
    {
        AnimationFinishTrigger();
        isAbilityDone = true;
    }

    
    public bool CanSkill()
    {
        return true;
        // ��ų ����� �������� ���� üũ
    }
}
