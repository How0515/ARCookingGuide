// using UnityEngine;
// using TMPro;
// using UnityEngine.UI;
// using System.Linq;

// public class RecipeManager : MonoBehaviour
// {
//     [Header("=== UI References (kéo từ HUD vào đây) ===")]
//     public TextMeshProUGUI txtRecipeName;
//     public TextMeshProUGUI txtStepTitle;
//     public TextMeshProUGUI txtDescription;
//     public TextMeshProUGUI txtIngredients;
//     public TextMeshProUGUI txtTimer;
//     public TextMeshProUGUI txtTip;
//     public Image imgStep;

//     [Header("=== Settings ===")]
//     public int startRecipeId = 1;           // món đầu tiên khi chạy Play

//     private RecipeDatabase db;
//     private Recipe currentRecipe;
//     private int currentStepIndex = 0;
//     private TimerHUD timerHUD;



//     void Awake()
//     {
//         timerHUD = GetComponent<TimerHUD>();
//         LoadDatabase();
//     }

//     void Start()
//     {
//         StartRecipe(startRecipeId);
//     }

//     void LoadDatabase()
//     {
//         TextAsset json = Resources.Load<TextAsset>("recipeDB");
//         db = JsonUtility.FromJson<RecipeDatabase>(json.text);
//     }

//     public void StartRecipe(int recipeId)
//     {
//         currentRecipe = db.recipes.Find(r => r.id == recipeId);
//         currentStepIndex = 0;
//         txtRecipeName.text = currentRecipe.name_kr;
//         txtIngredients.text = "재료: " + string.Join(", ", currentRecipe.ingredients);
//         ShowCurrentStep();
//     }

//     public void NextStep()
//     {
//         if (currentStepIndex < currentRecipe.steps.Count - 1)
//         {
//             currentStepIndex++;
//             ShowCurrentStep();
//         }
//     }

//     public void PreviousStep()
//     {
//         if (currentStepIndex > 0)
//         {
//             currentStepIndex--;
//             ShowCurrentStep();
//         }
//     }

//     void ShowCurrentStep()
//     {
//         var step = currentRecipe.steps[currentStepIndex];
//         txtStepTitle.text = $"단계 {currentStepIndex + 1}: {step.title}";
//         txtDescription.text = step.description;
//         txtTip.text = "팁: " + step.tip;

//         // Load ảnh (đặt ảnh trong Assets/RecipeDB/Images/step11.jpg …)
//         Sprite sprite = Resources.Load<Sprite>("RecipeDB/Images/" + step.imageName);
//         if (sprite != null) imgStep.sprite = sprite;

//         // Timer
//         if (step.timerSeconds > 0)
//             timerHUD.StartTimer(step.timerSeconds);
//         else
//             timerHUD.StopTimer();
//     }

//     // gọi từ VoiceController khi nói "팁 보여줘"
//     public void ShowRandomTip()
//     {
//         if (db.globalTips.Count > 0)
//         {
//             string tip = db.globalTips[Random.Range(0, db.globalTips.Count)];
//             txtTip.text = "팁: " + tip;
//         }
//     }
// }


//2


// using UnityEngine;
// using TMPro;
// using UnityEngine.UI;
// using System.Collections.Generic;

// public class RecipeManager : MonoBehaviour
// {
//     private TextMeshProUGUI txtRecipeName;
//     private TextMeshProUGUI txtStepTitle;
//     private TextMeshProUGUI txtIngredients;
//     private TextMeshProUGUI txtDescription;
//     private TextMeshProUGUI txtTimer;
//     private TextMeshProUGUI txtTip;
//     private Image imgStep;

//     private TimerHUD timerHUD;
//     private RecipeDatabase db;
//     private Recipe currentRecipe;
//     private int currentStepIndex = 0;

