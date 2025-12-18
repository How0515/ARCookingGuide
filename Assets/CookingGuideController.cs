// using UnityEngine;
// using TMPro;
// using System.Collections.Generic;

// public class CookingGuideController : MonoBehaviour
// {
//     // 1️⃣ [추가] 타이머 스크립트를 연결할 변수 만들기
//     public TimerObject timerObject;
//     public static CookingGuideController Instance; // 어디서든 접근 가능하게 싱글톤 처리

//     [Header("Data")]
//     public List<RecipeStep> recipeSteps; // 인스펙터에서 단계별 내용 입력
//     public int currentStepIndex = 0;

//     [Header("References")]
//     public RecipeBoard recipeBoard;    // 중앙 메인 보드
//     public HeadHUD headHUD;            // 우측 상단 HUD
//     public TimerManager timerManager;  // 좌측 타이머 관리자
//     public UnityEngine.UI.RawImage rightScreenImage; // 우측 화면 (Raw Image)
//     public UnityEngine.Video.VideoPlayer videoPlayer; // 영상 재생기

//     [Header("UI References")]
//     public TextMeshProUGUI stepTitleText;   // 제목 텍스트
//     public TextMeshProUGUI descriptionText;

//     private void Awake()
//     {
//         if (Instance == null) Instance = this;
//         else Destroy(gameObject);
//     }

//     private void Start()
//     {
//         // 시작 시 0번 단계 보여주기
//         UpdateAllUI();
//     }

//     // [다음] 버튼이나 음성인식으로 호출할 함수
//     public void GoToNextStep()
//     {
//         if (currentStepIndex < recipeSteps.Count - 1)
//         {
//             currentStepIndex++;
//             UpdateAllUI();
//         }
//         else
//         {
//             Debug.Log("요리 완료!");
//             // 요리 완료 UI 띄우기 로직 추가 가능
//         }
//     }

//     // [음성 명령] "Start Timer"라고 말하면 실행
//     public void StartCurrentTimer()
//     {
//         // 데이터 안전 장치
//         if (recipeSteps == null || recipeSteps.Count == 0) return;

//         RecipeStep currentStep = recipeSteps[currentStepIndex];

//         // 현재 단계에 타이머 설정 시간이 있다면 타이머 생성
//         if (currentStep.timerSeconds > 0)
//         {
//             timerManager.SpawnTimer(currentStep.timerSeconds, currentStep.stepTitle);
//             Debug.Log("음성 명령으로 타이머를 시작했습니다.");
//         }
//         else
//         {
//             Debug.Log("현재 단계에는 타이머가 없습니다.");
//         }
//     }

//     // [이전] 단계 (필요시)
//     public void GoToPrevStep()
//     {
//         if (currentStepIndex > 0)
//         {
//             currentStepIndex--;
//             UpdateAllUI();
//         }
//     }

//     // 모든 UI 갱신 로직
//     void UpdateAllUI()
//     {
//         if (recipeSteps == null || recipeSteps.Count == 0) return;

//         // 2. 현재 단계 데이터 가져오기
//         RecipeStep currentStep = recipeSteps[currentStepIndex];

//         // =========================================================
//         // [기능 1] 레시피 보드: (기존 코드 유지)
//         // =========================================================
//         stepTitleText.text = $"Step {currentStepIndex + 1}: {currentStep.stepTitle}";

//         string fullDescription = "";
//         for (int i = 0; i < recipeSteps.Count; i++)
//         {
//             if (i == currentStepIndex)
//                 fullDescription += $"<color=yellow><b>▶ {recipeSteps[i].description}</b></color>\n\n";
//             else
//                 fullDescription += $"<color=#CCCCCC>{i + 1}. {recipeSteps[i].description}</color>\n\n";
//         }
//         descriptionText.text = fullDescription;

//         // =========================================================
//         // 🔥 [기능 2] 타이머 관리 (여기를 수정했습니다!)
//         // =========================================================
        
