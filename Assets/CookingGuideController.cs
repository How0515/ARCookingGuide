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
        // 자동 로드 제거 - RecipeListManager에서 선택 후 LoadRecipe() 호출됨
        Debug.Log("✅ CookingGuideController 준비 완료 - 레시피 선택 대기 중...");
    }

    /// <summary>
    /// 외부에서 레시피를 로드 (RecipeListManager에서 호출)
    /// </summary>
    public void LoadRecipe(Recipe recipe)
    {
        if (recipe == null)
        {
            Debug.LogError("❌ 전달받은 레시피가 null입니다");
            return;
        }

        currentRecipe = recipe;
        currentStepIndex = 0;

        Debug.Log($"✅ 레시피 로드: {recipe.name} ({recipe.steps.Count} 단계)");
        
        if (recipe.steps.Count > 0)
        {
            UpdateAllUI();
        }
        else
        {
            Debug.LogError("❌ 레시피에 단계가 없습니다");
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

    // =========================================================
    // CSV 파싱 로직은 RecipeListManager로 이동
    // =========================================================

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