//     void Awake()
//     {
//         txtRecipeName   = GameObject.Find("txtRecipeName").GetComponent<TextMeshProUGUI>();
//         txtStepTitle    = GameObject.Find("txtStepTitle").GetComponent<TextMeshProUGUI>();
//         txtIngredients  = GameObject.Find("txtIngredients").GetComponent<TextMeshProUGUI>();
//         txtDescription  = GameObject.Find("txtDescription").GetComponent<TextMeshProUGUI>();
//         txtTimer        = GameObject.Find("txtTimer").GetComponent<TextMeshProUGUI>();
//         txtTip          = GameObject.Find("txtTip").GetComponent<TextMeshProUGUI>();
//         imgStep         = GameObject.Find("imgStep").GetComponent<Image>();

//         timerHUD = GetComponent<TimerHUD>();
//         LoadDatabase();
//         DisplayCurrentStep();
//     }

//     void LoadDatabase()
//     {
//         TextAsset jsonFile = Resources.Load<TextAsset>("recipeDB");
//         if (jsonFile == null) { Debug.LogError("Không tìm thấy recipeDB.json!"); return; }
//         db = JsonUtility.FromJson<RecipeDatabase>(jsonFile.text);
//         currentRecipe = db.recipes[0];
//     }

//     public void NextStep() { if (currentStepIndex < currentRecipe.steps.Count - 1) { currentStepIndex++; DisplayCurrentStep(); } }
//     public void PreviousStep() { if (currentStepIndex > 0) { currentStepIndex--; DisplayCurrentStep(); } }
//     public void ShowRandomTip()
//     {
//         if (db.globalTips.Count > 0)
//             txtTip.text = "팁: " + db.globalTips[Random.Range(0, db.globalTips.Count)];
//     }

//     void DisplayCurrentStep()
//     {
//         var step = currentRecipe.steps[currentStepIndex];
//         txtRecipeName.text  = $"{currentRecipe.name_kr}\n({currentRecipe.name_vi})";
//         txtStepTitle.text   = $"단계 {currentStepIndex + 1}: {step.title}";
//         txtDescription.text = step.description;
//         txtIngredients.text = "재료: " + string.Join(" • ", currentRecipe.ingredients);
//         txtTip.text = step.tip != "" ? "팁: " + step.tip : "";

//         if (step.timerSeconds > 0)
//         {
//             timerHUD.StartTimer(step.timerSeconds);
//             txtTimer.gameObject.SetActive(true);
//         }
//         else txtTimer.gameObject.SetActive(false);
//     }
// }

//3 

// using UnityEngine;
// using TMPro;
// using UnityEngine.UI;
// using System.Collections.Generic;

// public class RecipeManager : MonoBehaviour
// {
//     private TextMeshProUGUI txtRecipeName;
//     private TextMeshProUGUI txtStepTitle;
//     private TextMeshProUGUI txtIngredients;
//     private TextMeshProUGUI txtDescription;
//     private TextMeshProUGUI txtTimer;
//     private TextMeshProUGUI txtTip;
//     private Image imgStep;

//     private TimerHUD timerHUD;
//     private RecipeDatabase db;
//     private Recipe currentRecipe;
//     private int currentStepIndex = 0;

//     void Awake()
//     {
//         txtRecipeName   = GameObject.Find("txtRecipeName").GetComponent<TextMeshProUGUI>();
//         txtStepTitle    = GameObject.Find("txtStepTitle").GetComponent<TextMeshProUGUI>();
//         txtIngredients  = GameObject.Find("txtIngredients").GetComponent<TextMeshProUGUI>();
//         txtDescription  = GameObject.Find("txtDescription").GetComponent<TextMeshProUGUI>();
//         txtTimer        = GameObject.Find("txtTimer").GetComponent<TextMeshProUGUI>();
//         txtTip          = GameObject.Find("txtTip").GetComponent<TextMeshProUGUI>();
//         imgStep         = GameObject.Find("imgStep").GetComponent<Image>();

//         timerHUD = GetComponent<TimerHUD>();

//         LoadDatabase();
//         DisplayCurrentStep();
//     }

//     // ----------------------
//     // SỬA PHẦN LOAD DATA
//     // ----------------------
//     void LoadDatabase()
//     {
//         TextAsset jsonFile = Resources.Load<TextAsset>("recipeDB");

