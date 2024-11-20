using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI;          // Game Over UI (Canvas)
    public GameObject youDiedText;         // "You Died" 텍스트
    public GameObject pressAnyKeyText;     // "Press Any Key" 텍스트
    public GameObject backgroundOverlay;   // 화면을 어둡게 하는 배경

    private bool isGameOver = false;       // 게임 오버 상태
    private bool canAcceptInput = false;   // 입력 허용 상태
    public float inputDelay = 2f;          // 입력 지연 시간 (초)

    private void Update()
    {
        if (isGameOver && canAcceptInput && Input.anyKeyDown)
        {
            // "Title" 씬으로 전환
            SceneManager.LoadScene("Title");
        }
    }

    public void TriggerGameOver()
{
    if (gameOverUI != null)
    {
        gameOverUI.SetActive(true);

        if (youDiedText != null) youDiedText.SetActive(true);
        if (backgroundOverlay != null) backgroundOverlay.SetActive(true);

        isGameOver = true;
        canAcceptInput = false;

        // 일정 시간 후 "Press Any Key" 표시 및 입력 허용
        if (pressAnyKeyText != null)
        {
            pressAnyKeyText.SetActive(false); // 초기에는 숨김
            Invoke(nameof(ShowPressAnyKeyText), inputDelay);
        }

        Invoke(nameof(EnableInput), inputDelay);
    }
}private void ShowPressAnyKeyText()
{
    if (pressAnyKeyText != null)
    {
        pressAnyKeyText.SetActive(true); // 일정 시간 후 표시
    }
}

    private void EnableInput()
    {
        canAcceptInput = true;            // 입력 허용
    }
}