//         // 기존의 timerManager.SpawnTimer 대신 -> 우리가 만든 timerObject를 직접 제어합니다.
//         if (timerObject != null)
//         {
//             // DB에 설정된 시간이 있으면 (0보다 크면)
//             if (currentStep.timerSeconds > 0)
//             {
//                 timerObject.gameObject.SetActive(true);           // 타이머 켜기
//                 timerObject.Initialize(currentStep.timerSeconds); // 시간 주입 (자동 시작 대기)
//             }
//             else
//             {
//                 // 시간이 0이면 타이머 숨기기
//                 timerObject.gameObject.SetActive(false);
//             }
//         }

//         // =========================================================
//         // [기능 3] 비디오 플레이어 제어 (기존 코드 유지)
//         // =========================================================
//         if (currentStep.stepVideo != null)
//         {
//             rightScreenImage.gameObject.SetActive(true);
//             videoPlayer.clip = currentStep.stepVideo;
//             videoPlayer.Play();
//         }
//         else
//         {
//             videoPlayer.Stop();
//             rightScreenImage.gameObject.SetActive(false);
//         }

//         // =========================================================
//         // [기능 4] HUD 업데이트 (기존 코드 유지)
//         // =========================================================
//         if (headHUD != null)
//         {
//             float progress = (float)(currentStepIndex + 1) / (float)recipeSteps.Count;
//             headHUD.UpdateHUD(currentStepIndex + 1, recipeSteps.Count, currentStep.stepTitle, progress);
//         }
//     }
// }

// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Collections.Generic;

// public class CookingGuideController : MonoBehaviour
// {
//     public static CookingGuideController Instance;

//     [Header("Data")]
//     public List<RecipeStep> recipeSteps = new List<RecipeStep>();
//     public int currentStepIndex = 0;

//     [Header("UI References")]
//     public TextMeshProUGUI stepTitleText;
//     public TextMeshProUGUI descriptionText;
//     public RawImage stepImage; // hiển thị GIF/ảnh

//     [Header("Timer")]
//     public TimerObject timerObject;

//     [Header("HUD & Video")]
//     public HeadHUD headHUD;
//     public RawImage rightScreenImage;
//     public UnityEngine.Video.VideoPlayer videoPlayer;

//     [Header("Filter (optional)")]
//     public string currentRecipe = ""; // nếu muốn lọc theo recipe

//     private void Awake()
//     {
//         if (Instance == null) Instance = this;
//         else Destroy(gameObject);
//     }

//     private void Start()
//     {
//         LoadCSV("RecipeDB/recipe_tips");
//         UpdateAllUI();
//     }

//     // =========================
//     // Load CSV từ Resources
//     // =========================
//     void LoadCSV(string path)
//     {
//         TextAsset csvFile = Resources.Load<TextAsset>(path);
//         if (csvFile == null)
//         {
//             Debug.LogError("Không tìm thấy file CSV: " + path);
//             return;
//         }

//         string[] lines = csvFile.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

//         for (int i = 1; i < lines.Length; i++) // bỏ header
//         {
//             string line = lines[i];
//             string[] values = line.Split(';');
//             for (int j = 0; j < values.Length; j++)
//                 values[j] = values[j].Trim();

//             if (values.Length < 4) continue;

//             RecipeStep step = new RecipeStep
//             {
//                 tipId = values[0],
//                 recipe = values[1],
//                 stepTitle = values[2],
//                 description = values[3],
//                 gifFile = (values.Length >= 5) ? values[4] : "",
//                 timerSeconds = 0
//             };

//             if (string.IsNullOrEmpty(currentRecipe) || step.recipe == currentRecipe)
//                 recipeSteps.Add(step);
//         }

//         if (recipeSteps.Count == 0)
//             Debug.LogWarning("Không có step nào được load từ CSV.");
//         else
//             Debug.Log("Số step load được: " + recipeSteps.Count);
//     }

//     // =========================
//     // Update UI
//     // =========================
//     void UpdateAllUI()
//     {
//         if (recipeSteps.Count == 0) return;

//         RecipeStep currentStep = recipeSteps[currentStepIndex];

//         // Title
//         if (stepTitleText != null)
//             stepTitleText.text = $"Step {currentStepIndex + 1}: {currentStep.stepTitle}";