//         if (jsonFile == null)
//         {
//             Debug.LogError("❌ Không tìm thấy recipeDB.json trong thư mục Resources!");
//             return;
//         }

//         db = JsonUtility.FromJson<RecipeDatabase>(jsonFile.text);

//         if (db == null || db.recipes == null || db.recipes.Count == 0)
//         {
//             Debug.LogError("❌ JSON không hợp lệ hoặc không có recipes!");
//             return;
//         }

//         currentRecipe = db.recipes[0];
//         currentStepIndex = 0;

//         Debug.Log("✅ Loaded recipe: " + currentRecipe.name_vi);
//     }

//     // ----------------------

//     public void NextStep()
//     {
//         if (currentStepIndex < currentRecipe.steps.Count - 1)
//         {
//             currentStepIndex++;
//             DisplayCurrentStep();
//         }
//     }

//     public void PreviousStep()
//     {
//         if (currentStepIndex > 0)
//         {
//             currentStepIndex--;
//             DisplayCurrentStep();
//         }
//     }

//     public void ShowRandomTip()
//     {
//         if (db != null && db.globalTips != null && db.globalTips.Count > 0)
//             txtTip.text = "팁: " + db.globalTips[Random.Range(0, db.globalTips.Count)];
//     }

//     // ----------------------
//     // SỬA HIỂN THỊ STEP
//     // ----------------------
//     void DisplayCurrentStep()
//     {
//         if (currentRecipe == null) return;

//         var step = currentRecipe.steps[currentStepIndex];

//         txtRecipeName.text  = $"{currentRecipe.name_kr}\n({currentRecipe.name_vi})";
//         txtStepTitle.text   = $"단계 {currentStepIndex + 1}: {step.title}";
//         txtDescription.text = step.description;
//         txtIngredients.text = "재료: " + string.Join(" • ", currentRecipe.ingredients);

//         // Tip Step
//         txtTip.text = string.IsNullOrEmpty(step.tip) ? "" : "팁: " + step.tip;

//         // ----------- LOAD ẢNH -----------
//         if (!string.IsNullOrEmpty(step.imageName))
//         {
//             Sprite s = Resources.Load<Sprite>("RecipeImages/" + step.imageName);
//             if (s != null) imgStep.sprite = s;
//             else Debug.LogWarning("⚠ Không tìm thấy ảnh: " + step.imageName);
//         }

//         // ---------- TIMER ----------
//         if (step.timerSeconds > 0)
//         {
//             timerHUD.StartTimer(step.timerSeconds);
//             txtTimer.gameObject.SetActive(true);
//         }
//         else
//         {
//             txtTimer.gameObject.SetActive(false);
//         }
//     }
// }


// 4

// using UnityEngine;
// using TMPro;
// using UnityEngine.UI;
// using System.Collections.Generic;

// public class RecipeManager : MonoBehaviour
// {
//     private TextMeshProUGUI txtRecipeName;
//     private TextMeshProUGUI txtStepTitle;
//     private TextMeshProUGUI txtIngredients;
//     private TextMeshProUGUI txtDescription;
//     private TextMeshProUGUI txtTimer;
//     private TextMeshProUGUI txtTip;
//     private Image imgStep;

//     private TimerHUD timerHUD;
//     private RecipeDatabase db;
//     private Recipe currentRecipe;
//     private int currentStepIndex = 0;

//     void Awake()
//     {
//         txtRecipeName   = GameObject.Find("txtRecipeName").GetComponent<TextMeshProUGUI>();
//         txtStepTitle    = GameObject.Find("txtStepTitle").GetComponent<TextMeshProUGUI>();
//         txtIngredients  = GameObject.Find("txtIngredients").GetComponent<TextMeshProUGUI>();
//         txtDescription  = GameObject.Find("txtDescription").GetComponent<TextMeshProUGUI>();
//         txtTimer        = GameObject.Find("txtTimer").GetComponent<TextMeshProUGUI>();
//         txtTip          = GameObject.Find("txtTip").GetComponent<TextMeshProUGUI>();
//         imgStep         = GameObject.Find("imgStep").GetComponent<Image>();

