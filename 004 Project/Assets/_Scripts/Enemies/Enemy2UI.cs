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
            Debug.Log("이럼안됨.");
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
            // 부모의 Y축 회전값 확인
            float parentRotationY = transform.rotation.eulerAngles.y;

            if (Mathf.Approximately(parentRotationY, 180f))
            {
                // 부모가 180도로 회전한 경우, 캔버스 회전을 0으로 설정
                worldSpaceCanvas.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                // 기본 동작: 캔버스가 카메라를 바라보도록 설정
                Vector3 lookDirection = mainCamera.transform.position - worldSpaceCanvas.transform.position;
                worldSpaceCanvas.transform.rotation = Quaternion.LookRotation(-lookDirection);
                worldSpaceCanvas.transform.Rotate(0, 180, 0); // 캔버스 뒤집힘 방지
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