//         // Description
//         if (descriptionText != null)
//         {
//             string fullDescription = "";
//             for (int i = 0; i < recipeSteps.Count; i++)
//             {
//                 if (i == currentStepIndex)
//                     fullDescription += $"<color=yellow><b>▶ {recipeSteps[i].description}</b></color>\n\n";
//                 else
//                     fullDescription += $"<color=#CCCCCC>{i + 1}. {recipeSteps[i].description}</color>\n\n";
//             }
//             descriptionText.text = fullDescription;
//         }

//         // GIF/Ảnh
//         if (stepImage != null && !string.IsNullOrEmpty(currentStep.gifFile))
//         {
//             Sprite sprite = Resources.Load<Sprite>("RecipeDB/GIF/" + currentStep.gifFile);
//             if (sprite != null)
//             {
//                 stepImage.gameObject.SetActive(true);
//                 stepImage.texture = sprite.texture;
//             }
//             else
//             {
//                 stepImage.gameObject.SetActive(false);
//                 Debug.LogWarning("Không tìm thấy GIF: " + currentStep.gifFile);
//             }
//         }
//         else if(stepImage != null)
//             stepImage.gameObject.SetActive(false);

//         // Timer
//         if (timerObject != null)
//         {
//             if (currentStep.timerSeconds > 0)
//             {
//                 timerObject.gameObject.SetActive(true);
//                 timerObject.Initialize(currentStep.timerSeconds);
//             }
//             else
//             {
//                 timerObject.gameObject.SetActive(false);
//             }
//         }

//         // Video
//         if (currentStep.stepVideo != null && videoPlayer != null && rightScreenImage != null)
//         {
//             rightScreenImage.gameObject.SetActive(true);
//             videoPlayer.clip = currentStep.stepVideo;
//             videoPlayer.Play();
//         }
//         else if(rightScreenImage != null && videoPlayer != null)
//         {
//             rightScreenImage.gameObject.SetActive(false);
//             videoPlayer.Stop();
//         }

//         // HUD
//         if(headHUD != null)
//         {
//             float progress = (float)(currentStepIndex + 1) / recipeSteps.Count;
//             headHUD.UpdateHUD(currentStepIndex + 1, recipeSteps.Count, currentStep.stepTitle, progress);
//         }
//     }

//     // =========================
//     // Step control
//     // =========================
//     public void NextStep()
//     {
//         if (currentStepIndex < recipeSteps.Count - 1)
//         {
//             currentStepIndex++;
//             UpdateAllUI();
//         }
//     }

//     public void PrevStep()
//     {
//         if (currentStepIndex > 0)
//         {
//             currentStepIndex--;
//             UpdateAllUI();
//         }
//     }

//     public void GoToNextStep() => NextStep();
//     public void GoToPrevStep() => PrevStep();
// }


// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Collections.Generic;
// using System.IO;

// // [System.Serializable]
// public class RecipeStep
// {
//     public string tipId;        // tip_id từ CSV
//     public string recipe;       // recipe từ CSV
//     public string stepTitle;    // tip_name
//     [TextArea] public string description; // tip_description
//     public string gifFile;      // tên file GIF trong Resources/RecipeDB/GIF
//     public float timerSeconds;  // nếu muốn thêm timer
// }

// public class CookingGuideController : MonoBehaviour
// {
//     public static CookingGuideController Instance;

//     [Header("Data")]
//     public List<RecipeStep> recipeSteps = new List<RecipeStep>();
//     public int currentStepIndex = 0;

//     [Header("UI References")]
//     public TextMeshProUGUI stepTitleText;
//     public TextMeshProUGUI descriptionText;
//     public RawImage rightScreenImage;

//     [Header("Timer")]
//     public TimerManager timerManager; // hoặc TimerObject nếu dùng prefab

//     [Header("HUD")]
//     public HeadHUD headHUD;

//     private void Awake()
//     {
//         if (Instance == null) Instance = this;
//         else Destroy(gameObject);
//     }

//     private void Start()
//     {
//         // Load CSV từ Resources
//         LoadCSV("RecipeDB/recipe_tips");

//         // Update UI lần đầu
//         UpdateAllUI();
//     }

