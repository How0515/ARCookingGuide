using UnityEngine;
using TMPro;

public class TimerObject : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public GameObject startButton;
    
    private float remainingTime;
    private bool isRunning = false;

    public void Initialize(float seconds)
    {
        remainingTime = seconds;
        isRunning = false;
        
        UpdateText(); // 초기 시간(예: 03:00) 표시
        
        if (startButton != null)
            startButton.SetActive(true); // 시작 버튼 보여주기
    }

    // [추가] 버튼이 눌리면 실행되는 함수
    public void OnClickStart()
    {
        Debug.Log("🖱️ 버튼 클릭 신호 수신 성공!");
        isRunning = true; // 시간 흐르기 시작
        if (startButton != null)
            startButton.SetActive(false); // 시작 버튼 숨기기 (숫자만 보이게)
    }

    public void OnClickClose()
    {
        Destroy(gameObject);
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
                // 알람 소리 재생 등
            }
        }
    }

    void UpdateText()
    {
        int min = Mathf.FloorToInt(remainingTime / 60);
        int sec = Mathf.FloorToInt(remainingTime % 60);
        timeText.text = $"{min:00}:{sec:00}";
    }
}