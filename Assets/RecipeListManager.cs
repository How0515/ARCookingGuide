using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 레시피 목록을 표시하고 사용자가 선택할 수 있게 하는 UI 매니저
/// </summary>
public class RecipeListManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject recipeListPanel;           // 레시피 리스트를 표시할 패널
    public GameObject recipeButtonPrefab;        // 레시피 버튼 프리팹
    public Transform recipeListContent;          // ScrollView의 Content (버튼들이 들어갈 부모)
    
    [Header("Other References")]
    public MainMenuController mainMenuController; // 메인 메뉴 컨트롤러
    
    [Header("Auto Create Settings")]
    public bool autoCreateUI = true;             // UI 자동 생성 여부
    
    [Header("Data")]
    private List<Recipe> allRecipes = new List<Recipe>();

    void Awake()
    {
        // UI 자동 생성
        if (autoCreateUI && recipeListPanel == null)
        {
            CreateRecipeListUI();
        }
    }

    void Start()
    {
        // MainMenuController 찾기
        if (mainMenuController == null)
        {
            mainMenuController = FindObjectOfType<MainMenuController>();
            if (mainMenuController != null && recipeListPanel != null)
            {
                mainMenuController.recipeListPanel = recipeListPanel;
            }
        }

        Debug.Log("🔄 레시피 데이터 로드 시작...");
        LoadAllRecipes();
        Debug.Log($"📚 로드된 레시피 수: {allRecipes.Count}");
        
        Debug.Log("🔄 레시피 버튼 생성 시작...");
        ShowRecipeList();
        
        // 초기에는 숨기기 (메인 메뉴가 먼저 보여야 함)
        if (recipeListPanel != null)
        {
            recipeListPanel.SetActive(false);
            Debug.Log("✅ 레시피 리스트 패널 초기 숨김 완료");
        }
    }

    /// <summary>
    /// 레시피 리스트 UI 자동 생성
    /// </summary>
    void CreateRecipeListUI()
    {
        // Canvas 찾기
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ Canvas를 찾을 수 없습니다!");
            return;
        }

        // 레시피 리스트 패널 생성
        recipeListPanel = new GameObject("RecipeListPanel");
        recipeListPanel.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = recipeListPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        
        Image panelImage = recipeListPanel.AddComponent<Image>();
        panelImage.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

        // 타이틀 추가
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(recipeListPanel.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(600, 80);
        titleRect.anchoredPosition = new Vector2(0, -50);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "레시피 선택";
        titleText.fontSize = 48;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;

        // ScrollView 생성
        GameObject scrollViewObj = new GameObject("ScrollView");
        scrollViewObj.transform.SetParent(recipeListPanel.transform, false);
        
        RectTransform scrollRect = scrollViewObj.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0.1f, 0.15f);
        scrollRect.anchorMax = new Vector2(0.9f, 0.85f);
        scrollRect.sizeDelta = Vector2.zero;
        
        Image scrollImage = scrollViewObj.AddComponent<Image>();
        scrollImage.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        
        ScrollRect scrollComponent = scrollViewObj.AddComponent<ScrollRect>();

        // Viewport 생성
        GameObject viewportObj = new GameObject("Viewport");
        viewportObj.transform.SetParent(scrollViewObj.transform, false);
        
        RectTransform viewportRect = viewportObj.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.sizeDelta = Vector2.zero;
        
        Image viewportImage = viewportObj.AddComponent<Image>();
        viewportImage.color = Color.clear;
        
        Mask viewportMask = viewportObj.AddComponent<Mask>();
        viewportMask.showMaskGraphic = false;

        // Content 생성
        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(viewportObj.transform, false);
        
        RectTransform contentRect = contentObj.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.sizeDelta = new Vector2(0, 1000);
        contentRect.anchoredPosition = Vector2.zero;
        
        // Vertical Layout Group 추가
        VerticalLayoutGroup layoutGroup = contentObj.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 10;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;
        
        // Content Size Fitter 추가
        ContentSizeFitter sizeFitter = contentObj.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // ScrollRect 설정
        scrollComponent.content = contentRect;
        scrollComponent.viewport = viewportRect;
        scrollComponent.vertical = true;
        scrollComponent.horizontal = false;

        recipeListContent = contentObj.transform;

        // 뒤로가기 버튼 추가
        GameObject backButtonObj = new GameObject("BackButton");
        backButtonObj.transform.SetParent(recipeListPanel.transform, false);
        
        RectTransform backButtonRect = backButtonObj.AddComponent<RectTransform>();
        backButtonRect.anchorMin = new Vector2(0.5f, 0.05f);
        backButtonRect.anchorMax = new Vector2(0.5f, 0.05f);
        backButtonRect.sizeDelta = new Vector2(200, 60);
        backButtonRect.anchoredPosition = Vector2.zero;
        
        Image backButtonImage = backButtonObj.AddComponent<Image>();
        backButtonImage.color = new Color(0.6f, 0.2f, 0.2f, 1f);
        
        Button backButton = backButtonObj.AddComponent<Button>();
        backButton.onClick.AddListener(() => {
            if (mainMenuController != null)
                mainMenuController.BackToMainMenu();
        });
        
        GameObject backButtonTextObj = new GameObject("Text");
        backButtonTextObj.transform.SetParent(backButtonObj.transform, false);
        
        RectTransform backButtonTextRect = backButtonTextObj.AddComponent<RectTransform>();
        backButtonTextRect.anchorMin = Vector2.zero;
        backButtonTextRect.anchorMax = Vector2.one;
        backButtonTextRect.sizeDelta = Vector2.zero;
        
        TextMeshProUGUI backButtonText = backButtonTextObj.AddComponent<TextMeshProUGUI>();
        backButtonText.text = "뒤로가기";
        backButtonText.fontSize = 24;
        backButtonText.alignment = TextAlignmentOptions.Center;
        backButtonText.color = Color.white;

        Debug.Log("✅ 레시피 리스트 UI 자동 생성 완료");
    }

    /// <summary>
    /// CSV에서 모든 레시피 로드
    /// </summary>
    void LoadAllRecipes()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("RECIPEDB_100");
        if (csvFile == null)
        {
            Debug.LogError("❌ CSV 파일을 찾을 수 없습니다: Resources/RECIPEDB_100");
            return;
        }

        string[] lines = csvFile.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        
        // 첫 줄은 헤더이므로 건너뛰기
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            Recipe recipe = ParseRecipeLine(line);
            if (recipe != null)
            {
                allRecipes.Add(recipe);
            }
        }

        Debug.Log($"✅ 총 {allRecipes.Count}개의 레시피 로드 완료");
    }

    /// <summary>
    /// 레시피 리스트 UI 생성
    /// </summary>
    void ShowRecipeList()
    {
        if (recipeListContent == null)
        {
            Debug.LogError("❌ recipeListContent가 설정되지 않았습니다!");
            return;
        }

        if (allRecipes == null || allRecipes.Count == 0)
        {
            Debug.LogWarning("⚠️ 표시할 레시피가 없습니다!");
            return;
        }

        // 기존 버튼들 제거
        foreach (Transform child in recipeListContent)
        {
            Destroy(child.gameObject);
        }

        Debug.Log($"🔨 {allRecipes.Count}개의 레시피 버튼 생성 중...");

        // 각 레시피마다 버튼 생성
        for (int i = 0; i < allRecipes.Count; i++)
        {
            Recipe recipe = allRecipes[i];
            GameObject buttonObj;

            if (recipeButtonPrefab != null)
            {
                // 프리팹이 있으면 사용
                buttonObj = Instantiate(recipeButtonPrefab, recipeListContent);
            }
            else
            {
                // 프리팹이 없으면 간단한 버튼 생성
                buttonObj = CreateSimpleButton();
                buttonObj.transform.SetParent(recipeListContent, false);
            }

            buttonObj.name = $"RecipeButton_{i}_{recipe.name}";

            // 버튼 텍스트 설정
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = $"{recipe.name}\n({recipe.category})";
            }
            else
            {
                Debug.LogWarning($"⚠️ 버튼 {i}에 TextMeshProUGUI가 없습니다!");
            }

            // 버튼 클릭 이벤트 연결
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                int recipeIndex = i; // 클로저 문제 방지
                button.onClick.AddListener(() => OnRecipeSelected(recipeIndex));
            }
            else
            {
                Debug.LogWarning($"⚠️ 버튼 {i}에 Button 컴포넌트가 없습니다!");
            }
        }

        Debug.Log($"✅ {allRecipes.Count}개의 레시피 버튼 생성 완료!");
    }

    /// <summary>
    /// 프리팹이 없을 때 간단한 버튼 생성
    /// </summary>
    GameObject CreateSimpleButton()
    {
        GameObject buttonObj = new GameObject("RecipeButton");
        
        // RectTransform 설정 - 더 큰 크기로
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(700, 100); // 더 크게
        
        // Layout Element 추가 (VerticalLayoutGroup에서 제대로 작동하도록)
        LayoutElement layoutElement = buttonObj.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 100;
        layoutElement.minHeight = 100;
        
        // Image 컴포넌트 추가 - 더 선명한 색상
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.3f, 0.5f, 0.7f, 1f); // 파란색 계열
        
        // Button 컴포넌트 추가
        Button button = buttonObj.AddComponent<Button>();
        
        // 버튼 색상 전환 설정
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.3f, 0.5f, 0.7f, 1f);
        colors.highlightedColor = new Color(0.4f, 0.6f, 0.8f, 1f);
        colors.pressedColor = new Color(0.2f, 0.4f, 0.6f, 1f);
        button.colors = colors;
        
        // 텍스트 오브젝트 생성
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = new Vector2(10, 10); // 패딩
        textRect.offsetMax = new Vector2(-10, -10); // 패딩
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 28; // 더 큰 폰트
        text.color = Color.white;
        text.fontStyle = FontStyles.Bold; // 볼드체
        
        Debug.Log($"🔨 버튼 생성됨: 크기={rectTransform.sizeDelta}");
        
        return buttonObj;
    }

    /// <summary>
    /// 레시피 선택 시 호출
    /// </summary>
    void OnRecipeSelected(int recipeIndex)
    {
        if (recipeIndex < 0 || recipeIndex >= allRecipes.Count) return;

        Recipe selectedRecipe = allRecipes[recipeIndex];
        Debug.Log($"✅ 레시피 선택: {selectedRecipe.name}");

        // CookingGuideController에 선택된 레시피 전달
        if (CookingGuideController.Instance != null)
        {
            CookingGuideController.Instance.LoadRecipe(selectedRecipe);
        }

        // MainMenuController를 통해 레시피 보드 화면으로 전환
        if (mainMenuController != null)
        {
            mainMenuController.ShowRecipeBoard();
        }
        else
        {
            // MainMenuController가 없으면 직접 패널 숨기기
            if (recipeListPanel != null)
            {
                recipeListPanel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 레시피 리스트 다시 표시
    /// </summary>
    public void ShowRecipeListPanel()
    {
        if (recipeListPanel != null)
        {
            recipeListPanel.SetActive(true);
        }
    }

    // =========================================================
    // CSV 파싱 로직 (CookingGuideController와 동일)
    // =========================================================

    Recipe ParseRecipeLine(string line)
    {
        List<string> cols = ParseCSVLine(line);
        
        if (cols.Count < 6)
        {
            Debug.LogWarning($"⚠️ CSV 데이터가 불완전합니다: {cols.Count}개 필드");
            return null;
        }

        Recipe recipe = new Recipe();
        recipe.recipe_id = cols[0].Trim();
        recipe.name = cols[1].Trim();
        recipe.category = cols[2].Trim();
        recipe.ingredients = cols[3].Trim();
        
        string stepsText = cols[4].Trim();
        string imagesText = cols[5].Trim();

        recipe.steps = ParseSteps(stepsText, imagesText);
        return recipe;
    }

    List<string> ParseCSVLine(string line)
    {
        List<string> fields = new List<string>();
        bool inQuotes = false;
        string currentField = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(currentField);
                currentField = "";
            }
            else
            {
                currentField += c;
            }
        }

        fields.Add(currentField);
        return fields;
    }

    List<RecipeStep> ParseSteps(string stepsText, string imagesText)
    {
        List<RecipeStep> steps = new List<RecipeStep>();
        
        string[] stepLines = stepsText.Split(new string[] { "||" }, System.StringSplitOptions.None);
        string[] imageFiles = imagesText.Split(';');

        for (int i = 0; i < stepLines.Length; i++)
        {
            string stepText = stepLines[i].Trim();
            if (string.IsNullOrEmpty(stepText)) continue;

            RecipeStep step = new RecipeStep();
            step.stepNumber = i + 1;
            step.description = stepText;
            step.timerSeconds = 300f; // 기본값 5분

            if (i < imageFiles.Length)
            {
                step.imageName = imageFiles[i].Trim();
                LoadStepImage(step);
            }

            steps.Add(step);
        }

        return steps;
    }

    void LoadStepImage(RecipeStep step)
    {
        if (string.IsNullOrEmpty(step.imageName)) return;

        string filenameWithoutExtension = Path.GetFileNameWithoutExtension(step.imageName);
        
#if UNITY_EDITOR
        string assetPath = $"Assets/RecipeDB/Images/{step.imageName}";
        if (File.Exists(assetPath))
        {
            step.stepImage = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        }
#else
        step.stepImage = Resources.Load<Texture2D>($"RecipeDB/Images/{filenameWithoutExtension}");
#endif
    }
}
