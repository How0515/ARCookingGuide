using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Recipe
{
    public string recipe_id;       // 레시피 고유 ID
    public string name;            // 레시피 이름 (예: 매콤 된장찌개)
    public string category;        // 음식 카테고리 (예: 한식)
    public string ingredients;     // 재료 목록
    public List<RecipeStep> steps; // 단계 리스트
}

[Serializable]
public class RecipeStep
{
    public int stepNumber;          // 단계 번호 (1, 2, 3...)
    [TextArea] public string description;    // 단계 설명
    public float timerSeconds;      // 타이머 시간 (초 단위)
    public string imageName;        // 이미지 파일명 (예: 6859263_step_1.jpg)
    public Texture2D stepImage;     // 로드된 이미지
}