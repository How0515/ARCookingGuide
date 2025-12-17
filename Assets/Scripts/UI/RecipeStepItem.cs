using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;

/// <summary>
/// 단일 요리 단계 아이템
/// - 단계 번호, 설명, 이미지 표시
/// - 체크박스 기능
/// - 개별 드래그 가능 (번호 부분만 잡아서 드래그)
/// - 분리 시 재이동/휴지통 버튼 표시
/// </summary>
public class RecipeStepItem : MonoBehaviour
{
    [Header("UI Components")]
    private TMP_Text stepNumberText;
    private TMP_Text descriptionText;
    private Image stepImage;
    private Toggle completionCheckbox;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    [Header("Drag Handle")]
    private Image dragHandle;  // 번호 부분 (드래그 가능 영역)

    [Header("Action Buttons")]
    private Button moveButton;     // + 버튼 (분리)
    private Button deleteButton;   // 휴지통 버튼 (복귀)

    [Header("Drag Settings")]
    [SerializeField] private bool isDraggable = true;
    private ObjectManipulator objectManipulator;
    private Transform originalParent;
    private int originalSiblingIndex;
    private Vector3 originalAnchoredPosition;  // 원래 UI 위치 저장 (x, y, z 모두)
    private bool isDetached = false;
    private bool isDragging = false;

    // 분리 후에도 진행률 업데이트를 위해 부모 패널 참조 저장
    private RecipeStepPanel parentPanel;

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

        // RectTransform 저장
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

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

        // 드래그 핸들 찾기 (번호 부분)
        if (dragHandle == null)
        {
            Transform handleTrans = transform.Find("StepNumberText");
            if (handleTrans != null)
                dragHandle = handleTrans.GetComponent<Image>();
        }

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

        // 부모 패널 참조 저장 (분리 후에도 진행률 업데이트를 위함)
        if (parentPanel == null)
            parentPanel = GetComponentInParent<RecipeStepPanel>();

        // 버튼 초기화
        SetupActionButtons();

