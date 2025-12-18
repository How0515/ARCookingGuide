using UnityEngine;
using System;

[Serializable]
public class RecipeStep
{
    public string stepTitle;       // 단계 제목 (예: 양파 썰기)
    [TextArea] public string description;
    public float timerSeconds;
    
    public UnityEngine.Video.VideoClip stepVideo;
}