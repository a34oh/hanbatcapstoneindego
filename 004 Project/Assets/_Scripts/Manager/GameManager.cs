using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    static GameManager s_instance;
    public static GameManager Instance { get { Init(); return s_instance; } }


    DataManager data = new DataManager();
    ResourceManager resource = new ResourceManager();
    PlayerManager player;
    SharedCombatDataManager sharedData = new SharedCombatDataManager();
    ElementalManager elemental = new ElementalManager();
    SoundManager soundManager = new SoundManager();
    private GameObject pauseMenuCanvas;
    private bool isPaused = false;

    
    public static DataManager Data { get{ return Instance.data; } }
    public static ResourceManager Resource { get { return Instance.resource; } }
    public static PlayerManager PlayerManager { get { return Instance.player; } }
    public static SharedCombatDataManager SharedCombatDataManager { get { return Instance.sharedData; } }
    public static ElementalManager ElementalManager { get { return Instance.elemental; } }
    public static SoundManager SoundManager { get { return Instance.soundManager; } }
    void Start()
    {
        Init();
        pauseMenuCanvas = new GameObject("PauseMenuCanvas");
        Canvas canvas = pauseMenuCanvas.AddComponent<Canvas>();
        pauseMenuCanvas.AddComponent<CanvasScaler>();
        pauseMenuCanvas.AddComponent<GraphicRaycaster>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        DontDestroyOnLoad(canvas);
        // 초기 상태: 비활성화
        pauseMenuCanvas.SetActive(false);

        // Resume 버튼 생성
        CreateButton("Resume", new Vector2(0, 75), () => TogglePause());
        CreateButton("Title", new Vector2(0, 0), () => BackToTitle());
        // Quit 버튼 생성
        CreateButton("Quit", new Vector2(0, -75), () => QuitGame());
    }

    void Update()
    {
        // ESC 키 입력 시 일시중지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<GameManager>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<GameManager>();

            s_instance.data.Init();

            s_instance.soundManager.LoadAllSounds();
        }
        
    }

    public void CreatePlayerManager(Vector3? position = null)
    {
        if (PlayerManager == null)
        {
            GameObject go = new GameObject("PlayerManager") { name = "@PlayerManager"};

            player = go.AddComponent<PlayerManager>();
            PlayerManager.Initialize(position);
        }
    }
    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // 게임 일시중지
            pauseMenuCanvas.SetActive(true); // 메뉴 활성화
        }
        else
        {
            Time.timeScale = 1f; // 게임 재개
            pauseMenuCanvas.SetActive(false); // 메뉴 비활성화
        }
    }
    void BackToTitle()
    {
        isPaused = !isPaused;
        Time.timeScale = 1f; // 게임 재개
        pauseMenuCanvas.SetActive(false); // 메뉴 비활성화
        SceneManager.LoadScene("Title");
    }
    // 게임 종료
    void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    // 버튼 생성 함수
    void CreateButton(string buttonText, Vector2 position, UnityEngine.Events.UnityAction onClickAction)
    {
        // 버튼 오브젝트 생성
        GameObject buttonObject = new GameObject(buttonText);
        buttonObject.transform.SetParent(pauseMenuCanvas.transform);

        // 버튼 RectTransform 설정
        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(160, 40);
        rectTransform.anchoredPosition = position;

        // 버튼 UI 컴포넌트 추가
        Button button = buttonObject.AddComponent<Button>();
        Image image = buttonObject.AddComponent<Image>();
        image.color = Color.gray; // 버튼 배경색 설정
        button.targetGraphic = image;

        // 버튼 텍스트 생성
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform);
        Text text = textObject.AddComponent<Text>();
        text.text = buttonText;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        text.color = Color.black;

        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.sizeDelta = rectTransform.sizeDelta;
        textRect.anchoredPosition = Vector2.zero;

        // 버튼 클릭 이벤트 추가
        button.onClick.AddListener(onClickAction);
    }
}