        // 드래그 기능 설정
        SetupDragFunctionality();
    }

    /// <summary>
    /// 액션 버튼(+, 휴지통) 초기화
    /// </summary>
    private void SetupActionButtons()
    {
        // 기존 버튼 찾기 또는 생성
        Transform moveButtonTrans = transform.Find("MoveButton");
        Transform deleteButtonTrans = transform.Find("DeleteButton");

        if (moveButtonTrans != null && moveButton == null)
        {
            moveButton = moveButtonTrans.GetComponent<Button>();
            if (moveButton != null)
                moveButton.onClick.AddListener(OnMoveButtonClicked);
        }

        if (deleteButtonTrans != null && deleteButton == null)
        {
            deleteButton = deleteButtonTrans.GetComponent<Button>();
            if (deleteButton != null)
                deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        }

        // 초기에는 DeleteButton만 숨김 (분리되었을 때만 표시)
        if (deleteButton != null)
            deleteButton.gameObject.SetActive(false);
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
            // 초기에는 비활성화 (MoveButton 클릭 시에만 활성화)
            objectManipulator.enabled = false;
            objectManipulator.AllowFarManipulation = true;
            // OnManipulationStarted 이벤트는 제거 (MoveButton으로만 분리하도록)
        }

        // 부모 정보 저장
        if (transform.parent != null)
        {
            originalParent = transform.parent;
            originalSiblingIndex = transform.GetSiblingIndex();
            
            // UI 위치 저장 (x, y, z 모두)
            RectTransform rect = GetComponent<RectTransform>();
            if (rect != null)
                originalAnchoredPosition = rect.anchoredPosition3D;  // Vector3 사용!
        }
    }

    /// <summary>
    /// 손모양 버튼 클릭 이벤트
    /// 분리되지 않았으면 분리하고 드래그 활성화
    /// </summary>
    private void OnMoveButtonClicked()
    {
        if (!isDetached)
        {
            DetachFromParent();
            
            // 분리 후 ObjectManipulator 활성화 (이제 드래그 가능)
            if (objectManipulator != null)
            {
                objectManipulator.enabled = true;
                Debug.Log($"✅ [Step #{stepNumber}] ObjectManipulator 활성화 - 드래그 가능");
            }
        }
    }

    /// <summary>
    /// 휴지통 버튼 클릭 이벤트
    /// </summary>
    private void OnDeleteButtonClicked()
    {
        ReattachToParent();
    }

    /// <summary>
    /// 체크박스 변경 핸들러
    /// </summary>
    private void OnCheckboxChanged(bool isChecked)
    {
        SetCompleted(isChecked);
        
        // 부모 패널의 진행률 업데이트 (분리된 상태에서도 작동)
        if (parentPanel != null)
        {
            parentPanel.OnStepCompleted();
        }
        else
        {
            // 혹시 parentPanel이 null인 경우 다시 한 번 찾기
            RecipeStepPanel foundPanel = GetComponentInParent<RecipeStepPanel>();
            if (foundPanel != null)
            {
                parentPanel = foundPanel;
                parentPanel.OnStepCompleted();
            }
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
    /// 부모 패널에서 분리
    /// </summary>
    public void DetachFromParent()
    {
        if (isDetached) return;

        RectTransform rectTransform = GetComponent<RectTransform>();
        
        // 부모 정보 및 UI 위치 저장
        if (transform.parent != null)
        {
            originalParent = transform.parent;
            originalSiblingIndex = transform.GetSiblingIndex();
            originalAnchoredPosition = rectTransform.anchoredPosition3D;  // z까지 저장!
            
            Debug.Log($"💾 [Step #{stepNumber}] 위치 저장 - AnchoredPos3D: {originalAnchoredPosition}, SiblingIndex: {originalSiblingIndex}");
        }

        // 월드 좌표 저장 (분리 시 위치 유지용)
        Vector3 worldPos = rectTransform.position;
        Quaternion worldRot = rectTransform.rotation;
        Vector3 worldScale = rectTransform.lossyScale;

        // 부모에서 분리
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

        // 분리 후 버튼 표시 전환
        UpdateButtonVisibility();
    }

    /// <summary>
    /// 원래 부모 패널로 복귀
    /// </summary>
    public void ReattachToParent()
    {
        if (!isDetached || originalParent == null) return;

        // ObjectManipulator 비활성화 (복귀 후 드래그 불가)
        if (objectManipulator != null)
        {
            objectManipulator.enabled = false;
            Debug.Log($"❌ [Step #{stepNumber}] ObjectManipulator 비활성화");
        }

        // GraphicRaycaster를 먼저 제거 (Canvas 의존성 때문)
        GraphicRaycaster raycaster = GetComponent<GraphicRaycaster>();
        if (raycaster != null)
            DestroyImmediate(raycaster);
        
        // Canvas 제거
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
            DestroyImmediate(canvas);

        // 부모로 복귀
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.SetParent(originalParent);
        rectTransform.SetSiblingIndex(originalSiblingIndex);
        
        // 로컬 변환 리셋
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
        
        // 원래 UI 위치 복원 (x, y, z 모두 복원!)
        rectTransform.anchoredPosition3D = originalAnchoredPosition;
        
        Debug.Log($"📍 [Step #{stepNumber}] 위치 복원 - AnchoredPos3D: {originalAnchoredPosition}");

        isDetached = false;

        // 복귀 후 버튼 표시 전환
        UpdateButtonVisibility();
    }

    /// <summary>
    /// 분리 상태에 따라 버튼 표시 업데이트
    /// </summary>
    private void UpdateButtonVisibility()
    {
        // MoveButton은 항상 표시 (분리된 상태에서도 드래그 가능하게)
        // DeleteButton은 분리되었을 때만 표시
        
        if (moveButton != null)
            moveButton.gameObject.SetActive(true);  // 항상 표시

        if (deleteButton != null)
            deleteButton.gameObject.SetActive(isDetached);
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
