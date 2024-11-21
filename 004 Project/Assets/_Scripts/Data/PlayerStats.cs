using UnityEngine;

public class PlayerStats : CharacterStats<PlayerStatsData>
{
    [SerializeField] public int level;
    [SerializeField] public int MaxExp;
    public int currentExp;

    public static bool savePlayerData = false;
    public static Element currentElement;
    public static int currentLevel;
    public static float currentAddAttackDamage;
    public static int currentHp;
    public static int exp;
    public static int currentGoldPoint = 100;

    public int Level 
    {
        get => level;
        set
        {
            level = value;
            SetStat();
        }
    }
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void SetStatsData(PlayerStatsData stats, bool isSave)
    {
        base.SetStatsData(stats, isSave);
        MaxExp = stats.MaxExp;
    }
    private void OnEnable()
    {
        if (savePlayerData)
            InitializeLoadPlayerStats();
        else
            InitializePlayerStats();

    }

    private void OnDisable()
    {
        if (isDead == true)
        {
            transform.root.GetComponent<CharacterAudio>().PlayDeathSound();
        }
        else
        {
            currentElement = Element;
            currentLevel = level;
            currentAddAttackDamage = addAttackDamage;
            currentHp = curHp;
            exp = currentExp;
        }
    }
    private void Start()
    {
        if (!savePlayerData)
            ChangeElement(Element.Fire, fireLevel);
        else
            ChangeElement(currentElement, fireLevel);
    }

    private void InitializeLoadPlayerStats()
    {
        id = 1;
        level = currentLevel;
        currentExp = exp;
        Element = currentElement;
        curHp = currentHp;
        addAttackDamage = currentAddAttackDamage;
        SetStat();

    }
    private void InitializePlayerStats()
    {
        // ���⼭ id�� level�� �ʱ�ȭ
        id = 1; // �÷��̾��� ���� ID ����
        level = 1; // �ʱ� ���� ����
        currentExp = 0;
        SetStat();

    }

    protected override void SetStat()
    {
        // id�� level�� �´� ������ �˻�
        foreach (var kvp in GameManager.Data.PlayerStatsDict)
        {
            var stats = kvp.Value;
            if (stats.id == id && stats.level == level)
            {
                SetStatsData(stats, savePlayerData);
                return;
            }
        }
        Debug.LogError("Failed to load player stats for id: " + id + " and level: " + level);
    }

    public void AddExp(int exp)
    {
        currentExp += exp;
        Debug.Log($"����ġ ȹ��: {exp}, ���� ����ġ: {currentExp}/{MaxExp}");

        if (currentExp >= MaxExp)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        currentExp -= MaxExp; // �ʰ��� ����ġ�� ���� ������ �̾���
        Level++; // ���� ����
        Debug.Log($"������! ���� ����: {Level}");
    }

    protected override void UpdateAnimatorMoveSpeed()
    {
        if (animator != null)
        {
            animator.SetFloat("MoveSpeed", moveSpeed);
        }
    }
    protected override void UpdateAnimatorAttackSpeed()
    {
        if (animator != null)
        {
            GameObject sword = transform.parent.Find("Weapons").GetChild(0).gameObject;

            Animator swordbaseAnim = sword.transform.GetChild(0).GetComponent<Animator>();
            Animator swordweaponAnim = sword.transform.GetChild(1).GetComponent<Animator>();

            Debug.Log(attackSpeed);
            swordbaseAnim.SetFloat("AttackSpeed", attackSpeed);
            swordweaponAnim.SetFloat("AttackSpeed", attackSpeed);
        }
    }
    private void Update()
    {
        HandleElementChange();
    }
    private void HandleElementChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!transform.root.GetComponentInChildren<Skill>(true).gameObject.activeSelf)
            {
                ChangeElement(Element.Fire, fireLevel);
                Debug.Log("Player ChangeElement : Fire");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!transform.root.GetComponentInChildren<Skill>(true).gameObject.activeSelf)
            {
                ChangeElement(Element.Ice, iceLevel);
                Debug.Log("Player ChangeElement : Ice");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (!transform.root.GetComponentInChildren<Skill>(true).gameObject.activeSelf)
            {
                ChangeElement(Element.Land, landLevel);
                Debug.Log("Player ChangeElement : Land");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (!transform.root.GetComponentInChildren<Skill>(true).gameObject.activeSelf)
            {
                ChangeElement(Element.Light, lightLevel);
                Debug.Log("Player ChangeElement : Light");
            }
        }
    }




    public void IncreaseElementLevel(Element element)
    {
        int level = 0;
        switch (element)
        {
            case Element.Fire:
                fireLevel++;
                level = fireLevel;
                break;
            case Element.Ice:
                iceLevel++;
                level = iceLevel;
                break;
            case Element.Land:
                landLevel++;
                level = landLevel;
                break;
            case Element.Light:
                lightLevel++;
                level = lightLevel;
                break;
        }
        Debug.Log($"{element} level up");
        UpdateElementalEffect(element, level);
    }

    public int GetElementLevel(Element element)
    {
        switch (element)
        {
            case Element.Fire:
                return fireLevel;
            case Element.Ice:
                return iceLevel;
            case Element.Land:
                return landLevel;
            case Element.Light:
                return lightLevel;
            default:
                return 1;
        }
    }
}
