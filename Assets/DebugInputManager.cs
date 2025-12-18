using UnityEngine;

public class DebugInputManager : MonoBehaviour
{
    // 여기에 우리가 만든 매니저들을 연결할 겁니다.
    public TimerObject timerObject;           // 타이머 스크립트
    public CookingGuideController guideController; // 레시피 넘기는 스크립트

    void Update()
    {
        // 1. 스페이스바 누르면 -> 타이머 시작
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("⌨️ 스페이스바 입력: 타이머 시작 시도");
            if (timerObject != null)
            {
                timerObject.OnClickStart(); // 타이머 시작 함수 호출
            }
        }

        // 2. 오른쪽 화살표(->) 누르면 -> 다음 스텝
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("⌨️ 오른쪽 화살표 입력: 다음 단계 이동 시도");
            if (guideController != null)
            {
                guideController.GoToNextStep(); // 다음 단계 함수 호출
            }
        }
    }
}