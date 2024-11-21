using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    public List<GameObject> objectPrefab;   // 풀링할 오브젝트의 프리팹
    public List<GameObject> monster;
    public int poolSize = 350;         // 초기 풀 크기
    public Queue<GameObject> objectPool = new Queue<GameObject>();  // 오브젝트 풀
    bool ispool = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {        // 싱글톤 패턴 적용
        if(SceneManager.GetActiveScene().name == "Test 2")
        {
            Debug.Log(SceneManager.GetActiveScene().name);
            InitializePool();
        }
    }

    private void Update()
    {
        
    }
    // 풀 초기화 메서드
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            int rand = Random.Range(0, objectPrefab.Count);
            GameObject obj = Instantiate(objectPrefab[rand]);
            obj.SetActive(false); // 비활성화 상태로 초기화
            objectPool.Enqueue(obj);
        }
    }

    // 풀에서 오브젝트 가져오기
    public GameObject GetObjectFromPool(Vector3 pos)
    {
        if (objectPool.Count > 0)
        {
            GameObject obj = objectPool.Dequeue();
            monster.Add(obj);
            obj.transform.position = pos;
            // obj.SetActive(true);
            return obj;
        }
        else
        {
            // 풀에 더 이상 오브젝트가 없으면 새로운 오브젝트 생성
            int rand = Random.Range(0,objectPrefab.Count);
            GameObject obj = Instantiate(objectPrefab[rand],transform.position, Quaternion.identity);
            monster.Add(obj);
            obj.transform.position = pos;

            // obj.SetActive(true);
            return obj;
        }
    }
    // 오브젝트 풀에 반환하기
    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }
    public void Reset_Monster()
    {
        for(int i = 0 ; i < monster.Count;i++)
        {
            Destroy(monster[i]);
        }
        monster.Clear();
        objectPool.Clear();
        if(SceneManager.GetActiveScene().name == "Test 2")
            InitializePool();
    }
}

public static class ElementRelations
{
    private static Dictionary<Element, Dictionary<Element, float>> damageMultiplier = new Dictionary<Element, Dictionary<Element, float>>
    {
        // 위에서 정의한 damageMultiplier를 그대로 사용
        {
            Element.None, new Dictionary<Element, float>
            {
                { Element.None, 1.0f },
                { Element.Fire, 1.0f },
                { Element.Ice, 1.0f },
                { Element.Land, 1.0f },
                { Element.Light, 1.0f }
            }
        },
        {
            Element.Fire, new Dictionary<Element, float>
            {
                { Element.None, 1.0f },
                { Element.Fire, 1.0f },
                { Element.Ice, 1.25f },
                { Element.Land, 0.5f },
                { Element.Light, 1.0f }
            }
        },
        {
            Element.Ice, new Dictionary<Element, float>
            {
                { Element.None, 1.0f },
                { Element.Fire, 0.5f },
                { Element.Ice, 1.0f },
                { Element.Land, 1.0f },
                { Element.Light, 1.25f }
            }
        },
        {
            Element.Land, new Dictionary<Element, float>
            {
                { Element.None, 1.0f },
                { Element.Fire, 1.25f },
                { Element.Ice, 1.0f },
                { Element.Land, 1.0f },
                { Element.Light, 0.5f }
            }
        },
        {
            Element.Light, new Dictionary<Element, float>
            {
                { Element.None, 1.0f },
                { Element.Fire, 1.0f },
                { Element.Ice, 0.5f },
                { Element.Land, 1.25f },
                { Element.Light, 1.0f }
            }
        }
    };

    private static List<Element> GetOppositeElements(Element playerElement)
    {
        List<Element> oppositeElements = new List<Element>();
        var multipliers = damageMultiplier[playerElement];
        foreach (var kvp in multipliers)
        {
            if (kvp.Value == 0.5f)
            {
                oppositeElements.Add(kvp.Key);
            }
        }
        return oppositeElements;
    }
    private static List<Element> GetAdvantageousElements(Element playerElement)
    {
        List<Element> advantageousElements = new List<Element>();
        var multipliers = damageMultiplier[playerElement];
        foreach (var kvp in multipliers)
        {
            if (kvp.Value == 1.25f)
            {
                advantageousElements.Add(kvp.Key);
            }
        }
        return advantageousElements;
    }

    private static Element GetRandomElementBasedOnPlayer(Element playerElement)
    {
        float randomValue = Random.Range(0f, 100f);

        if (randomValue < 40f)
        {
            // 40% 확률로 반대되는 속성 선택
            var oppositeElements = GetOppositeElements(playerElement);
            if (oppositeElements.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, oppositeElements.Count);
                return oppositeElements[index];
            }
            else
            {
                // 반대되는 속성이 없으면 플레이어의 속성을 반환
                return playerElement;
            }
        }
        else if (randomValue < 70f)
        {
            // 다음 30% 확률로 동일한 속성 선택
            return playerElement;
        }
        else
        {
            // 나머지 30% 확률로 유리한 속성 선택
            var advantageousElements = GetAdvantageousElements(playerElement);
            if (advantageousElements.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, advantageousElements.Count);
                return advantageousElements[index];
            }
            else
            {
                // 유리한 속성이 없으면 플레이어의 속성을 반환
                return playerElement;
            }
        }
    }
    // 플레이어 레벨에 따른 속성 레벨 결정
    private static int GetElementLevelBasedOnPlayerLevel(int playerLevel)
    {
        if (playerLevel >= 1 && playerLevel <= 3)
        {
            return 1; // 플레이어 레벨 1~3 -> 속성 레벨 1
        }
        else if (playerLevel >= 4 && playerLevel <= 6)
        {
            return 2; // 플레이어 레벨 4~6 -> 속성 레벨 2
        }
        else if (playerLevel >= 7 && playerLevel <= 9)
        {
            return 3; // 플레이어 레벨 7~9 -> 속성 레벨 3
        }
        else
        {
            Debug.LogWarning("잘못된 플레이어 레벨입니다. 기본 값으로 속성 레벨 1을 반환합니다.");
            return 1;
        }
    }

    // 몬스터 속성을 생성할 때 사용: 플레이어의 레벨과 속성을 기반으로 속성 및 레벨 결정
    public static (Element, int) GenerateMonsterElementAndLevel(Element playerElement, int playerLevel)
    {
        Element monsterElement = GetRandomElementBasedOnPlayer(playerElement);
        int elementLevel = GetElementLevelBasedOnPlayerLevel(playerLevel);

        return (monsterElement, elementLevel);
    }

}