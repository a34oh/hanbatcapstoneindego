using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy2UI : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI attackSpeed;
    public TextMeshProUGUI moveSpeed;
    public Canvas worldSpaceCanvas;

    Define.Scene sceneType;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        GameObject go = GameObject.Find("Scene");
        if (go == null)
            Debug.Log("�̷��ȵ�.");
        else
        {
            sceneType = go.GetComponent<BaseScene>().SceneType;
        }
    }

    private void Update()
    {
      
    }

    private void LateUpdate()
    {
        if (sceneType == Define.Scene.Test)
        {
            // �θ��� Y�� ȸ���� Ȯ��
            float parentRotationY = transform.rotation.eulerAngles.y;

            if (Mathf.Approximately(parentRotationY, 180f))
            {
                // �θ� 180���� ȸ���� ���, ĵ���� ȸ���� 0���� ����
                worldSpaceCanvas.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                // �⺻ ����: ĵ������ ī�޶� �ٶ󺸵��� ����
                Vector3 lookDirection = mainCamera.transform.position - worldSpaceCanvas.transform.position;
                worldSpaceCanvas.transform.rotation = Quaternion.LookRotation(-lookDirection);
                worldSpaceCanvas.transform.Rotate(0, 180, 0); // ĵ���� ������ ����
            }

            UpdateUI();
        }
    }
    private void UpdateUI()
    {
        if (attackSpeed != null)
        {
            attackSpeed.text = EnemyChangeStats.BaseAttackSpeed.ToString("F2");
            UpdateTextColor(attackSpeed, EnemyChangeStats.BaseAttackSpeed);
        }

        if (moveSpeed != null)
        {
            moveSpeed.text = EnemyChangeStats.BaseMoveSpeed.ToString("F2");
            UpdateTextColor(moveSpeed, EnemyChangeStats.BaseMoveSpeed);
        }
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
