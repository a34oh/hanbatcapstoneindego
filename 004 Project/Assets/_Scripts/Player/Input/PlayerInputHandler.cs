using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    private Camera cam;
    public Vector2 MovementInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool JumpInput { get; private set; }
    public bool JumpInputStop { get; private set; }

    public bool[] AttackInputs { get; private set; }
    public bool DashInput { get; private set; }
    public bool GrabInput { get; private set; }

    public bool ShieldInput { get; private set; }
    public bool ShieldHoldInput { get; private set; }


    public bool SkillInput { get; private set; }
    public bool SkillHoldInput { get; private set; }
    public bool SkillChangeInput { get; private set; }
    [SerializeField]
    private float inputHoldTime = 0.2f;

    private float jumpInputStartTime;
    private float DashInputStartTime;
    private float ShieldHoldInputStartTime;
    private float SkillHoldInputStartTime;
    private bool isParrySuccessful;
    public event Action OnSkillChangeInputAction;


    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        int count = Enum.GetValues(typeof(CombatInputs)).Length;
        AttackInputs = new bool[count];

        cam = Camera.main;
    }
    private void Update()
    {
        CheckJumpInputHoldTime();
        CheckDashInputHoldTime();
        CheckShieldInputHolding();
        CheckSkillInputHolding();
    }


    public void OnPrimaryAttackInput(InputAction.CallbackContext context)
    {
        // UI 클릭 중이면 공격 무시
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!InventoryUI.instance.activeInventory)
        {
            if (context.started)
                AttackInputs[(int)CombatInputs.primary] = true;
        }
        if (context.canceled)
            AttackInputs[(int)CombatInputs.primary] = false;
    }
    public void OnSecondaryAttackInput(InputAction.CallbackContext context)
    {
        // UI 클릭 중이면 공격 무시
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (context.started)
            AttackInputs[(int)CombatInputs.secondary] = true;

        if (context.canceled)
            AttackInputs[(int)CombatInputs.secondary] = false;
    }
    public void OnMovementInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
        NormInputX = Mathf.RoundToInt(MovementInput.x);
        NormInputY = Mathf.RoundToInt(MovementInput.y);
    }
    public void OnGrabInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GrabInput = true;
        }

        if (context.canceled)
        {
            GrabInput = false;
        }
    }
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            JumpInput = true;
            JumpInputStop = false;

            jumpInputStartTime = Time.time;
        }
        if (context.canceled)
        {
            JumpInputStop = true;
        }

    }


    public void UseJumpInput() => JumpInput = false;
    public void UseDashInput() => DashInput = false;
    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            DashInput = true;
            DashInputStartTime = Time.time;
        }
    }
    private void CheckJumpInputHoldTime()
    {
        if (Time.time >= jumpInputStartTime + inputHoldTime)
        {
            JumpInput = false;
        }
    }

    private void CheckDashInputHoldTime()
    {
        if (Time.time >= DashInputStartTime + inputHoldTime)
        {
            DashInput = false;
        }
    }
    public void OnShieldInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ShieldInput = true;
            ShieldHoldInputStartTime = Time.time;
        }

        if (context.canceled)
        {
            isParrySuccessful = false;
            ShieldInput = false;
            ShieldHoldInput = false;
        }
    }
    private void CheckShieldInputHolding()
    {
        // �и� ���� �� �߰� ���� ����
        if (isParrySuccessful) return;

        if (ShieldInput && Time.time >= ShieldHoldInputStartTime + inputHoldTime)
        {
            ShieldHoldInput = true;
        }
    }
    public void OnParrySuccess()
    {
        // �и� ���� �� �÷��� ���� �� �Է� �ʱ�ȭ
        isParrySuccessful = true;
        ShieldInput = false;
        ShieldHoldInput = false;
    }

    public void OnSkillInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SkillInput = true;
            SkillHoldInputStartTime = Time.time;
        }
        if (context.canceled)
        {
            SkillInput = false;
            SkillHoldInput = false;
        }
    }
    private void CheckSkillInputHolding()
    {
        if (SkillInput && Time.time >= SkillHoldInputStartTime + inputHoldTime)
        {
            SkillHoldInput = true;
        }
    }

    public void OnSkillChangeInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnSkillChangeInputAction?.Invoke();
        }
    }
}
public enum CombatInputs
{
    primary = 0,
    secondary
}
