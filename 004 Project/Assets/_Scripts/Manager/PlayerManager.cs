using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject Player { get; private set; }
    private SkillSetup skillSetup;

    public PlayerDataCollect PlayerDataCollect { get; private set; }
    public PlayerDataAnalyze DataAnalyze { get; private set; }

    public void Initialize(Vector3? position = null)
    {
        CreatePlayer(position);
        SetSkills();
    }

    private void CreatePlayer(Vector3? position = null)
    {
        Player = GameObject.Find("Player");

        if (Player == null)
        {
            if (position.HasValue)
            {
                // 지정된 위치에서 Player 생성
                Player = GameManager.Resource.Instantiate("Player", position.Value);
            }
            else
            {
                // 프리팹에 지정된 기본 위치에서 Player 생성
                Player = GameManager.Resource.Instantiate("Player");
            }
        }
        if (position.HasValue)
        {
            Player.transform.position = position.Value;
        }

        PlayerDataCollect = new PlayerDataCollect();
        DataAnalyze = new PlayerDataAnalyze();
        Camera.main.gameObject.GetComponent<MainCameraController>().SetPlayer(Player);
    }

    private void SetSkills()
    {
        skillSetup = new SkillSetup(Player);
    }

    public Skill GetCurrentSkill()
    {
        return skillSetup.GetCurrentSkill();
    }

    public void ChangeSkill(Element newElement)
    {
        skillSetup.ChangeSkill(newElement);
    }
}
