using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] monsterPrefabs;
    public GameObject healthBarPrefab;
    private int selectedMonsterIndex = -1;  // 기본값을 유효하지 않은 값으로 설정
    private Element selectedElement = Element.None;
    private int selectedElementLevel = 1;

    public Button[] monsterButtons;
    public Button[] elementButtons;
    public Button[] elementLevelButtons;
    public Button spawnButton;

    private Color activeColor = Color.green;
    private Color inactiveColor = Color.white;

    void Start()
    {
        AssignButtons();
        SetButton();
    }

  
    private void AssignButtons()
    {
        GameObject canvas = GameObject.Find("SpawnerUI");
        if (canvas == null)
        {
            return;
        }

        Transform monsterUI = canvas.transform.Find("Monster");
        monsterButtons = monsterUI?.GetComponentsInChildren<Button>();

        Transform elementUI = canvas.transform.Find("Elementeal");
        elementButtons = elementUI?.GetComponentsInChildren<Button>();

        Transform levelUI = canvas.transform.Find("Level");
        elementLevelButtons = levelUI?.GetComponentsInChildren<Button>();

        spawnButton = canvas.transform.Find("Spawn").GetComponent<Button>();
    }
    private void SetButton()
    {
        GameObject canvas = GameObject.Find("SpawnerUI");
        if (canvas != null)
        {
            for (int i = 0; i < monsterButtons.Length; i++)
            {
                int index = i;  // 클로저 문제를 피하기 위해 지역 변수로 선언
                monsterButtons[i].onClick.AddListener(() => SelectMonster(index));
            }

            // 속성 버튼 클릭 시 속성 선택
            elementButtons[0].onClick.AddListener(() => SelectElement(Element.Fire));
            elementButtons[1].onClick.AddListener(() => SelectElement(Element.Ice));
            elementButtons[2].onClick.AddListener(() => SelectElement(Element.Land));
            elementButtons[3].onClick.AddListener(() => SelectElement(Element.Light));

            // 속성 레벨 버튼 클릭 시 레벨 선택
            elementLevelButtons[0].onClick.AddListener(() => SelectElementLevel(1));
            elementLevelButtons[1].onClick.AddListener(() => SelectElementLevel(2));
            elementLevelButtons[2].onClick.AddListener(() => SelectElementLevel(3));

            // 소환 버튼 클릭 시 몬스터 소환
            spawnButton.onClick.AddListener(SpawnSelectedMonster);
        }
        
    }

    private void SelectMonster(int index)
    {
        selectedMonsterIndex = index;
        Debug.Log($"Selected Monster Index: {index}");

        foreach (Button btn in monsterButtons)
        {
            btn.image.color = inactiveColor;
        }
        if (index >= 0 && index < monsterButtons.Length)
        {
            monsterButtons[index].image.color = activeColor;  // 선택된 버튼을 녹색으로 변경
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void SelectElement(Element element)
    {
        selectedElement = element;
        Debug.Log($"Selected Element: {element}");

        foreach (Button btn in elementButtons)
        {
            btn.image.color = inactiveColor;
        }

        int elementIndex = (int)element - 1;

        if (elementIndex >= 0 && elementIndex < elementButtons.Length)
        {
            elementButtons[elementIndex].image.color = activeColor;
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            Debug.LogError("Element index out of bounds!");
        }
    }
    private void SelectElementLevel(int level)
    {
        selectedElementLevel = level;
        Debug.Log($"Selected Element Level: {level}");

        foreach (Button btn in elementLevelButtons)
        {
            btn.image.color = inactiveColor;
        }
        elementLevelButtons[level - 1].image.color = activeColor;
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void SpawnSelectedMonster()
    {
        if (selectedMonsterIndex < 0 || selectedMonsterIndex >= monsterPrefabs.Length)
        {
            Debug.Log("유효하지 않은 몬스터 인덱스입니다.");
            return;
        }

        if (selectedElement == Element.None)
        {
            Debug.Log("속성이 선택되지 않았습니다.");
            return;
        }
        EventSystem.current.SetSelectedGameObject(null);
        GameObject monsterPrefab = monsterPrefabs[selectedMonsterIndex];
        GameObject monsterInstance = Instantiate(monsterPrefab, transform.position, Quaternion.identity);

        // 몬스터의 속성 설정
        EnemyStats enemyStats = monsterInstance.GetComponentInChildren<EnemyStats>();
        if (enemyStats != null)
        {
            enemyStats.ChangeElement(selectedElement, selectedElementLevel);
            Debug.Log($"Spawned Monster with Element: {selectedElement}");
        }
        else
        {
            Debug.LogError("EnemyStats component not found on the monster prefab.");
        }
        selectedMonsterIndex = -1;

    }


}