//     // ========================
//     // Load CSV từ Resources
//     // ========================
//     public void LoadCSV(string csvPath)
//     {
//         TextAsset data = Resources.Load<TextAsset>(csvPath); // không cần .csv
//         if (data == null)
//         {
//             Debug.LogError("Không tìm thấy file CSV: " + csvPath);
//             return;
//         }

//         string[] lines = data.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
//         recipeSteps.Clear();

//         for (int i = 1; i < lines.Length; i++) // bỏ header
//         {
//             string[] cols = lines[i].Split(';'); // CSV ngăn cách bằng ;

//             if (cols.Length < 5)
//             {
//                 Debug.LogWarning("Dòng CSV thiếu cột: " + lines[i]);
//                 continue;
//             }

//             RecipeStep step = new RecipeStep
//             {
//                 tipId = cols[0],
//                 recipe = cols[1],
//                 stepTitle = cols[2],
//                 description = cols[3],
//                 gifFile = cols[4],
//                 timerSeconds = 0 // nếu muốn set thời gian từ CSV, thêm cột tương ứng
//             };

//             recipeSteps.Add(step);
//         }

//         Debug.Log("Số step load được: " + recipeSteps.Count);
//     }

//     // ========================
//     // Chuyển bước tiếp theo
//     // ========================
//     public void GoToNextStep()
//     {
//         if (currentStepIndex < recipeSteps.Count - 1)
//         {
//             currentStepIndex++;
//             UpdateAllUI();
//         }
//         else
//         {
//             Debug.Log("Cooking Complete!");
//         }
//     }

//     // ========================
//     // Chuyển bước trước
//     // ========================
//     public void GoToPrevStep()
//     {
//         if (currentStepIndex > 0)
//         {
//             currentStepIndex--;
//             UpdateAllUI();
//         }
//     }

//     // ========================
//     // Update UI
//     // ========================
//     private void UpdateAllUI()
//     {
//         if (recipeSteps == null || recipeSteps.Count == 0) return;

//         RecipeStep currentStep = recipeSteps[currentStepIndex];

//         // --- Title ---
//         stepTitleText.text = $"Step {currentStepIndex + 1}: {currentStep.stepTitle}";

//         // --- Description ---
//         string fullDescription = "";
//         for (int i = 0; i < recipeSteps.Count; i++)
//         {
//             if (i == currentStepIndex)
//                 fullDescription += $"<color=yellow><b>▶ {recipeSteps[i].description}</b></color>\n\n";
//             else
//                 fullDescription += $"<color=#CCCCCC>{i + 1}. {recipeSteps[i].description}</color>\n\n";
//         }
//         descriptionText.text = fullDescription;

//         // --- GIF Image ---
//         if (!string.IsNullOrEmpty(currentStep.gifFile))
//         {
//             Sprite sprite = Resources.Load<Sprite>("RecipeDB/GIF/" + currentStep.gifFile);
//             if (sprite != null)
//             {
//                 rightScreenImage.texture = sprite.texture;
//                 rightScreenImage.gameObject.SetActive(true);
//             }
//             else
//             {
//                 Debug.LogWarning("Không tìm thấy GIF: " + currentStep.gifFile);
//                 rightScreenImage.gameObject.SetActive(false);
//             }
//         }
//         else
//         {
//             rightScreenImage.gameObject.SetActive(false);
//         }

//         // --- Timer ---
//         if (currentStep.timerSeconds > 0 && timerManager != null)
//         {
//             timerManager.SpawnTimer(currentStep.timerSeconds, currentStep.stepTitle);
//         }

//         // --- HUD ---
//         if (headHUD != null)
//         {
//             float progress = (float)(currentStepIndex + 1) / recipeSteps.Count;
//             headHUD.UpdateHUD(currentStepIndex + 1, recipeSteps.Count, currentStep.stepTitle, progress);
//         }
//     }
// }


// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Collections.Generic;

// public class CookingGuideController : MonoBehaviour
// {
//     [System.Serializable]
//     public class RecipeStep
//     {
//         public string tipId;
//         public string recipe;
//         public string stepTitle;       // 단계 제목
//         [TextArea] public string description;
//         public string gifFile;
//         public float timerSeconds;     // Nếu muốn set timer
//     }

