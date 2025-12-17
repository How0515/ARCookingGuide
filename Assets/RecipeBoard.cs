using UnityEngine;
using TMPro;

public class RecipeBoard : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    // public VideoPlayer videoPlayer; // 영상 필요 시

    public void UpdateBoard(RecipeStep step)
    {
        titleText.text = step.stepTitle;
        descriptionText.text = step.description;
        
        // 영상 재생 로직이 필요하면 여기에 추가
        // if(step.clip != null) { videoPlayer.clip = step.clip; videoPlayer.Play(); }
    }
}