using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 레시피 데이터 관리자
/// - CSV 파일에서 레시피 데이터 로드
/// - 레시피별 이미지 로드
/// - 다른 컨트롤러에서 사용
/// </summary>
public class RecipeDataManager : MonoBehaviour
{
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
        public string fullIngredients;  // 전체 재료 (쉼표로 구분)
        public List<CookingStep> steps;
    }

    private static RecipeDataManager instance;
    private List<RecipeData> allRecipes = new List<RecipeData>();
    private Dictionary<int, Sprite[]> recipeImages = new Dictionary<int, Sprite[]>();

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
            recipe.steps = ParseSteps(recipe.recipeId, fields[4], fields[5]);

            allRecipes.Add(recipe);

            Debug.Log($"✅ 레시피 로드: {recipe.recipeName} ({recipe.steps.Count}단계)");
        }

        Debug.Log($"📚 총 {allRecipes.Count}개의 레시피 로드 완료");
    }

    /// <summary>
    /// CSV 라인을 필드로 분리 (세미콜론 처리)
    /// </summary>
    private string[] ParseCSVLine(string line)
    {
        // 간단한 파싱 (쉼표로 분리)
        return line.Split(',');
    }

    /// <summary>
    /// 단계별 데이터 파싱
    /// </summary>
    private List<CookingStep> ParseSteps(int recipeId, string stepsText, string imagesText)
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
            step.ingredients = "";  // 나중에 설정
            step.heatLevel = "";    // 나중에 설정
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
    /// 특정 레시피 가져오기
    /// </summary>
    public RecipeData GetRecipe(int recipeId)
    {
        return allRecipes.Find(r => r.recipeId == recipeId);
    }

    /// <summary>
    /// 모든 레시피 목록 가져오기
    /// </summary>
    public List<RecipeData> GetAllRecipes()
    {
        return allRecipes;
    }

    /// <summary>
    /// 레시피 이름으로 검색
    /// </summary>
    public RecipeData GetRecipeByName(string recipeName)
    {
        return allRecipes.Find(r => r.recipeName == recipeName);
    }
}