//     public static CookingGuideController Instance;

//     [Header("Data")]
//     public List<RecipeStep> recipeSteps = new List<RecipeStep>();
//     public int currentStepIndex = 0;

//     [Header("UI References")]
//     public TextMeshProUGUI stepTitleText;
//     public TextMeshProUGUI descriptionText;
//     public RawImage rightScreenImage;

//     [Header("Timer & HUD")]
//     public TimerManager timerManager;
//     public HeadHUD headHUD;

//     private void Awake()
//     {
//         if (Instance == null) Instance = this;
//         else Destroy(gameObject);
//     }

//     private void Start()
//     {
//         LoadCSV("RecipeDB/recipe_tips"); // CSV nằm trong Resources/RecipeDB/
//         UpdateAllUI();
//     }

//     #region CSV Loader
//     public void LoadCSV(string csvPath)
//     {
//         TextAsset data = Resources.Load<TextAsset>(csvPath);
//         if (data == null)
//         {
//             Debug.LogError("Không tìm thấy file CSV: " + csvPath);
//             return;
//         }

//         string[] lines = data.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
//         recipeSteps.Clear();

//         for (int i = 1; i < lines.Length; i++) // Bỏ header
//         {
//             string[] cols = lines[i].Split(';');
//             if (cols.Length < 5) continue;

//             RecipeStep step = new RecipeStep
//             {
//                 tipId = cols[0],
//                 recipe = cols[1],
//                 stepTitle = cols[2],       // tip_name
//                 description = cols[3],     // tip_description
//                 gifFile = cols[4],
//                 timerSeconds = 0
//             };

//             recipeSteps.Add(step);
//         }

//         Debug.Log("Số step load được: " + recipeSteps.Count);
//     }
//     #endregion

//     #region Step Control
//     public void GoToNextStep()
//     {
//         if (currentStepIndex < recipeSteps.Count - 1)
//         {
//             currentStepIndex++;
//             UpdateAllUI();
//         }
//         else
//         {
//             Debug.Log("Cooking Complete!");
//         }
//     }

//     public void GoToPrevStep()
//     {
//         if (currentStepIndex > 0)
//         {
//             currentStepIndex--;
//             UpdateAllUI();
//         }
//     }
//     #endregion

//     #region UI Update
//     private void UpdateAllUI()
//     {
//         if (recipeSteps == null || recipeSteps.Count == 0) return;

//         RecipeStep currentStep = recipeSteps[currentStepIndex];

//         // --- Title ---
//         if (stepTitleText != null)
//             stepTitleText.text = $"Step {currentStepIndex + 1}: {currentStep.stepTitle}";

//         // --- Description ---
//         if (descriptionText != null)
//         {
//             string fullDescription = "";
//             for (int i = 0; i < recipeSteps.Count; i++)
//             {
//                 if (i == currentStepIndex)
//                     fullDescription += $"<color=yellow><b>▶ {recipeSteps[i].description}</b></color>\n\n";
//                 else
//                     fullDescription += $"<color=#CCCCCC>{i + 1}. {recipeSteps[i].description}</color>\n\n";
//             }
//             descriptionText.text = fullDescription;
//         }

//         // --- GIF Image ---
//         if (!string.IsNullOrEmpty(currentStep.gifFile) && rightScreenImage != null)
//         {
//             Sprite sprite = Resources.Load<Sprite>("RecipeDB/GIF/" + currentStep.gifFile);
//             if (sprite != null)
//             {
//                 rightScreenImage.texture = sprite.texture;
//                 rightScreenImage.gameObject.SetActive(true);
//             }
//             else
//             {
//                 rightScreenImage.gameObject.SetActive(false);
//             }
//         }
//         else if (rightScreenImage != null)
//         {
//             rightScreenImage.gameObject.SetActive(false);
//         }

//         // --- Timer ---
//         if (currentStep.timerSeconds > 0 && timerManager != null)
//         {
//             timerManager.SpawnTimer(currentStep.timerSeconds, currentStep.stepTitle);
//         }

