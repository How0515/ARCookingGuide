using UnityEngine;

public class NextButtonHandler : MonoBehaviour
{
    // 버튼 클릭 이벤트(OnClick)에 연결할 함수
    public void OnClickNext()
    {
        CookingGuideController.Instance.GoToNextStep();
    }

    // 개발 중 키보드로 테스트하기 위함 (스페이스바 누르면 다음 단계)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnClickNext();
        }
    }
    
    // 나중에 음성 인식 SDK에서 "Next" 감지 시 이 함수 호출
    public void OnVoiceCommandReceived(string command)
    {
        if(command.Contains("다음") || command.Contains("Next"))
        {
            OnClickNext();
        }
    }
}