//         timerHUD = GetComponent<TimerHUD>();

//         LoadDatabase();
//         DisplayCurrentStep();
//     }

//     [System.Serializable]
//     private class Wrapper
//     {
//         public List<Recipe> recipes;
//         public List<string> globalTips;
//     }

//     void LoadDatabase()
//     {
//         TextAsset jsonFile = Resources.Load<TextAsset>("recipeDB_Json");

//         if (jsonFile == null)
//         {
//             Debug.LogError("❌ Không tìm thấy recipeDB_Json.json trong thư mục Resources!");
//             return;
//         }

//         Debug.Log("➡ JSON RAW:");
//         Debug.Log(jsonFile.text);

//         Wrapper w = JsonUtility.FromJson<Wrapper>(jsonFile.text);

//         if (w == null)
//         {
//             Debug.LogError("❌ Wrapper null → JSON lỗi format cho JsonUtility!");
//             return;
//         }

//         db = new RecipeDatabase
//         {
//             recipes = w.recipes,
//             globalTips = w.globalTips
//         };

//         if (db.recipes == null || db.recipes.Count == 0)
//         {
//             Debug.LogError("❌ Không có recipe nào trong JSON!");
//             return;
//         }

//         currentRecipe = db.recipes[0];
//         currentStepIndex = 0;

//         Debug.Log("✅ Loaded recipe: " + currentRecipe.name_vi);
//     }

//     public void NextStep()
//     {
//         if (currentStepIndex < currentRecipe.steps.Count - 1)
//         {
//             currentStepIndex++;
//             DisplayCurrentStep();
//         }
//     }

//     public void PreviousStep()
//     {
//         if (currentStepIndex > 0)
//         {
//             currentStepIndex--;
//             DisplayCurrentStep();
//         }
//     }

//     public void ShowRandomTip()
//     {
//         if (db != null && db.globalTips != null && db.globalTips.Count > 0)
//             txtTip.text = "팁: " + db.globalTips[Random.Range(0, db.globalTips.Count)];
//     }

//     void DisplayCurrentStep()
//     {
//         if (currentRecipe == null) return;

//         var step = currentRecipe.steps[currentStepIndex];

//         txtRecipeName.text  = $"{currentRecipe.name_kr}\n({currentRecipe.name_vi})";
//         txtStepTitle.text   = $"단계 {currentStepIndex + 1}: {step.title}";
//         txtDescription.text = step.description;
//         txtIngredients.text = "재료: " + string.Join(" • ", currentRecipe.ingredients);

//         txtTip.text = string.IsNullOrEmpty(step.tip) ? "" : "팁: " + step.tip;

//         if (!string.IsNullOrEmpty(step.imageName))
//         {
//             Sprite s = Resources.Load<Sprite>("RecipeImages/" + step.imageName);
//             if (s != null) imgStep.sprite = s;
//             else Debug.LogWarning("⚠ Không tìm thấy ảnh: " + step.imageName);
//         }

//         if (step.timerSeconds > 0)
//         {
//             timerHUD.StartTimer(step.timerSeconds);
//             txtTimer.gameObject.SetActive(true);
//         }
//         else
//         {
//             txtTimer.gameObject.SetActive(false);
//         }
//     }
// }


// 5

// using UnityEngine;
// using TMPro;
// using UnityEngine.UI;
// using System.Collections.Generic;

// public class RecipeManager : MonoBehaviour
// {
//     private TextMeshProUGUI txtRecipeName;
//     private TextMeshProUGUI txtStepTitle;
//     private TextMeshProUGUI txtIngredients;
//     private TextMeshProUGUI txtDescription;
//     private TextMeshProUGUI txtTimer;
//     private TextMeshProUGUI txtTip;
//     private Image imgStep;

//     private TimerHUD timerHUD;
//     private RecipeDatabase db;
//     private Recipe currentRecipe;
//     private int currentStepIndex = 0;

