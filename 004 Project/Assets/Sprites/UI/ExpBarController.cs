using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpBarController : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;   // 레벨 텍스트
    [SerializeField] private Image expBarImage; // 경험치 바 이미지
    private PlayerStats playerStats;
    private float currentFillAmount; // 현재 fillAmount 상태를 저장

    private void Start()
    {
        // PlayerStats 연결
        var player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player를 찾을 수 없습니다.");
            return;
        }

        Transform statsTransform = player.transform.Find("Stats");
        if (statsTransform == null)
        {
            Debug.LogError("Stats라는 자식 오브젝트를 찾을 수 없습니다.");
            return;
        }

        playerStats = statsTransform.GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        currentFillAmount = 0f; // 초기화
        UpdateUI(); // 초기화
        
    }

    private void Update()
    {
        if (playerStats != null)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
{
    // 레벨 텍스트 업데이트
    levelText.text = $"Level: {playerStats.Level}";

    // 목표 경험치 비율
    float targetExpRatio = (float)playerStats.currentExp / playerStats.MaxExp;

    currentFillAmount = Mathf.Lerp(currentFillAmount, targetExpRatio, Time.deltaTime * 5f);
    expBarImage.fillAmount = Mathf.Clamp01(currentFillAmount); // fillAmount 업데이트
}
}