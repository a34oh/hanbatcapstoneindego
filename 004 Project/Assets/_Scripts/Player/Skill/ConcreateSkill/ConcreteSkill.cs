using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillStateName
{
    public const string Enter = "Enter";
    public const string Exit = "Exit";
    public const string Hold = "Hold";
    public const string Fire = "Fire";
    public const string Attack = "Attack";
    public const string Attack1 = "Attack1";
    public const string Attack2 = "Attack2";
}
public abstract class ConcreteSkill : ISkillAction
{
    protected Skill skill;
    public Animator baseAnim;
    public Animator weaponAnim;

  
    public virtual void Initialize(Skill skill, GameObject prefab = null, Transform prefabParent = null, Transform playerTransform = null, Vector2 prefabOffset = default(Vector2))
    {
        this.skill = skill;
        baseAnim = skill.BaseGameObject.GetComponent<Animator>();
        weaponAnim = skill.WeaponGameObject.GetComponent<Animator>();
        skill.OnLogicUpdate += LogicUpdate;
    }
    public virtual void Enter()
    {
      //  stateMachine.Initialize(SkillEnterState);
    }

    public virtual void Exit()
    {
    }
    private void LogicUpdate()
    {
        if (skill.StateMachine.CurrentState != null)
        {
            skill.StateMachine.CurrentState.LogicUpdate();
        }
    }

}

public class SkillStateMachine
{
    public SkillState CurrentState { get; private set; }
    public void Initialize(SkillState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(SkillState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
public class SkillState
{

    protected ConcreteSkill concreteSkill;
    protected Skill skill;
    protected Animator baseAnim;
    protected Animator weaponAnim;
    protected string animBoolName;
    protected bool isExitingState;
    protected SkillStateMachine stateMachine;

    protected SkillMovement skillMovement;
    protected SkillDamage skillDamage;
    protected SkillLight skillLight;
    //��Ÿ Component��.. �� Component���� ��ӹ��� �ְ� ����

    public SkillState(ConcreteSkill concreteSkill, Skill skill, SkillStateMachine stateMachine, string animBoolName)
    {
        this.concreteSkill = concreteSkill;
        this.skill = skill;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
        baseAnim = concreteSkill.baseAnim;
        weaponAnim = concreteSkill.weaponAnim;        
    }
    public virtual void Enter()
    {
        baseAnim.SetBool(animBoolName, true);
        weaponAnim.SetBool(animBoolName, true);
        isExitingState = false;
        skillMovement ??= skill.GetComponent<SkillMovement>();
        skillDamage ??= skill.GetComponent<SkillDamage>();
        skillLight ??= skill.GetComponent<SkillLight>();
        // Debug.Log(this.ToString() + "���� ����");
    }
    public virtual void Exit()
    {
        //Debug.Log(this.ToString() + "���� ����");
        baseAnim.SetBool(animBoolName, false);
        weaponAnim.SetBool(animBoolName, false);
        isExitingState = true;
    }
    public virtual void LogicUpdate()
    {

    }
}


public class SkillEnterState : SkillState
{
    public SkillEnterState(ConcreteSkill concreteSkill, Skill skill, SkillStateMachine stateMachine, string animBoolName) : base(concreteSkill, skill, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        skill.EventHandler.OnStateFinish += EventHandler;
        GameManager.SharedCombatDataManager.IsPlayerNotEnterHitState = true;


    }

    public override void Exit()
    {
        base.Exit();
        skill.EventHandler.OnStateFinish -= EventHandler;


    }
    public override void LogicUpdate()
    {
        if(!isExitingState)
        {
            skillMovement.HandleStopMovementX();
        }
    }
    protected virtual void EventHandler()
    {

    }
}

public class SkillExitState : SkillState
{
    public SkillExitState(ConcreteSkill concreteSkill, Skill skill, SkillStateMachine stateMachine, string animBoolName) : base(concreteSkill, skill, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        //skill.EventHandler.OnFinish -= EventHandler;
        skill.EventHandler.OnFinish += EventHandler;
        skill.EventHandler.AnimationFinishedTriggerFunc();
    }

    public override void Exit()
    {
        base.Exit();
        skill.EventHandler.OnFinish -= EventHandler;
        skill.EventHandler.OnFinish -= EventHandler;
        GameManager.SharedCombatDataManager.IsPlayerNotEnterHitState = false;
    }
    public override void LogicUpdate()
    {
        if (!isExitingState)
        {
            skillMovement.HandleStopMovementX();
        }
        //�ִϸ��̼�finishTrigger�� �ϸ� �����.
    }

    protected void EventHandler()
    {
        Exit();
    }
}
