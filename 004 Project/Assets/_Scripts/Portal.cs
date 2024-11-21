using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Portal : MonoBehaviour
{
    public GameObject text;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) text.SetActive(true);
    }
    void OnTriggerStay2D(Collider2D other) 
    {
        if (other.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.F))
        {

            {
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

                // 다음 씬의 인덱스 계산
                int nextSceneIndex = currentSceneIndex + 1;

                // 씬이 빌드에 포함되어 있는지 확인
                if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                {
                    PlayerStats.savePlayerData = true;

                    // 다음 씬 로드
                    SceneManager.LoadScene(nextSceneIndex);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) text.SetActive(false);
    }
}