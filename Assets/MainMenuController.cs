using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 메인 메뉴 화면 - 앱 시작 시 표시되는 첫 화면
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mainMenuPanel;         // 메인 메뉴 패널
    public Button startButton;               // 레시피 선택 시작 버튼
    public TextMeshProUGUI titleText;        // 앱 제목 텍스트
    
    [Header("Other Panels")]
    public GameObject recipeListPanel;       // 레시피 리스트 패널 (RecipeListManager)
    public GameObject recipeBoardCanvas;     // 레시피 보드 캔버스 (실제 요리 화면)

    [Header("Auto Create Settings")]
    public bool autoCreateUI = true;         // UI 자동 생성 여부

    void Awake()
    {
        // UI 자동 생성
        if (autoCreateUI && mainMenuPanel == null)
        {
            CreateMainMenuUI();
        }

        // 시작 버튼 클릭 이벤트 연결
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        // 타이틀 설정
        if (titleText != null)
        {
            titleText.text = "AR Cooking Guide";
        }
    }

    void Start()
    {
        // 다른 패널들 찾기
        if (recipeListPanel == null)
        {
            RecipeListManager recipeManager = FindObjectOfType<RecipeListManager>();
            if (recipeManager != null)
            {
                recipeListPanel = recipeManager.recipeListPanel;
            }
        }

        if (recipeBoardCanvas == null)
        {
            GameObject canvas = GameObject.Find("RecipeBoardCanvas");
            if (canvas != null)
            {
                recipeBoardCanvas = canvas;
            }
        }

        // 시작 시 메인 메뉴만 표시
        ShowMainMenu();
    }

    /// <summary>
    /// 메인 메뉴 UI 자동 생성
    /// </summary>
    void CreateMainMenuUI()
    {
        // Canvas 찾기 또는 생성
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // 메인 메뉴 패널 생성
        mainMenuPanel = new GameObject("MainMenuPanel");
        mainMenuPanel.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = mainMenuPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        
        Image panelImage = mainMenuPanel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);

        // 타이틀 텍스트 생성
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(mainMenuPanel.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.7f);
        titleRect.sizeDelta = new Vector2(600, 100);
        titleRect.anchoredPosition = Vector2.zero;
        
        titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "AR Cooking Guide";
        titleText.fontSize = 60;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;

        // 시작 버튼 생성
        GameObject buttonObj = new GameObject("StartButton");
        buttonObj.transform.SetParent(mainMenuPanel.transform, false);
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.4f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.4f);
        buttonRect.sizeDelta = new Vector2(300, 80);
        buttonRect.anchoredPosition = Vector2.zero;
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 0.2f, 1f);
        
        startButton = buttonObj.AddComponent<Button>();
        
        // 버튼 텍스트 생성
        GameObject buttonTextObj = new GameObject("Text");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.sizeDelta = Vector2.zero;
        
        TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "레시피 선택하기";
        buttonText.fontSize = 32;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;

        Debug.Log("✅ 메인 메뉴 UI 자동 생성 완료");
    }

    /// <summary>
    /// 메인 메뉴 표시
    /// </summary>
    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (recipeListPanel != null)
            recipeListPanel.SetActive(false);

        if (recipeBoardCanvas != null)
            recipeBoardCanvas.SetActive(false);

        Debug.Log("✅ 메인 메뉴 표시");
    }

    /// <summary>
    /// 시작 버튼 클릭 시
    /// </summary>
    void OnStartButtonClicked()
    {
        Debug.Log("✅ 시작 버튼 클릭 - 레시피 리스트로 이동");

        // 메인 메뉴 숨기기
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        // 레시피 리스트 표시
        if (recipeListPanel != null)
            recipeListPanel.SetActive(true);
    }

    /// <summary>
    /// 레시피 선택 후 요리 화면으로 전환 (RecipeListManager에서 호출)
    /// </summary>
    public void ShowRecipeBoard()
    {
        if (recipeListPanel != null)
            recipeListPanel.SetActive(false);

        if (recipeBoardCanvas != null)
            recipeBoardCanvas.SetActive(true);

        Debug.Log("✅ 레시피 보드 화면 표시");
    }

    /// <summary>
    /// 레시피 리스트로 돌아가기
    /// </summary>
    public void BackToRecipeList()
    {
        if (recipeBoardCanvas != null)
            recipeBoardCanvas.SetActive(false);

        if (recipeListPanel != null)
            recipeListPanel.SetActive(true);

        Debug.Log("✅ 레시피 리스트로 복귀");
    }

    /// <summary>
    /// 메인 메뉴로 돌아가기
    /// </summary>
    public void BackToMainMenu()
    {
        ShowMainMenu();
    }
}