//     void Awake()
//     {
//         // --- Tìm UI tự động + chống NullReference ---
//         txtRecipeName   = FindTMP("txtRecipeName");
//         txtStepTitle    = FindTMP("txtStepTitle");
//         txtIngredients  = FindTMP("txtIngredients");
//         txtDescription  = FindTMP("txtDescription");
//         txtTimer        = FindTMP("txtTimer");
//         txtTip          = FindTMP("txtTip");
//         imgStep         = FindImg("imgStep");

//         timerHUD = GetComponent<TimerHUD>();

//         LoadDatabase();
//         DisplayCurrentStep();
//     }

//     // ===========================
//     // HELPER: Tìm TMP an toàn
//     // ===========================
//     private TextMeshProUGUI FindTMP(string name)
//     {
//         GameObject obj = GameObject.Find(name);

//         if (obj == null)
//         {
//             Debug.LogError("❌ Không tìm thấy object: " + name);
//             return null;
//         }

//         var tmp = obj.GetComponent<TextMeshProUGUI>();
//         if (tmp == null)
//             Debug.LogError("❌ Object '" + name + "' KHÔNG có TextMeshProUGUI!");

//         return tmp;
//     }

//     // ===========================
//     // HELPER: Tìm Image an toàn
//     // ===========================
//     private Image FindImg(string name)
//     {
//         GameObject obj = GameObject.Find(name);

//         if (obj == null)
//         {
//             Debug.LogError("❌ Không tìm thấy object: " + name);
//             return null;
//         }

//         var img = obj.GetComponent<Image>();
//         if (img == null)
//             Debug.LogError("❌ Object '" + name + "' KHÔNG có Image component!");

//         return img;
//     }


//     // ===========================
//     // LOAD DATABASE JSON
//     // ===========================
//     [System.Serializable]
//     private class Wrapper
//     {
//         public List<Recipe> recipes;
//         public List<string> globalTips;
//     }

//     void LoadDatabase()
//     {
//         TextAsset jsonFile = Resources.Load<TextAsset>("recipeDB_Json");

//         if (jsonFile == null)
//         {
//             Debug.LogError("❌ Không tìm thấy recipeDB_Json.json trong Resources!");
//             return;
//         }

//         Debug.Log("➡ JSON RAW:");
//         Debug.Log(jsonFile.text);

//         Wrapper w = JsonUtility.FromJson<Wrapper>(jsonFile.text);

//         if (w == null)
//         {
//             Debug.LogError("❌ JSON format sai — JsonUtility không đọc được!");
//             return;
//         }

//         db = new RecipeDatabase
//         {
//             recipes = w.recipes,
//             globalTips = w.globalTips
//         };

//         if (db.recipes == null || db.recipes.Count == 0)
//         {
//             Debug.LogError("❌ Không có recipe nào trong JSON!");
//             return;
//         }

//         currentRecipe = db.recipes[0];
//         currentStepIndex = 0;

//         Debug.Log("✅ Loaded recipe: " + currentRecipe.name_vi);
//     }


//     // ===========================
//     // STEP CHANGE
//     // ===========================
//     public void NextStep()
//     {
//         if (currentStepIndex < currentRecipe.steps.Count - 1)
//         {
//             currentStepIndex++;
//             DisplayCurrentStep();
//         }
//     }

//     public void PreviousStep()
//     {
//         if (currentStepIndex > 0)
//         {
//             currentStepIndex--;
//             DisplayCurrentStep();
//         }
//     }


//     // ===========================
//     // RANDOM TIP
//     // ===========================
//     public void ShowRandomTip()
//     {
//         if (db != null && db.globalTips != null && db.globalTips.Count > 0)
//             txtTip.text = "팁: " + db.globalTips[Random.Range(0, db.globalTips.Count)];
//     }


//     // ===========================
//     // DISPLAY CURRENT STEP
//     // ===========================
//     void DisplayCurrentStep()
//     {
//         if (currentRecipe == null)
//         {
//             Debug.LogError("❌ currentRecipe NULL → database chưa load?");
//             return;
//         }

