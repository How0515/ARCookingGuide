using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public TMP_Text recipeNameText;
    public TMP_Text ingredientsText;
    public Transform stepsContainer; // 단계들을 담을 컨테이너
    public Transform progressDisplay; // 진행률 표시 (1/6 같은)

    [Header("UI Prefab")]
    public GameObject stepItemPrefab; // 단계 아이템 프리팹

    [Header("Panel Settings")]
    public CanvasGroup canvasGroup; // 투명도 조절용
    [SerializeField] private float defaultAlpha = 0.9f;

    [Header("Detach Settings")]
    [SerializeField] private float detachMaxDistance = 2f; // 이 거리 이상 벗어나면 표시
    [SerializeField] private bool autoReattachOnDistance = false; // 거리 기반 자동 복귀 활성화

    private List<RecipeStepItem> stepItems = new List<RecipeStepItem>();

    private void Start()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup != null)
            canvasGroup.alpha = defaultAlpha;

        // Canvas Event Camera 자동 설정
        SetupCanvasEventCamera();

        // StepsContainer에 LayoutGroup 자동 추가
        SetupStepsContainerLayout();
        
        InitializeUI();
    }

    private void Update()
    {
        // 거리 기반 자동 복귀 (활성화된 경우)
        if (autoReattachOnDistance)
        {
            CheckDetachedStepsDistance();
        }
    }

    /// <summary>
    /// Canvas의 Event Camera를 자동으로 설정
    /// </summary>
    private void SetupCanvasEventCamera()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            if (canvas.worldCamera == null)
            {
                canvas.worldCamera = Camera.main;
            }
        }

        // Graphic Raycaster 확인
        GraphicRaycaster raycaster = GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }
    }

    /// <summary>
    /// StepsContainer에 VerticalLayoutGroup 자동 설정
    /// </summary>
    private void SetupStepsContainerLayout()
    {
        if (stepsContainer == null)
            return;

        // 이미 LayoutGroup이 있으면 스킵 (에러 방지)
        if (stepsContainer.GetComponent<VerticalLayoutGroup>() != null)
            return;

        // 자식이 없을 때만 LayoutGroup 추가
        if (stepsContainer.childCount == 0)
        {
            VerticalLayoutGroup layout = stepsContainer.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.childForceExpandHeight = false;
            layout.childControlHeight = true;
            layout.childControlWidth = true;

            // ContentSizeFitter도 추가
            ContentSizeFitter fitter = stepsContainer.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    /// <summary>
    /// UI 초기화 - 패널을 생성합니다
    /// </summary>
    private void InitializeUI()
    {
        // 샘플 데이터가 없으면 자동 생성
        if (stepList.Count == 0)
        {
            CreateSampleSteps();
        }

        // 레시피 이름과 재료 설정
        if (recipeNameText != null)
            recipeNameText.text = recipeName;

        if (ingredientsText != null && stepList.Count > 0)
            ingredientsText.text = stepList[0].ingredients;

        // 단계 아이템 생성
        if (stepsContainer != null && stepItemPrefab != null)
        {
            int index = 1;
            foreach (var step in stepList)
            {
                GameObject itemObj = Instantiate(stepItemPrefab, stepsContainer);
                RecipeStepItem stepItem = itemObj.GetComponent<RecipeStepItem>();
                
                if (stepItem != null)
                {
                    stepItem.Initialize(step, index);
                    stepItems.Add(stepItem);
                    index++;
                }
            }
        }
        else
        {
            Debug.LogError($"❌ stepsContainer 또는 stepItemPrefab이 null입니다!");
        }

        UpdateProgressDisplay();
    }

    /// <summary>
    /// 샘플 6단계 데이터 생성
    /// </summary>
    private void CreateSampleSteps()
    {
        if (stepList == null)
            stepList = new List<CookingStep>();
        
        stepList.Clear(); // 기존 데이터 제거
        
        // 샘플 데이터 추가
        stepList.Add(new CookingStep
        {
            title = "재료 준비",
            ingredients = "파스타 면 200g, 토마토 소스 1컵, 올리브유 2스푼, 마늘 3쪽, 양파 1/2개, 파슬리 약간",
            heatLevel = "준비",
            timerSeconds = 0,
            description = "모든 재료를 깨끗이 씻고 손질합니다. 마늘과 양파는 잘게 다집니다."
        });
        stepList.Add(new CookingStep
        {
            title = "물 끓이기",
            ingredients = "",
            heatLevel = "강불",
            timerSeconds = 300,
            description = "큰 냄비에 물을 충분히 붓고 소금을 한 꼬집 넣어 끓입니다."
        });
        stepList.Add(new CookingStep
        {
            title = "면 삶기",
            ingredients = "",
            heatLevel = "중불",
            timerSeconds = 480,
            description = "끓는 물에 파스타 면을 넣고 8분간 삶습니다. 알덴테 상태가 되도록 합니다."
        });
        stepList.Add(new CookingStep
        {
            title = "소스 만들기",
            ingredients = "",
            heatLevel = "중불",
            timerSeconds = 420,
            description = "팬에 올리브유를 두르고 마늘과 양파를 볶다가 토마토 소스를 넣고 7분간 끓입니다."
        });
        stepList.Add(new CookingStep
        {
            title = "면과 소스 섞기",
            ingredients = "",
            heatLevel = "약불",
            timerSeconds = 120,
            description = "삶은 면의 물기를 빼고 소스와 함께 2분간 버무립니다."
        });
        stepList.Add(new CookingStep
        {
            title = "플레이팅",
            ingredients = "",
            heatLevel = "완료",
            timerSeconds = 0,
                description = "접시에 담고 파슬리를 뿌려 완성합니다. 취향에 따라 파마산 치즈를 추가하세요."
            });
    }

    /// <summary>
    /// 단계 완료 시 호출 (public으로 RecipeStepItem에서 접근)
    /// </summary>
    public void OnStepCompleted()
    {
        UpdateProgressDisplay();
    }

    /// <summary>
    /// 진행률 표시 업데이트 (예: 1/6)
    /// </summary>
    private void UpdateProgressDisplay()
    {
        if (progressDisplay != null)
        {
            TMP_Text progressText = progressDisplay.GetComponentInChildren<TMP_Text>();
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

    /// <summary>
    /// 특정 단계를 패널에서 분리
    /// </summary>
    public void DetachStep(int stepIndex)
    {
        if (stepIndex >= 0 && stepIndex < stepItems.Count)
        {
            stepItems[stepIndex].DetachFromParent();
        }
    }

    /// <summary>
    /// 특정 단계를 패널로 복귀
    /// </summary>
    public void ReattachStep(int stepIndex)
    {
        if (stepIndex >= 0 && stepIndex < stepItems.Count)
        {
            stepItems[stepIndex].ReattachToParent();
        }
    }

    /// <summary>
    /// 모든 분리된 단계를 패널로 복귀
    /// </summary>
    public void ReattachAllDetachedSteps()
    {
        foreach (var stepItem in stepItems)
        {
            if (stepItem.IsDetached)
            {
                stepItem.ReattachToParent();
            }
        }
    }

    /// <summary>
    /// 분리된 단계 개수 반환
    /// </summary>
    public int GetDetachedStepCount()
    {
        int count = 0;
        foreach (var stepItem in stepItems)
        {
            if (stepItem.IsDetached)
                count++;
        }
        return count;
    }

    /// <summary>
    /// 분리된 단계들의 거리 확인 및 자동 복귀 처리
    /// </summary>
    private void CheckDetachedStepsDistance()
    {
        foreach (var stepItem in stepItems)
        {
            if (stepItem.IsDetached)
            {
                float distance = Vector3.Distance(stepItem.transform.position, transform.position);
                if (distance > detachMaxDistance)
                {
                    stepItem.ReattachToParent();
                }
            }
        }
    }

    /// <summary>
    /// 거리 기반 자동 복귀 설정
    /// </summary>
    public void SetAutoReattachOnDistance(bool enable, float maxDistance = 2f)
    {
        autoReattachOnDistance = enable;
        detachMaxDistance = maxDistance;
    }
