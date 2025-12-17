using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 레시피 데이터 관리자
/// - CSV 파일에서 레시피 데이터 로드
/// - CookingGuideController와 호환되는 형식으로 제공
/// - 다른 UI에서도 재사용 가능
/// </summary>
public class RecipeDataManager : MonoBehaviour
{
    /// <summary>
    /// CookingGuideController.CookingStep과 동일한 구조
    /// </summary>
    [System.Serializable]
    public struct CookingStep
    {
        public string title;
        public string ingredients;
        public string heatLevel;
        public float timerSeconds;
        public Sprite stepImage;
    }

    [System.Serializable]
    public struct RecipeData
    {
        public int recipeId;
        public string recipeName;
        public string category;
        public string fullIngredients;
        public List<CookingStep> steps;
    }

    private static RecipeDataManager instance;
    private List<RecipeData> allRecipes = new List<RecipeData>();

    public static RecipeDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RecipeDataManager>();
                if (instance == null)
                {
                    GameObject managerObj = new GameObject("RecipeDataManager");
                    instance = managerObj.AddComponent<RecipeDataManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadRecipeDatabase();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// CSV 파일에서 레시피 데이터 로드
    /// </summary>
    private void LoadRecipeDatabase()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("recipeDB");
        if (csvFile == null)
        {
            Debug.LogError("❌ recipeDB.csv 파일을 찾을 수 없습니다!");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        
        // 헤더 스킵 (첫 번째 라인)
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] fields = ParseCSVLine(lines[i]);
            if (fields.Length < 6)
                continue;

            RecipeData recipe = new RecipeData();
            recipe.recipeId = int.Parse(fields[0].Trim());
            recipe.recipeName = fields[1].Trim();
            recipe.category = fields[2].Trim();
            recipe.fullIngredients = fields[3].Trim();

            // 단계 파싱
            recipe.steps = ParseSteps(recipe.recipeId, fields[4], fields[5], recipe.fullIngredients);

            allRecipes.Add(recipe);

            Debug.Log($"✅ 레시피 로드: {recipe.recipeName} ({recipe.steps.Count}단계)");
        }

        Debug.Log($"📚 총 {allRecipes.Count}개의 레시피 로드 완료");
    }

    /// <summary>
    /// CSV 라인을 필드로 분리
    /// </summary>
    private string[] ParseCSVLine(string line)
    {
        return line.Split(',');
    }

    /// <summary>
    /// 단계별 데이터 파싱
    /// </summary>
    private List<CookingStep> ParseSteps(int recipeId, string stepsText, string imagesText, string fullIngredients)
    {
        List<CookingStep> steps = new List<CookingStep>();
        
        string[] stepArray = stepsText.Split(';');
        string[] imageArray = imagesText.Split(';');

        for (int i = 0; i < stepArray.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(stepArray[i]))
                continue;

            CookingStep step = new CookingStep();
            step.title = stepArray[i].Trim();
            step.ingredients = ""; // 단계별로 나눠서 가져올 수 없음
            step.heatLevel = ""; // csv파일에서 가져올 없는 값
            step.timerSeconds = 300f;  // 기본 5분

            // 이미지 로드
            if (i < imageArray.Length)
            {
                string imageName = imageArray[i].Trim().Replace(".jpg", "").Replace(".png", "");
                step.stepImage = Resources.Load<Sprite>($"RecipeDB/Images/{imageName}");
            }

            steps.Add(step);
        }

        return steps;
    }

    /// <summary>
    /// 레시피 이름으로 검색 → 해당 레시피의 모든 단계만 반환
    /// ingredients는 모든 단계에 같은 재료 문자열 포함
    /// 레시피가 없으면 빈 리스트 반환
    /// 
    /// Input : 레시피 이름 ex) "계란볶음밥"
    /// Output : 레시피 단계 리스트, 원소는 dict로 title, ingredients, heatLevel, timerSeconds, stepImage로 구성
    /// 
    /// 사용 예시:
    /// List<CookingStep> steps = RecipeDataManager.Instance.GetRecipeSteps("계란볶음밥");
    /// cookingController.stepList = steps;
    /// </summary>
    public List<CookingStep> GetRecipeSteps(string recipeName)
    {
        var recipe = allRecipes.Find(r => r.recipeName == recipeName);
        return recipe.steps != null ? recipe.steps : new List<CookingStep>();
    }

    /// <summary>
    /// ID로 검색 (숫자)
    /// GetRecipeSteps()와 동일하지만 ID 기반

    /// Input : 레시피 ID ex) 2
    /// Output : GetRecipeSteps와 동일
    /// </summary>
    public List<CookingStep> GetRecipeStepsById(int recipeId)
    {
        var recipe = allRecipes.Find(r => r.recipeId == recipeId);
        return recipe.steps != null ? recipe.steps : new List<CookingStep>();
    }

    /// <summary>
    /// 단계뿐만 아니라 레시피 메타데이터도 반환(RecipeData 구조체)
    /// recipe.recipeName, recipe.category 같은 정보 접근 가능
    /// 
    /// Input : 레시피 이름 ex) "계란볶음밥"
    /// Output : 레시피 단계 리스트와 메타데이터를 포함하는 dict
    /// recipeId, recipeName, category, fulIngredients, steps(레시피단계 리스트)로 구성
    /// </summary>
    public RecipeData GetRecipe(string recipeName)
    {
        return allRecipes.Find(r => r.recipeName == recipeName);
    }

    /// <summary>
    /// GetRecipe()와 동일하지만 ID 기반
    /// 
    /// Input : 레시피 ID
    /// Output : GetRecipe과 동일
    /// </summary>
    public RecipeData GetRecipeById(int recipeId)
    {
        return allRecipes.Find(r => r.recipeId == recipeId);
    }

    /// <summary>
    /// DB에 있는 모든 레시피 목록 반환
    /// UI에서 레시피 선택 목록 만들 때 사용
    /// 
    /// output :  레시피 리스트, 각 원소는 RecipeData 구조체
    /// recipeId, recipeName, category, fullIngredients, steps 포함
    /// </summary>
    public List<RecipeData> GetAllRecipes()
    {
        return allRecipes;
    }

    /// <summary>
    /// DB에 있는 모든 레시피 목록 반환
    /// UI에서 레시피 선택 목록 만들 때 사용
    /// 
    /// output : 레시피 이름 리스트, 각 원소가 레시피 이름 문자열
    /// </summary>
    public List<string> GetAllRecipeNames()
    {
        List<string> names = new List<string>();
        foreach (var recipe in allRecipes)
        {
            names.Add(recipe.recipeName);
        }
        return names;
    }
}
