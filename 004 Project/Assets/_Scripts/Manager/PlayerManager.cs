using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject Player { get; private set; }
    private SkillSetup skillSetup;

    public PlayerDataCollect PlayerDataCollect { get; private set; }
    

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
                // ������ ��ġ���� Player ����
                Player = GameManager.Resource.Instantiate("Player", position.Value);
            }
            else
            {
                // �����տ� ������ �⺻ ��ġ���� Player ����
                Player = GameManager.Resource.Instantiate("Player");
            }
        }
        if (position.HasValue)
        {
            Player.transform.position = position.Value;
        }

        PlayerDataCollect = new PlayerDataCollect();
        
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
