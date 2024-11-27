using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public static class EnemyChangeStats
{
    private static float AttackSpeed = 1f;
    private static float MoveSpeed = 1f;

    public static event Action OnStatsUpdated;

    public static float BaseAttackSpeed
    {
        get => AttackSpeed;
        private set
        {
            AttackSpeed = value;
            OnStatsUpdated?.Invoke();
        }
    }

    public static float BaseMoveSpeed
    {
        get => MoveSpeed;
        private set
        {
            MoveSpeed = value;
            OnStatsUpdated?.Invoke();
        }
    }

    public static void SetBaseStats(string playerType)
    {
        ResetStatsToBaseValues();

        switch (playerType)
        {
            case "High_parry":
                SetAttackSpeed(1.5f);
                SetMoveSpeed(0.8f);
                break;

            case "parry":
                SetAttackSpeed(1.25f);
                SetMoveSpeed(0.9f);
                break;

            case "High_dash":
                SetAttackSpeed(0.8f);
                SetMoveSpeed(1.5f);
                break;

            case "dash":
                SetAttackSpeed(0.9f);
                SetMoveSpeed(1.25f);
                break;

            case "High_run":
                SetAttackSpeed(1.3f);
                SetMoveSpeed(1.3f);
                break;

            case "run":
                SetAttackSpeed(1.15f);
                SetMoveSpeed(1.15F);
                break;

            default:
                break;
        }

    }

    static void SetAttackSpeed(float multiplier)
    {
        BaseAttackSpeed *= multiplier;
    }

    static void SetMoveSpeed(float multiplier)
    {
        BaseMoveSpeed *= multiplier;
    }
    static void ResetStatsToBaseValues()
    {
        BaseAttackSpeed = 1f;
        BaseMoveSpeed = 1f;
    }
}