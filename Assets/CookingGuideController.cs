using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CookingGuideController : MonoBehaviour
{
    // 1️⃣ [추가] 타이머 스크립트를 연결할 변수 만들기
    public TimerObject timerObject;
    public static CookingGuideController Instance; // 어디서든 접근 가능하게 싱글톤 처리

    [Header("Data")]
    public List<RecipeStep> recipeSteps; // 인스펙터에서 단계별 내용 입력
    public int currentStepIndex = 0;

    [Header("References")]
    public RecipeBoard recipeBoard;    // 중앙 메인 보드
    public HeadHUD headHUD;            // 우측 상단 HUD
    public TimerManager timerManager;  // 좌측 타이머 관리자
    public UnityEngine.UI.RawImage rightScreenImage; // 우측 화면 (Raw Image)
    public UnityEngine.Video.VideoPlayer videoPlayer; // 영상 재생기

    [Header("UI References")]
    public TextMeshProUGUI stepTitleText;   // 제목 텍스트
    public TextMeshProUGUI descriptionText;

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

    // [음성 명령] "Start Timer"라고 말하면 실행
    public void StartCurrentTimer()
    {
        // 데이터 안전 장치
        if (recipeSteps == null || recipeSteps.Count == 0) return;

        RecipeStep currentStep = recipeSteps[currentStepIndex];

        // 현재 단계에 타이머 설정 시간이 있다면 타이머 생성
        if (currentStep.timerSeconds > 0)
        {
            timerManager.SpawnTimer(currentStep.timerSeconds, currentStep.stepTitle);
            Debug.Log("음성 명령으로 타이머를 시작했습니다.");
        }
        else
        {
            Debug.Log("현재 단계에는 타이머가 없습니다.");
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
    void UpdateAllUI()
    {
        if (recipeSteps == null || recipeSteps.Count == 0) return;

        // 2. 현재 단계 데이터 가져오기
        RecipeStep currentStep = recipeSteps[currentStepIndex];

        // =========================================================
        // [기능 1] 레시피 보드: (기존 코드 유지)
        // =========================================================
        stepTitleText.text = $"Step {currentStepIndex + 1}: {currentStep.stepTitle}";

        string fullDescription = "";
        for (int i = 0; i < recipeSteps.Count; i++)
        {
            if (i == currentStepIndex)
                fullDescription += $"<color=yellow><b>▶ {recipeSteps[i].description}</b></color>\n\n";
            else
                fullDescription += $"<color=#CCCCCC>{i + 1}. {recipeSteps[i].description}</color>\n\n";
        }
        descriptionText.text = fullDescription;

        // =========================================================
        // 🔥 [기능 2] 타이머 관리 (여기를 수정했습니다!)
        // =========================================================
        
        // 기존의 timerManager.SpawnTimer 대신 -> 우리가 만든 timerObject를 직접 제어합니다.
        if (timerObject != null)
        {
            // DB에 설정된 시간이 있으면 (0보다 크면)
            if (currentStep.timerSeconds > 0)
            {
                timerObject.gameObject.SetActive(true);           // 타이머 켜기
                timerObject.Initialize(currentStep.timerSeconds); // 시간 주입 (자동 시작 대기)
            }
            else
            {
                // 시간이 0이면 타이머 숨기기
                timerObject.gameObject.SetActive(false);
            }
        }

        // =========================================================
        // [기능 3] 비디오 플레이어 제어 (기존 코드 유지)
        // =========================================================
        if (currentStep.stepVideo != null)
        {
            rightScreenImage.gameObject.SetActive(true);
            videoPlayer.clip = currentStep.stepVideo;
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Stop();
            rightScreenImage.gameObject.SetActive(false);
        }

        // =========================================================
        // [기능 4] HUD 업데이트 (기존 코드 유지)
        // =========================================================
        if (headHUD != null)
        {
            float progress = (float)(currentStepIndex + 1) / (float)recipeSteps.Count;
            headHUD.UpdateHUD(currentStepIndex + 1, recipeSteps.Count, currentStep.stepTitle, progress);
        }
    }
}