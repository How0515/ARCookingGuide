using UnityEngine;
using System;

[Serializable]
public class RecipeStep
{
    public string stepTitle;       // 단계 제목 (예: 양파 썰기)
    public string description;     // 상세 설명
    public float timerSeconds;     // 타이머 필요 시 시간 (0이면 없음)
    // 영상이나 이미지가 필요하면 아래 주석 해제
    // public VideoClip clip; 
}