//         var step = currentRecipe.steps[currentStepIndex];

//         if (txtRecipeName != null)
//             txtRecipeName.text = $"{currentRecipe.name_kr}\n({currentRecipe.name_vi})";

//         if (txtStepTitle != null)
//             txtStepTitle.text = $"단계 {currentStepIndex + 1}: {step.title}";

//         if (txtDescription != null)
//             txtDescription.text = step.description;

//         if (txtIngredients != null)
//             txtIngredients.text = "재료: " + string.Join(" • ", currentRecipe.ingredients);

//         if (txtTip != null)
//             txtTip.text = string.IsNullOrEmpty(step.tip) ? "" : "팁: " + step.tip;

//         // Load image
//         if (!string.IsNullOrEmpty(step.imageName) && imgStep != null)
//         {
//             Sprite s = Resources.Load<Sprite>("RecipeImages/" + step.imageName);
//             if (s != null) imgStep.sprite = s;
//             else Debug.LogWarning("⚠ Không tìm thấy ảnh: " + step.imageName);
//         }

//         // Timer
//         if (txtTimer != null)
//         {
//             if (step.timerSeconds > 0)
//             {
//                 timerHUD.StartTimer(step.timerSeconds);
//                 txtTimer.gameObject.SetActive(true);
//             }
//             else
//             {
//                 txtTimer.gameObject.SetActive(false);
//             }
//         }
//     }
// }


