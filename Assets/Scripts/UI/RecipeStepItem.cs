using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 단일 요리 단계 아이템
/// - 단계 번호, 설명, 이미지 표시
/// - 체크박스 기능
/// - 개별 드래그 가능
/// </summary>
public class RecipeStepItem : MonoBehaviour
{
    [Header("UI Components")]
    private TMP_Text stepNumberText;
    private TMP_Text descriptionText;
    private Image stepImage;
    private Toggle completionCheckbox;
    private CanvasGroup canvasGroup;

    private RecipeStepPanel.CookingStep stepData;
    private int stepNumber;
    private bool isCompleted = false;

    /// <summary>
    /// 단계 아이템 초기화
    /// </summary>
    public void Initialize(RecipeStepPanel.CookingStep step, int number)
    {
        stepData = step;
        stepNumber = number;

        // 컴포넌트 자동 찾기
        if (completionCheckbox == null)
            completionCheckbox = GetComponentInChildren<Toggle>();
        
        if (stepNumberText == null)
            stepNumberText = GetComponentInChildren<TMP_Text>();
        
        if (descriptionText == null)
        {
            TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
            if (texts.Length > 1)
                descriptionText = texts[1];
        }
        
        if (stepImage == null)
            stepImage = GetComponentInChildren<Image>();
        
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        // UI 업데이트
        if (stepNumberText != null)
            stepNumberText.text = number.ToString();

        if (descriptionText != null)
            descriptionText.text = step.description;

        if (stepImage != null && step.stepImage != null)
            stepImage.sprite = step.stepImage;

        if (completionCheckbox != null)
        {
            // 기존 리스너 제거 후 추가 (중복 방지)
            completionCheckbox.onValueChanged.RemoveAllListeners();
            completionCheckbox.onValueChanged.AddListener(OnCheckboxChanged);
        }

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 체크박스 변경 핸들러
    /// </summary>
    private void OnCheckboxChanged(bool isChecked)
    {
        SetCompleted(isChecked);
        
        // 부모 패널의 진행률 업데이트
        RecipeStepPanel parentPanel = GetComponentInParent<RecipeStepPanel>();
        if (parentPanel != null)
        {
            parentPanel.OnStepCompleted();
        }
    }

    /// <summary>
    /// 단계 완료 설정
    /// </summary>
    public void SetCompleted(bool completed)
    {
        isCompleted = completed;
        
        if (completionCheckbox != null)
            completionCheckbox.isOn = completed;

        // 완료된 항목은 약간 투명하게
        if (canvasGroup != null)
            canvasGroup.alpha = completed ? 0.6f : 1f;
    }

    /// <summary>
    /// 완료 상태 반환
    /// </summary>
    public bool IsCompleted => isCompleted;

    /// <summary>
    /// 단계 번호 반환
    /// </summary>
    public int StepNumber => stepNumber;

    /// <summary>
    /// 단계 데이터 반환
    /// </summary>
    public RecipeStepPanel.CookingStep StepData => stepData;
}
