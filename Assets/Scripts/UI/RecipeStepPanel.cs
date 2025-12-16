using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 요리 가이드를 패널형 UI로 관리하는 컨트롤러
/// - 월드 스페이스에 고정
/// - 단계별 패널로 표시
/// - 각 단계는 독립적으로 드래그 가능
/// </summary>
public class RecipeStepPanel : MonoBehaviour
{
    [System.Serializable]
    public struct CookingStep
    {
        public string title;
        [TextArea] public string ingredients;
        public string heatLevel;
        public float timerSeconds;
        public Sprite stepImage;
        public string description;
    }

    [Header("Recipe Information")]
    [SerializeField] private string recipeName = "파스타 요리하기";
    [SerializeField] private List<CookingStep> stepList = new List<CookingStep>();

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private TextMeshProUGUI ingredientsText;
    [SerializeField] private Transform stepsContainer; // 단계들을 담을 컨테이너
    [SerializeField] private Transform progressDisplay; // 진행률 표시 (1/6 같은)

    [Header("UI Prefab")]
    [SerializeField] private GameObject stepItemPrefab; // 단계 아이템 프리팹

    [Header("Panel Settings")]
    [SerializeField] private CanvasGroup canvasGroup; // 투명도 조절용
    [SerializeField] private float defaultAlpha = 0.9f;

    private List<RecipeStepItem> stepItems = new List<RecipeStepItem>();

    private void Start()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup != null)
            canvasGroup.alpha = defaultAlpha;

        InitializeUI();
    }

    /// <summary>
    /// UI 초기화 - 패널을 생성합니다
    /// </summary>
    private void InitializeUI()
    {
        // 레시피 이름과 재료 설정
        if (recipeNameText != null)
            recipeNameText.text = recipeName;

        if (ingredientsText != null && stepList.Count > 0)
            ingredientsText.text = stepList[0].ingredients;

        // 단계 아이템 생성
        if (stepsContainer != null && stepItemPrefab != null)
        {
            foreach (var step in stepList)
            {
                GameObject itemObj = Instantiate(stepItemPrefab, stepsContainer);
                RecipeStepItem stepItem = itemObj.GetComponent<RecipeStepItem>();
                
                if (stepItem != null)
                {
                    stepItem.Initialize(step, stepItems.Count + 1);
                    stepItems.Add(stepItem);
                }
            }
        }

        UpdateProgressDisplay();
    }

    /// <summary>
    /// 진행률 표시 업데이트 (예: 1/6)
    /// </summary>
    private void UpdateProgressDisplay()
    {
        if (progressDisplay != null)
        {
            TextMeshProUGUI progressText = progressDisplay.GetComponentInChildren<TextMeshProUGUI>();
            if (progressText != null)
            {
                int completedCount = stepItems.FindAll(s => s.IsCompleted).Count;
                progressText.text = $"{completedCount}/{stepList.Count}";
            }
        }
    }

    /// <summary>
    /// 특정 단계 완료 처리
    /// </summary>
    public void CompleteStep(int stepIndex)
    {
        if (stepIndex >= 0 && stepIndex < stepItems.Count)
        {
            stepItems[stepIndex].SetCompleted(true);
            UpdateProgressDisplay();
        }
    }

    /// <summary>
    /// 투명도 설정
    /// </summary>
    public void SetAlpha(float alpha)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = Mathf.Clamp01(alpha);
    }

    /// <summary>
    /// 현재 투명도 반환
    /// </summary>
    public float GetAlpha()
    {
        return canvasGroup != null ? canvasGroup.alpha : 1f;
    }

    /// <summary>
    /// 패널 회전 (Y축 중심)
    /// </summary>
    public void RotatePanelY(float angle)
    {
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    /// <summary>
    /// 모든 단계 데이터 반환
    /// </summary>
    public List<CookingStep> GetSteps()
    {
        return stepList;
    }

    /// <summary>
    /// 모든 단계 아이템 반환
    /// </summary>
    public List<RecipeStepItem> GetStepItems()
    {
        return stepItems;
    }
}