// 6

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class RecipeManager : MonoBehaviour
{
    private TextMeshProUGUI txtRecipeName;
    private TextMeshProUGUI txtStepTitle;
    private TextMeshProUGUI txtIngredients;
    private TextMeshProUGUI txtDescription;
    private TextMeshProUGUI txtTimer;
    private TextMeshProUGUI txtTip;
    private Image imgStep;

    private TimerHUD timerHUD;
    private RecipeDatabase db;
    private Recipe currentRecipe;
    private int currentStepIndex = 0;

    // ======================================================
    // AWAKE — FIND UI + LOAD DB
    // ======================================================
    void Awake()
    {
        // --- Auto find UI safely ---
        txtRecipeName   = FindTMP("txtRecipeName");
        txtStepTitle    = FindTMP("txtStepTitle");
        txtIngredients  = FindTMP("txtIngredients");
        txtDescription  = FindTMP("txtDescription");
        txtTimer        = FindTMP("txtTimer");
        txtTip          = FindTMP("txtTip");
        imgStep         = FindImg("imgStep");

        timerHUD = GetComponent<TimerHUD>();

        LoadDatabase();
        DisplayCurrentStep();
    }

    // ======================================================
    // SAFE FIND TMP
    // ======================================================
    private TextMeshProUGUI FindTMP(string name)
    {
        GameObject obj = GameObject.Find(name);

        if (obj == null)
        {
            Debug.LogError("❌ Không tìm thấy object: " + name);
            return null;
        }

        var tmp = obj.GetComponent<TextMeshProUGUI>();
        if (tmp == null)
            Debug.LogError("❌ Object '" + name + "' KHÔNG có TextMeshProUGUI!");

        return tmp;
    }

    // ======================================================
    // SAFE FIND IMAGE
    // ======================================================
    private Image FindImg(string name)
    {
        GameObject obj = GameObject.Find(name);

        if (obj == null)
        {
            Debug.LogError("❌ Không tìm thấy object: " + name);
            return null;
        }

        var img = obj.GetComponent<Image>();
        if (img == null)
            Debug.LogError("❌ Object '" + name + "' KHÔNG có Image component!");

        return img;
    }

    // ======================================================
    // JSON WRAPPER
    // ======================================================
    [System.Serializable]
    private class Wrapper
    {
        public List<Recipe> recipes;
        public List<string> globalTips;
    }

    // ======================================================
    // LOAD JSON DATABASE
    // ======================================================
    void LoadDatabase()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("recipeDB_Json");

        if (jsonFile == null)
        {
            Debug.LogError("❌ Không tìm thấy recipeDB_Json.json trong Resources!");
            return;
        }

        Debug.Log("➡ JSON RAW:");
        Debug.Log(jsonFile.text);

        Wrapper w = JsonUtility.FromJson<Wrapper>(jsonFile.text);

        if (w == null)
        {
            Debug.LogError("❌ JSON format sai — JsonUtility không đọc được!");
            return;
        }

        db = new RecipeDatabase
        {
            recipes = w.recipes,
            globalTips = w.globalTips
        };

        if (db.recipes == null || db.recipes.Count == 0)
        {
            Debug.LogError("❌ Không có recipe nào trong JSON!");
            return;
        }

        currentRecipe = db.recipes[0];
        currentStepIndex = 0;

        Debug.Log("✅ Loaded recipe: " + currentRecipe.name_vi);
    }

    // ======================================================
    // LOAD SPRITE (NEW)
    // Tự thử: no extension → .png → .jpg
    // ======================================================
    private Sprite LoadStepImage(string imageName)
    {
        if (string.IsNullOrEmpty(imageName))
        {
            Debug.LogWarning("⚠ step.imageName rỗng!");
            return null;
        }

        // thử không có extension
        Sprite sprite = Resources.Load<Sprite>("RecipeImages/" + imageName);

        // thử .png
        if (sprite == null)
            sprite = Resources.Load<Sprite>("RecipeImages/" + imageName + ".png");

        // thử .jpg
        if (sprite == null)
            sprite = Resources.Load<Sprite>("RecipeImages/" + imageName + ".jpg");

        if (sprite == null)
        {
            Debug.LogWarning("⚠ Không tìm thấy ảnh trong Resources/RecipeImages/: " + imageName);
        }

        return sprite;
    }

    // ======================================================
    // STEP NAVIGATION
    // ======================================================
    public void NextStep()
    {
        if (currentStepIndex < currentRecipe.steps.Count - 1)
        {
            currentStepIndex++;
            DisplayCurrentStep();
        }
    }

    public void PreviousStep()
    {
        if (currentStepIndex > 0)
        {
            currentStepIndex--;
            DisplayCurrentStep();
        }
    }

    // ======================================================
    // RANDOM TIP
    // ======================================================
    public void ShowRandomTip()
    {
        if (db != null && db.globalTips != null && db.globalTips.Count > 0)
            txtTip.text = "팁: " + db.globalTips[Random.Range(0, db.globalTips.Count)];
    }

    // ======================================================
    // DISPLAY STEP (UPDATED FULL)
    // ======================================================
    void DisplayCurrentStep()
    {
        if (currentRecipe == null)
        {
            Debug.LogError("❌ currentRecipe NULL → database chưa load?");
            return;
        }

        var step = currentRecipe.steps[currentStepIndex];

        if (txtRecipeName != null)
            txtRecipeName.text = $"{currentRecipe.name_kr}\n({currentRecipe.name_vi})";

        if (txtStepTitle != null)
            txtStepTitle.text = $"단계 {currentStepIndex + 1}: {step.title}";

        if (txtDescription != null)
            txtDescription.text = step.description;

        if (txtIngredients != null)
            txtIngredients.text = "재료: " + string.Join(" • ", currentRecipe.ingredients);

        if (txtTip != null)
            txtTip.text = string.IsNullOrEmpty(step.tip) ? "" : "팁: " + step.tip;

        // ===========================
        // IMAGE LOADING (NEW)
        // ===========================
        if (imgStep != null)
        {
            Sprite stepSprite = LoadStepImage(step.imageName);

            if (stepSprite != null)
                imgStep.sprite = stepSprite;
            else
                imgStep.sprite = null; // tránh giữ ảnh cũ
        }

        // ===========================
        // TIMER
        // ===========================
        if (txtTimer != null)
        {
            if (step.timerSeconds > 0)
            {
                timerHUD.StartTimer(step.timerSeconds);
                txtTimer.gameObject.SetActive(true);
            }
            else
            {
                txtTimer.gameObject.SetActive(false);
            }
        }
    }
}
