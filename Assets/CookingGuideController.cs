using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingGuideController : MonoBehaviour
{
    public static CookingGuideController Instance;

    [Header("Data")]
    public Recipe currentRecipe;           // 현재 선택된 레시피
    public int currentStepIndex = 0;

    [Header("References")]
    public RecipeBoard recipeBoard;        // 중앙 메인 보드
    public HeadHUD headHUD;                // 우측 상단 HUD
    public TimerManager timerManager;      // 좌측 타이머 관리자

    [Header("UI References")]
    public TextMeshProUGUI recipeTitleText;     // 레시피 제목 (예: 매콤 된장찌개)
    public TextMeshProUGUI ingredientsText;     // 재료 목록
    public TextMeshProUGUI stepDescriptionText; // 단계 설명
    public RawImage stepImage;                  // 단계 이미지 표시
    public TextMeshProUGUI progressText;        // 진행도 (예: Step 1 of 4)

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // UI 참조 자동 연결 시도
        AutoConnectUIReferences();
    }

    private void Start()
    {
        LoadRecipeFromCSV();
        if (currentRecipe != null && currentRecipe.steps.Count > 0)
        {
            UpdateAllUI();
        }
        else
        {
            Debug.LogError("❌ 레시피 로드 실패 또는 빈 데이터");
        }
    }

    /// <summary>
    /// Scene에서 UI 요소들을 자동으로 찾아 연결
    /// </summary>
    void AutoConnectUIReferences()
    {
        // RecipeBoard 찾기
        if (recipeBoard == null)
        {
            recipeBoard = FindObjectOfType<RecipeBoard>();
        }

        // HeadHUD 찾기
        if (headHUD == null)
        {
            headHUD = FindObjectOfType<HeadHUD>();
        }

        // TimerManager 찾기
        if (timerManager == null)
        {
            timerManager = FindObjectOfType<TimerManager>();
        }

        // TextMeshPro UI 요소들 찾기 (정확한 이름으로 검색)
        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
        
        foreach (var text in allTexts)
        {
            string name = text.gameObject.name;
            
            // StepTitleText → 레시피 제목
            if (recipeTitleText == null && name == "StepTitleText")
            {
                recipeTitleText = text;
                Debug.Log($"✅ Recipe Title Text 연결: {name}");
            }
            
            // StepIndexText → 진행도
            if (progressText == null && name == "StepIndexText")
            {
                progressText = text;
                Debug.Log($"✅ Progress Text 연결: {name}");
            }
        }

        // NextButton 아래의 Text 찾기 (stepDescriptionText)
        if (stepDescriptionText == null)
        {
            GameObject nextButton = GameObject.Find("NextButton");
            if (nextButton != null)
            {
                TextMeshProUGUI buttonText = nextButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    stepDescriptionText = buttonText;
                    Debug.Log($"✅ Step Description Text 연결: NextButton/Text");
                }
            }
        }

        // RightScreen RawImage 찾기
        if (stepImage == null)
        {
            GameObject rightScreen = GameObject.Find("RightScreen");
            if (rightScreen != null)
            {
                stepImage = rightScreen.GetComponent<RawImage>();
                if (stepImage != null)
                {
                    Debug.Log($"✅ Step Image 연결: RightScreen");
                }
            }
        }

        Debug.Log($"📋 UI 참조 연결 결과:\n" +
                  $"  RecipeBoard: {recipeBoard != null}\n" +
                  $"  HeadHUD: {headHUD != null}\n" +
                  $"  TimerManager: {timerManager != null}\n" +
                  $"  RecipeTitleText: {recipeTitleText != null}\n" +
                  $"  IngredientsText: {ingredientsText != null}\n" +
                  $"  StepDescriptionText: {stepDescriptionText != null}\n" +
                  $"  StepImage: {stepImage != null}\n" +
                  $"  ProgressText: {progressText != null}");
    }

    /// <summary>
    /// CSV 파일에서 레시피 데이터 로드
    /// </summary>
    void LoadRecipeFromCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("RECIPEDB_100");
        if (csvFile == null)
        {
            Debug.LogError("❌ CSV 파일을 찾을 수 없습니다: Resources/RECIPEDB_100");
            return;
        }

        string[] lines = csvFile.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        
        if (lines.Length < 2)
        {
            Debug.LogError("❌ CSV 파일이 비어있습니다");
            return;
        }

        // 두 번째 레시피 로드 (첫 번째는 이미지 없음)
        string line = lines[2].Trim();
        if (string.IsNullOrEmpty(line)) return;

        currentRecipe = ParseRecipeLine(line);
        if (currentRecipe != null)
        {
            Debug.Log($"✅ 레시피 로드 완료: {currentRecipe.name} ({currentRecipe.steps.Count} 단계)");
        }
    }

    /// <summary>
    /// CSV 한 줄을 파싱해서 Recipe 객체 생성
    /// CSV 포맷: recipe_id, name, category, ingredients, steps, images
    /// </summary>
    Recipe ParseRecipeLine(string line)
    {
        // CSV 파싱 (큰따옴표로 묶인 필드 고려)
        List<string> cols = ParseCSVLine(line);
        
        if (cols.Count < 6)
        {
            Debug.LogError($"❌ CSV 데이터가 불완전합니다: {cols.Count}개 필드");
            return null;
        }

        Recipe recipe = new Recipe();
        recipe.recipe_id = cols[0].Trim();
        recipe.name = cols[1].Trim();
        recipe.category = cols[2].Trim();
        recipe.ingredients = cols[3].Trim();
        
        // 단계 파싱 (|| 로 구분)
        string stepsText = cols[4].Trim();
        string imagesText = cols[5].Trim();

        recipe.steps = ParseSteps(stepsText, imagesText);
        return recipe;
    }

    /// <summary>
    /// CSV 라인을 큰따옴표 고려하여 파싱
    /// </summary>
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

        // 마지막 필드 추가
        fields.Add(currentField);
        return fields;
    }

    /// <summary>
    /// 단계 텍스트와 이미지를 파싱해서 RecipeStep 리스트 생성
    /// </summary>
    List<RecipeStep> ParseSteps(string stepsText, string imagesText)
    {
        List<RecipeStep> steps = new List<RecipeStep>();
        
        // || 로 구분된 단계들 파싱
        string[] stepLines = stepsText.Split(new string[] { "||" }, System.StringSplitOptions.None);
        string[] imageFiles = imagesText.Split(';');

        for (int i = 0; i < stepLines.Length; i++)
        {
            string stepText = stepLines[i].Trim();
            if (string.IsNullOrEmpty(stepText)) continue;

            RecipeStep step = new RecipeStep();
            step.stepNumber = i + 1;
            step.description = stepText;
            step.timerSeconds = 300f; // 기본값 5분 (300초)

            // 해당 단계의 이미지 로드
            if (i < imageFiles.Length)
            {
                step.imageName = imageFiles[i].Trim();
                LoadStepImage(step);
            }

            steps.Add(step);
        }

        return steps;
    }

    /// <summary>
    /// 단계 이미지 로드 (Assets/RecipeDB/Images 폴더에서)
    /// </summary>
    void LoadStepImage(RecipeStep step)
    {
        if (string.IsNullOrEmpty(step.imageName)) return;

        // 파일명에서 확장자 제거
        string filenameWithoutExtension = Path.GetFileNameWithoutExtension(step.imageName);
        
        // 런타임: Resources 폴더에 이미지가 있어야 함
        // 에디터: Assets/RecipeDB/Images에서 로드
#if UNITY_EDITOR
        string assetPath = $"Assets/RecipeDB/Images/{step.imageName}";
        if (File.Exists(assetPath))
        {
            step.stepImage = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        }
        else
        {
            Debug.LogWarning($"⚠️ 파일을 찾을 수 없습니다: {assetPath}");
        }
#else
        // 런타임: Resources에서 로드
        step.stepImage = Resources.Load<Texture2D>($"RecipeDB/Images/{filenameWithoutExtension}");
        if (step.stepImage == null)
        {
            Debug.LogWarning($"⚠️ 이미지를 로드할 수 없습니다: RecipeDB/Images/{filenameWithoutExtension}");
        }
#endif
    }

    // =========================================================
    // UI 업데이트 로직
    // =========================================================

    /// <summary>
    /// 모든 UI 갱신
    /// </summary>
    void UpdateAllUI()
    {
        if (currentRecipe == null || currentRecipe.steps.Count == 0) return;

        RecipeStep currentStep = currentRecipe.steps[currentStepIndex];

        // 레시피 제목
        if (recipeTitleText != null)
        {
            recipeTitleText.text = currentRecipe.name;
        }

        // 재료 목록
        if (ingredientsText != null)
        {
            ingredientsText.text = currentRecipe.ingredients;
        }

        // 단계 설명 (타이머 안내 텍스트)
        if (stepDescriptionText != null)
        {
            stepDescriptionText.text = "타이머 시작";
        }

        // 단계 이미지
        if (stepImage != null)
        {
            if (currentStep.stepImage != null)
            {
                // RawImage Material을 null로 설정 (기본 UI Material 사용)
                stepImage.material = null;
                
                stepImage.texture = currentStep.stepImage;
                stepImage.gameObject.SetActive(true);
            }
            else
            {
                stepImage.gameObject.SetActive(false);
            }
        }

        // 진행도
        if (progressText != null)
        {
            progressText.text = $"Step {currentStepIndex + 1} of {currentRecipe.steps.Count}";
        }

        // RecipeBoard 업데이트
        if (recipeBoard != null)
        {
            recipeBoard.UpdateBoard(currentRecipe, currentStep);
        }

        // HUD 업데이트
        if (headHUD != null)
        {
            float progress = (float)(currentStepIndex + 1) / currentRecipe.steps.Count;
            headHUD.UpdateHUD(currentStepIndex + 1, currentRecipe.steps.Count, 
                            currentRecipe.name, progress);
        }
    }

    /// <summary>
    /// 다음 단계로 이동
    /// </summary>
    public void GoToNextStep()
    {
        if (currentRecipe == null || currentRecipe.steps.Count == 0) return;

        if (currentStepIndex < currentRecipe.steps.Count - 1)
        {
            currentStepIndex++;
            UpdateAllUI();
        }
        else
        {
            Debug.Log("✅ 요리 완료!");
        }
    }

    /// <summary>
    /// 이전 단계로 이동
    /// </summary>
    public void GoToPrevStep()
    {
        if (currentStepIndex > 0)
        {
            currentStepIndex--;
            UpdateAllUI();
        }
    }

    /// <summary>
    /// 특정 단계로 이동
    /// </summary>
    public void GoToStep(int stepIndex)
    {
        if (currentRecipe == null || currentRecipe.steps.Count == 0) return;

        if (stepIndex >= 0 && stepIndex < currentRecipe.steps.Count)
        {
            currentStepIndex = stepIndex;
            UpdateAllUI();
        }
    }
}
