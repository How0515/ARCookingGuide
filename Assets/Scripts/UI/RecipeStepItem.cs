using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;

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

    [Header("Drag Settings")]
    [SerializeField] private bool isDraggable = true;
    private ObjectManipulator objectManipulator;
    private Transform originalParent;
    private int originalSiblingIndex;
    private bool isDetached = false;

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

        // 드래그 기능 설정
        SetupDragFunctionality();
    }

    /// <summary>
    /// 드래그 기능 설정
    /// </summary>
    private void SetupDragFunctionality()
    {
        if (objectManipulator == null)
            objectManipulator = GetComponent<ObjectManipulator>();

        if (objectManipulator != null)
        {
            objectManipulator.enabled = isDraggable;
            objectManipulator.AllowFarManipulation = true;
            objectManipulator.OnManipulationStarted.AddListener((eventData) => OnDragStarted());
            objectManipulator.OnManipulationEnded.AddListener((eventData) => OnDragEnded());
        }

        // 부모 정보 저장
        if (transform.parent != null)
        {
            originalParent = transform.parent;
            originalSiblingIndex = transform.GetSiblingIndex();
        }
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

    /// <summary>
    /// 드래그 시작 핸들러
    /// </summary>
    private void OnDragStarted()
    {
        if (!isDetached)
        {
            DetachFromParent();
        }
    }

    /// <summary>
    /// 드래그 종료 핸들러
    /// </summary>
    private void OnDragEnded()
    {
        // 드래그 종료 후 처리 (필요시 확장)
    }

    /// <summary>
    /// 부모 패널에서 분리
    /// </summary>
    public void DetachFromParent()
    {
        if (isDetached) return;

        // 부모 정보 저장
        if (transform.parent != null)
        {
            originalParent = transform.parent;
            originalSiblingIndex = transform.GetSiblingIndex();
        }

        // 월드 스페이스로 이동
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 worldPos = rectTransform.position;
        Quaternion worldRot = rectTransform.rotation;
        Vector3 worldScale = rectTransform.lossyScale;

        transform.SetParent(null);
        rectTransform.position = worldPos;
        rectTransform.rotation = worldRot;
        rectTransform.localScale = worldScale;

        // Canvas 추가 (독립적으로 렌더링)
        Canvas canvas = gameObject.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            
            GraphicRaycaster raycaster = gameObject.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
                gameObject.AddComponent<GraphicRaycaster>();
        }

        isDetached = true;
    }

    /// <summary>
    /// 원래 부모 패널로 복귀
    /// </summary>
    public void ReattachToParent()
    {
        if (!isDetached || originalParent == null) return;

        // Canvas 제거
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
            DestroyImmediate(canvas);
        
        GraphicRaycaster raycaster = GetComponent<GraphicRaycaster>();
        if (raycaster != null)
            DestroyImmediate(raycaster);

        // 부모로 복귀
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.SetParent(originalParent);
        rectTransform.SetSiblingIndex(originalSiblingIndex);
        
        // 로컬 스케일 리셋
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;

        isDetached = false;
    }

    /// <summary>
    /// 드래그 가능 여부 설정
    /// </summary>
    public void SetDraggable(bool draggable)
    {
        isDraggable = draggable;
        if (objectManipulator != null)
            objectManipulator.enabled = draggable;
    }

    /// <summary>
    /// 분리 상태 확인
    /// </summary>
    public bool IsDetached => isDetached;
}
