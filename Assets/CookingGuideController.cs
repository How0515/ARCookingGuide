using UnityEngine;
using System.Collections.Generic;

public class CookingGuideController : MonoBehaviour
{
    public static CookingGuideController Instance; // 어디서든 접근 가능하게 싱글톤 처리

    [Header("Data")]
    public List<RecipeStep> recipeSteps; // 인스펙터에서 단계별 내용 입력
    public int currentStepIndex = 0;

    [Header("References")]
    public RecipeBoard recipeBoard;    // 중앙 메인 보드
    public HeadHUD headHUD;            // 우측 상단 HUD
    public TimerManager timerManager;  // 좌측 타이머 관리자

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 시작 시 0번 단계 보여주기
        UpdateAllUI();
    }

    // [다음] 버튼이나 음성인식으로 호출할 함수
    public void GoToNextStep()
    {
        if (currentStepIndex < recipeSteps.Count - 1)
        {
            currentStepIndex++;
            UpdateAllUI();
        }
        else
        {
            Debug.Log("요리 완료!");
            // 요리 완료 UI 띄우기 로직 추가 가능
        }
    }

    // [이전] 단계 (필요시)
    public void GoToPrevStep()
    {
        if (currentStepIndex > 0)
        {
            currentStepIndex--;
            UpdateAllUI();
        }
    }

    // 모든 UI 갱신 로직
    private void UpdateAllUI()
    {
        RecipeStep currentStep = recipeSteps[currentStepIndex];

        // 1. 중앙 보드 갱신
        recipeBoard.UpdateBoard(currentStep);

        // 2. 헤드 HUD (진행률) 갱신
        float progress = (float)(currentStepIndex + 1) / recipeSteps.Count;
        headHUD.UpdateHUD(currentStepIndex + 1, recipeSteps.Count, currentStep.stepTitle, progress);

        // 3. 타이머 자동 생성 (시간 설정이 있는 단계라면)
        if (currentStep.timerSeconds > 0)
        {
            // 타이머 매니저에게 요청 (좌측 상단에 띄우기)
            timerManager.SpawnTimer(currentStep.timerSeconds, currentStep.stepTitle);
        }
    }
}