using UnityEngine;

public class NextButtonHandler : MonoBehaviour
{
    // 버튼 클릭 이벤트(OnClick)에 연결할 함수
    public void OnClickNext()
    {
        Debug.Log("버튼 눌림 성공!");
        CookingGuideController.Instance.GoToNextStep();
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