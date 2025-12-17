using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

/// <summary>
/// 레시피 데이터를 CookingGuideController에 할당하는 초기화 스크립트
/// - RecipeDataManager에서 데이터 가져오기
/// - CookingGuideController.stepList에 할당
/// - 레시피 변경 시 상태 초기화 처리
/// </summary>
public class RecipeInitializer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CookingGuideController cookingController;

    [Header("Recipe Settings")]
    [SerializeField] private string selectedRecipeName = "계란볶음밥";

    private void Start()
    {
        // CookingGuideController를 찾지 못했으면 자동으로 찾기
        if (cookingController == null)
        {
            cookingController = GetComponent<CookingGuideController>();
            
            if (cookingController == null)
            {
                Debug.LogError("❌ CookingGuideController를 찾을 수 없습니다!");
                return;
            }
        }

        InitializeRecipe();
    }

    /// <summary>
    /// RecipeDataManager에서 선택된 레시피를 로드
    /// CookingGuideController.stepList에 할당
    /// 
    /// csv 파일 형식과 호환성 부족
    /// </summary>
    public void InitializeRecipe()
    {
        // 1. RecipeDataManager에서 데이터 가져오기
        var recipeSteps = RecipeDataManager.Instance.GetRecipeSteps(selectedRecipeName);

        if (recipeSteps.Count == 0)
        {
            Debug.LogError($"❌ '{selectedRecipeName}' 레시피를 찾을 수 없습니다!");
            return;
        }

        Debug.Log($"🔍 '{selectedRecipeName}' 로드: {recipeSteps.Count}단계");
        for (int i = 0; i < recipeSteps.Count; i++)
        {
            Debug.Log($"  Step {i}: {recipeSteps[i].title}");
        }

        // 2. RecipeDataManager.CookingStep을 CookingGuideController.CookingStep으로 변환
        List<CookingGuideController.CookingStep> convertedSteps = 
            new List<CookingGuideController.CookingStep>();

        foreach (var step in recipeSteps)
        {
            var convertedStep = new CookingGuideController.CookingStep
            {
                title = step.title,
                ingredients = step.ingredients,
                heatLevel = step.heatLevel,
                timerSeconds = step.timerSeconds,
                videoClip = null  // CSV에는 영상이 없으므로 null
            };
            convertedSteps.Add(convertedStep);
        }

        // 3. CookingGuideController.stepList에 할당
        cookingController.stepList = convertedSteps;

        // 4. private 변수들 Reflection으로 초기화
        var type = cookingController.GetType();
        type.GetField("currentStepIndex", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cookingController, 0);
        type.GetField("currentTimer", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cookingController, 0f);
        type.GetField("isTimerRunning", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(cookingController, false);
        
        // 5. UI 업데이트
        type.GetMethod("UpdateUI", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(cookingController, null);

        Debug.Log($"✅ '{selectedRecipeName}' 레시피 로드 완료 ({convertedSteps.Count}단계)");
    }

    /// <summary>
    /// 다른 레시피로 변경 (버튼에서 호출 가능)
    /// </summary>
    public void ChangeRecipe(string recipeName)
    {
        selectedRecipeName = recipeName;
        InitializeRecipe();
    }
}
