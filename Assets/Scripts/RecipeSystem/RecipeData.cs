using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CookingStep
{
    public string title;
    public string description;
    public int timerSeconds;      // 0 = không có timer, đổi float thành int
    public string tip;
    public string imageName;        // tên file ảnh trong RecipeDB/Images (không cần đuôi)
}

[Serializable]
public class Recipe
{
    public int id;
    public string name_kr;
    public string name_vi;
    public string name_en;
    public List<string> ingredients;
    public List<CookingStep> steps;
}

[Serializable]
public class RecipeDatabase
{
    public List<Recipe> recipes;
    public List<string> globalTips;
}