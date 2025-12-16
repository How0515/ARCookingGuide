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
    [SerializeField] private TextMeshProUGUI stepNumberText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image stepImage;
    [SerializeField] private Toggle completionCheckbox;
    [SerializeField] private CanvasGroup canvasGroup;

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

        // UI 업데이트
        if (stepNumberText != null)
            stepNumberText.text = number.ToString();

        if (descriptionText != null)
            descriptionText.text = step.description;

        if (stepImage != null && step.stepImage != null)
            stepImage.sprite = step.stepImage;

        if (completionCheckbox != null)
        {
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
