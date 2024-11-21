using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    // Start 버튼 클릭 시 호출
    public void LoadStartScene()
    {
        SceneManager.LoadScene("Test 2"); // "SampleScene"으로 변경
    }

    // Developer Tools 버튼 클릭 시 호출
    public void LoadDeveloperToolsScene()
    {
        SceneManager.LoadScene("Test 3"); // "Test 1"으로 변경
    }

    // Exit 버튼 클릭 시 호출
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Unity Editor에서 실행 중지
        #else
        Application.Quit(); // 실제 빌드된 게임 종료
        #endif
    }
}