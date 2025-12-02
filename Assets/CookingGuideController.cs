using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 텍스트 제어용
using UnityEngine.Video; // 비디오 제어용

public class CookingGuideController : MonoBehaviour
{
    // 1. 데이터 꾸러미 정의 (각 단계별 정보)
    [System.Serializable]
    public struct CookingStep
    {
        public string title;        // 예: "떡 불리기"
        [TextArea] public string ingredients; // 예: "떡 200g, 물"
        public string heatLevel;    // 예: "사용 안함" or "중불"
        public float timerSeconds;  // 타이머 시간 (초 단위, 0이면 타이머 없음)
        public VideoClip videoClip; // 보여줄 영상/움짤
    }

    public List<CookingStep> stepList; // 인스펙터에서 입력할 리스트

    // 2. UI 연결 (유니티에서 끌어다 놓을 곳)
    public TextMeshPro textTitle;
    public TextMeshPro textIngredients;
    public TextMeshPro textHeat;
    public TextMeshPro textTimer;
    public VideoPlayer videoPlayer;

    private int currentStepIndex = 0; // 현재 몇 번째 단계인지
    private float currentTimer = 0f;
    private bool isTimerRunning = false;

    void Start()
    {
        UpdateUI(); // 시작하면 첫 번째 단계 보여주기
    }

    void Update()
    {
        // 타이머 작동 시작 버튼 구현 필요
        // 타이머가 작동 중일 때만 시간 줄이기
        if (isTimerRunning && currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
            UpdateTimerText();
        }
    }

    // 다음 단계로 넘어가는 함수 (버튼/음성에 연결)
    public void NextStep()
    {
        if (currentStepIndex < stepList.Count - 1)
        {
            currentStepIndex++;
            UpdateUI();
        }
    }

    // 이전 단계로 (필요하면 버튼 연결)
    public void PrevStep()
    {
        if (currentStepIndex > 0)
        {
            currentStepIndex--;
            UpdateUI();
        }
    }

    // 화면 갱신 함수
    void UpdateUI()
    {
        CookingStep currentData = stepList[currentStepIndex];

        // 텍스트 변경
        textTitle.text = currentData.title;
        textIngredients.text = currentData.ingredients;
        textHeat.text = currentData.heatLevel;

        // 비디오 변경
        if(currentData.videoClip != null)
        {
            videoPlayer.clip = currentData.videoClip;
            videoPlayer.Play();
        }

        // 타이머 설정
        if (currentData.timerSeconds > 0)
        {
            currentTimer = currentData.timerSeconds;
            isTimerRunning = true;
            textTimer.gameObject.SetActive(true); // 타이머 켜기
        }
        else
        {
            isTimerRunning = false;
            textTimer.text = ""; 
            textTimer.gameObject.SetActive(false); // 타이머 끄기
        }
    }

    void UpdateTimerText()
    {
        // 시간을 00:00 형식으로 표시
        int minutes = Mathf.FloorToInt(currentTimer / 60F);
        int seconds = Mathf.FloorToInt(currentTimer % 60F);
        textTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}