//         // --- HUD ---
//         if (headHUD != null)
//         {
//             float progress = (float)(currentStepIndex + 1) / recipeSteps.Count;
//             headHUD.UpdateHUD(currentStepIndex + 1, recipeSteps.Count, currentStep.stepTitle, progress);
//         }
//     }
//     #endregion
// }



using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class RecipeStep
{
    public string tip_id;
    public string recipe;
    public string stepTitle;      // map từ tip_name
    [TextArea] public string description;  // map từ tip_description
    public string gif;            // tên file gif/ảnh trong Resources/RecipeDB/GIF
}

public class CookingGuideController : MonoBehaviour
{
    public static CookingGuideController Instance;

    [Header("Data")]
    public List<RecipeStep> recipeSteps = new List<RecipeStep>();
    public int currentStepIndex = 0;

    [Header("UI References")]
    public TextMeshProUGUI stepTitleText;
    public TextMeshProUGUI descriptionText;
    public RawImage stepImage;

    [Header("Timer & Video")]
    public TimerManager timerManager;       
    // public VideoPlayer videoPlayer; // comment nếu không dùng video

    [Header("HUD")]
    public HeadHUD headHUD;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadCSV();
        UpdateAllUI();
    }

    void LoadCSV()
    {
        TextAsset data = Resources.Load<TextAsset>("RecipeDB/recipe_tips");
        if (data == null)
        {
            Debug.LogError("Không tìm thấy file recipe_tips.csv trong Resources!");
            return;
        }

        string[] lines = data.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] cols = line.Split(',');

            if (cols.Length < 4) continue;

            RecipeStep step = new RecipeStep();
            step.tip_id = cols[0];
            step.recipe = cols[1];
            step.stepTitle = cols[2];          
            step.description = cols[3];        
            step.gif = cols.Length > 4 ? cols[4] : "";

            recipeSteps.Add(step);
        }

        Debug.Log("Số step load được: " + recipeSteps.Count);
    }

    public void GoToNextStep()
    {
        if (currentStepIndex < recipeSteps.Count - 1)
        {
            currentStepIndex++;
            UpdateAllUI();
        }
        else
        {
            Debug.Log("Đã hoàn thành tất cả các bước!");
        }
    }

    public void GoToPrevStep()
    {
        if (currentStepIndex > 0)
        {
            currentStepIndex--;
            UpdateAllUI();
        }
    }

    void UpdateAllUI()
    {
        if (recipeSteps == null || recipeSteps.Count == 0) return;

        RecipeStep currentStep = recipeSteps[currentStepIndex];

        // Title
        if (stepTitleText != null)
            stepTitleText.text = $"Step {currentStepIndex + 1}: {currentStep.stepTitle}";

        // Description
        if (descriptionText != null)
        {
            string fullDescription = "";
            for (int i = 0; i < recipeSteps.Count; i++)
            {
                if (i == currentStepIndex)
                    fullDescription += $"<color=yellow><b>▶ {recipeSteps[i].description}</b></color>\n\n";
                else
                    fullDescription += $"<color=#CCCCCC>{i + 1}. {recipeSteps[i].description}</color>\n\n";
            }
            descriptionText.text = fullDescription;
        }

        // Load GIF / ảnh
        if (stepImage != null)
        {
            if (!string.IsNullOrEmpty(currentStep.gif))
            {
                Texture2D tex = Resources.Load<Texture2D>($"RecipeDB/GIF/{currentStep.gif}");
                if (tex != null)
                {
                    stepImage.texture = tex;
                    stepImage.gameObject.SetActive(true);
                }
                else
                {
                    stepImage.gameObject.SetActive(false);
                    Debug.LogWarning("Không tìm thấy GIF: " + currentStep.gif);
                }
            }
            else
            {
                stepImage.gameObject.SetActive(false);
            }
        }

        // Timer
        if (timerManager != null)
        {
            timerManager.ResetTimer(); 
        }

        // HUD
        if (headHUD != null)
        {
            float progress = (float)(currentStepIndex + 1) / recipeSteps.Count;
            headHUD.UpdateHUD(currentStepIndex + 1, recipeSteps.Count, currentStep.stepTitle, progress);
        }
    }
}
