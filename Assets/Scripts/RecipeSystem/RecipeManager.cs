using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class RecipesManager : MonoBehaviour
{
    // ================= DATA STRUCT =================
    [System.Serializable]
    public class Recipe
    {
        public string recipe_id;
        public string name;
        public string category;
        public string ingredients;
        public string steps;
        public string images;
    }

    // ================= CSV =================
    public string csvFileName = "recipeDB";
    private List<Recipe> recipes = new List<Recipe>();
    private Recipe currentRecipe;

    // ================= STEP =================
    private int currentStepIndex = 0;
    private List<string> stepList = new List<string>();
    private List<string> imageList = new List<string>();

    // ================= UI =================
    [Header("UI")]
    public TextMeshPro ingredientsText;
    public TextMeshPro stepDescriptionText;
    public Image stepImage;

    // ================= START =================
    void Start()
    {
        LoadRecipesFromCSV();

        if (recipes.Count == 0)
        {
            Debug.LogError("❌ No recipes loaded");
            return;
        }

        SelectFirstRecipe();
        UpdateUI();
    }

    // ================= SELECT =================
    void SelectFirstRecipe()
    {
        currentRecipe = recipes[0];
        currentStepIndex = 0;

        ParseSteps();
        ParseImages();
    }

    // ================= PARSE =================
    void ParseSteps()
    {
        stepList.Clear();

        string[] rawSteps = currentRecipe.steps.Split('\n');
        foreach (string s in rawSteps)
        {
            if (!string.IsNullOrWhiteSpace(s))
                stepList.Add(s.Trim());
        }
    }

    void ParseImages()
    {
        imageList.Clear();

        string[] rawImages = currentRecipe.images.Split(';');
        foreach (string img in rawImages)
        {
            if (!string.IsNullOrWhiteSpace(img))
                imageList.Add(img.Trim());
        }
    }

    // ================= UI =================
    void UpdateUI()
    {
        if (currentRecipe == null) return;

        ingredientsText.text = currentRecipe.ingredients;

        if (currentStepIndex < stepList.Count)
            stepDescriptionText.text = stepList[currentStepIndex];

        LoadStepImage();
    }

    void LoadStepImage()
    {
        if (stepImage == null) return;
        if (currentStepIndex >= imageList.Count) return;

        string imageName = imageList[currentStepIndex];
        string cleanName = Path.GetFileNameWithoutExtension(imageName);

        string path = "Images/" + cleanName;
        Debug.Log("🔍 Load image: " + path);

        Sprite sprite = Resources.Load<Sprite>(path);

        if (sprite == null)
        {
            Debug.LogError("❌ Image not found: " + path);
            return;
        }

        stepImage.sprite = sprite;
        stepImage.preserveAspect = true;
    }

    // ================= BUTTON =================
    public void NextStep()
    {
        if (currentStepIndex < stepList.Count - 1)
        {
            currentStepIndex++;
            UpdateUI();
        }
    }

    public void PreviousStep()
    {
        if (currentStepIndex > 0)
        {
            currentStepIndex--;
            UpdateUI();
        }
    }

    // ================= CSV LOADER =================
    void LoadRecipesFromCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>(csvFileName);

        if (csvFile == null)
        {
            Debug.LogError("❌ CSV NOT FOUND: " + csvFileName);
            return;
        }

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            List<string> cols = ParseCSVLine(lines[i]);
            if (cols.Count < 6) continue;

            recipes.Add(new Recipe
            {
                recipe_id = cols[0],
                name = cols[1],
                category = cols[2],
                ingredients = cols[3],
                steps = cols[4],
                images = cols[5]
            });
        }

        Debug.Log("✅ Loaded recipes: " + recipes.Count);
    }

    List<string> ParseCSVLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string current = "";

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (c == ',' && !inQuotes)
            {
                result.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }

        result.Add(current);
        return result;
    }
}
