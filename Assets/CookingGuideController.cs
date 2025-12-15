using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;

public class CookingGuideController : MonoBehaviour
{
    [System.Serializable]
    public struct CookingStep
    {
        public string title;
        [TextArea] public string ingredients;
        public string heatLevel;
        public float timerSeconds;
        public VideoClip videoClip;
    }

    public List<CookingStep> stepList;
    public TextMeshPro textTitle;
    public TextMeshPro textIngredients;
    public TextMeshPro textHeat;
    public TextMeshPro textTimer;
    public VideoPlayer videoPlayer;

    private int currentStepIndex = 0;
    private float currentTimer = 0f;
    private bool isTimerRunning = false;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        if (isTimerRunning && currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
            UpdateTimerText();
        }
        else if (currentTimer <= 0 && isTimerRunning)
        {
            isTimerRunning = false;
            textTimer.text = "00:00 - 완료!";
        }
    }
    private float lastClickTime = 0f;
    private float clickCooldown = 1f;
    public void NextStep()
    {
        if (Time.time - lastClickTime < clickCooldown) return;
        
        lastClickTime = Time.time;

        if (currentStepIndex < stepList.Count - 1)
        {
            currentStepIndex++;
            UpdateUI();
            Debug.Log("다음 단계로 이동: " + currentStepIndex);
        }
    }

    public void StartTimer()
    {
        Debug.Log("타이머 시작 시도! 현재 설정된 시간: " + currentTimer);
        if (currentTimer > 0)
        {
            isTimerRunning = true;
            Debug.Log("타이머가 실행 상태로 변경되었습니다.");
        }
        else
        {
            Debug.Log("설정된 시간이 0이라서 시작할 수 없습니다.");
        }
    }

    void UpdateUI()
    {
        if (stepList.Count == 0) return;
        CookingStep currentData = stepList[currentStepIndex];
        
        if(textTitle) textTitle.text = currentData.title;
        if(textIngredients) textIngredients.text = currentData.ingredients;
        if(textHeat) textHeat.text = currentData.heatLevel;

        if(videoPlayer && currentData.videoClip != null)
        {
            videoPlayer.clip = currentData.videoClip;
            videoPlayer.Play();
        }

        if (currentData.timerSeconds > 0)
        {
            currentTimer = currentData.timerSeconds;
            isTimerRunning = false; 
            if(textTimer) textTimer.gameObject.SetActive(true);
            UpdateTimerText();
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTimer / 60F);
        int seconds = Mathf.FloorToInt(currentTimer % 60F);
        if(textTimer) textTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}