using UnityEngine;
using TMPro;

// 개별 타이머 하나하나에 붙는 스크립트입니다.
public class TimerObject : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    private float remainingTime;
    private bool isRunning = false;

    // 타이머 시작 함수 (초 단위)
    public void Initialize(float seconds)
    {
        remainingTime = seconds;
        isRunning = true;
        UpdateText();
    }

    void Update()
    {
        if (isRunning && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateText();
            
            if (remainingTime <= 0)
            {
                remainingTime = 0;
                isRunning = false;
                timeText.text = "완료!";
                // 여기에 알람 소리 등을 추가할 수 있습니다.
            }
        }
    }

    void UpdateText()
    {
        // 남은 시간을 분:초 형태로 표시
        int min = Mathf.FloorToInt(remainingTime / 60);
        int sec = Mathf.FloorToInt(remainingTime % 60);
        timeText.text = $"{min:00}:{sec:00}";
    }
}