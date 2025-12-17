using UnityEngine;
using TMPro;
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
        // 1. 데이터가 없으면 아무것도 안 함 (에러 방지)
        if (recipeSteps == null || recipeSteps.Count == 0) return;

        // 2. 현재 단계 데이터 가져오기
        RecipeStep currentStep = recipeSteps[currentStepIndex];

        // =========================================================
        // [기능 1] 레시피 보드: 전체 내용을 보여주되, 현재 단계만 강조
        // =========================================================
        
        // 제목: "Step 1: 양파 썰기" (여기는 이미 +1이 잘 되어 있었습니다)
        stepTitleText.text = $"Step {currentStepIndex + 1}: {currentStep.stepTitle}";

        // 내용: 전체 리스트를 돌면서 현재 단계만 노란색으로 칠하기
        string fullDescription = "";
        for (int i = 0; i < recipeSteps.Count; i++)
        {
            if (i == currentStepIndex)
            {
                // [현재 단계] 노란색 + 굵게 + 화살표
                fullDescription += $"<color=yellow><b>▶ {recipeSteps[i].description}</b></color>\n\n";
            }
            else
            {
                // [다른 단계] 회색으로 연하게
                fullDescription += $"<color=#CCCCCC>{i + 1}. {recipeSteps[i].description}</color>\n\n";
            }
        }
        descriptionText.text = fullDescription;

        // =========================================================
        // [기능 2] 타이머 관리
        // =========================================================
        
        // 현재 단계에 타이머가 필요하면 생성
        if (currentStep.timerSeconds > 0)
        {
            timerManager.SpawnTimer(currentStep.timerSeconds, currentStep.stepTitle);
        }
        
        // (주의: 아까 코드에 타이머 생성 구문이 여기에 또 있었습니다. 중복 삭제했습니다!)

        // =========================================================
        // [기능 3] 비디오 플레이어 제어
        // =========================================================
        if (currentStep.stepVideo != null)
        {
            rightScreenImage.gameObject.SetActive(true); // 화면 켜기
            videoPlayer.clip = currentStep.stepVideo;    // 비디오 갈아끼우기
            videoPlayer.Play();                          // 재생
        }
        // 비디오가 없으면 -> 화면 끄기
        else
        {
            videoPlayer.Stop();                           // 정지
            rightScreenImage.gameObject.SetActive(false); // 화면 숨기기
        }

        // =========================================================
        // [기능 4] HUD 업데이트 (수정된 부분)
        // =========================================================
        if (headHUD != null)
        {
            // 1. 진행률 계산
            float progress = (float)(currentStepIndex + 1) / (float)recipeSteps.Count;

            // 2. HUD 갱신
            // [중요] 첫 번째 인자에 +1을 해서 넘겨줍니다! (0 -> 1, 1 -> 2)
            headHUD.UpdateHUD(currentStepIndex + 1, recipeSteps.Count, currentStep.stepTitle, progress);
        }
    }
}