using UnityEngine;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour
{
    [SerializeField] private Image hpBarImage; // HP바 Image
    [SerializeField] private Text hpText;      // 체력 텍스트
    private PlayerStats playerStats;

    private void Start()
    {
        // Player 오브젝트와 연결
        var player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player를 찾을 수 없습니다. Tag를 확인하세요.");
            return;
        }

        // 자식 오브젝트 Stats에서 PlayerStats 찾기
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

        Debug.Log($"PlayerStats 연결 성공: CurHp = {playerStats.CurHp}, MaxHp = {playerStats.MaxHp}");
        UpdateHPBar();
    }

    private void Update()
    {
        if (playerStats != null)
        {
            UpdateHPBar();
        }
    }

    private void UpdateHPBar()
    {
        // 현재 체력 비율 계산
        float hpRatio = (float)playerStats.CurHp / playerStats.MaxHp;
        hpRatio = Mathf.Clamp01(hpRatio);

        // HP바 업데이트
        hpBarImage.fillAmount = hpRatio;

        // 텍스트 업데이트 (현재 체력 / 최대 체력)
        hpText.text = $"{playerStats.CurHp} / {playerStats.MaxHp}";
    }
}