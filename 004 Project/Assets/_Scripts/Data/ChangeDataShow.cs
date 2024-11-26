using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlaterDataShow : MonoBehaviour
{
    public TextMeshProUGUI playerType;
    public TextMeshProUGUI parryRatio;
    public TextMeshProUGUI runRatio;
    public TextMeshProUGUI dashRatio;
    public TextMeshProUGUI AttackSpeed;
    public TextMeshProUGUI MoveSpeed;


    // Start is called before the first frame update
    void Start()
    {
        EnemyChangeStats.OnStatsUpdated += UpdateUI;
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        playerType.text = "PlayerType : " + PlayerDataAnalyze.Instance.playerType;
        parryRatio.text = "parryratio : " + PlayerDataAnalyze.Instance.parryRatio.ToString("F2");
        runRatio.text = "runratio  : " + PlayerDataAnalyze.Instance.runRatio.ToString("F2");
        dashRatio.text = "dashratio    : " + PlayerDataAnalyze.Instance.dashRatio.ToString("F2");
    }

    private void UpdateUI()
    {
        AttackSpeed.text = EnemyChangeStats.BaseAttackSpeed.ToString("F2");
        MoveSpeed.text = EnemyChangeStats.BaseMoveSpeed.ToString("F2");

        UpdateTextColor(AttackSpeed, EnemyChangeStats.BaseAttackSpeed);
        UpdateTextColor(MoveSpeed, EnemyChangeStats.BaseMoveSpeed);
    }

    private void UpdateTextColor(TextMeshProUGUI textComponent, float value)
    {
        if (Mathf.Approximately(value, 1f))
        {
            textComponent.color = Color.white;
        }
        else if (value > 1f)
        {
            textComponent.color = Color.red;
        }
        else
        {
            textComponent.color = Color.blue;
        }
    }
}