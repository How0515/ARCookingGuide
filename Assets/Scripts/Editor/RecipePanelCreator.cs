using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;

/// <summary>
/// Unity 에디터에서 RecipePanel Prefab을 자동으로 생성하는 헬퍼
/// 메뉴: Tools > AR Cooking Guide > Create Recipe Panel Prefab
/// </summary>
public class RecipePanelCreator : EditorWindow
{
    private string recipeName = "떡볶이 레시피";
    private int stepCount = 6;
    private bool createMRTKComponents = true;

    [MenuItem("Tools/AR Cooking Guide/Create Recipe Panel Prefab")]
    public static void ShowWindow()
    {
        GetWindow<RecipePanelCreator>("Recipe Panel Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Recipe Panel Prefab 생성", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        recipeName = EditorGUILayout.TextField("레시피 이름:", recipeName);
        stepCount = EditorGUILayout.IntSlider("단계 수:", stepCount, 3, 12);
        createMRTKComponents = EditorGUILayout.Toggle("MRTK 컴포넌트 추가:", createMRTKComponents);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "이 도구는 새로운 RecipePanel.prefab을 생성합니다.\n" +
            "- World Space Canvas\n" +
            "- Header (제목 + 재료)\n" +
            "- Steps Container (단계 리스트)\n" +
            "- Progress Display (진행률)\n" +
            "- MRTK 컴포넌트 (선택)",
            MessageType.Info);

        EditorGUILayout.Space();

        if (GUILayout.Button("RecipePanel 프리팹 생성", GUILayout.Height(40)))
        {
            CreateRecipePanel();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("RecipeStepItem 프리팹만 생성", GUILayout.Height(30)))
        {
            CreateRecipeStepItemPrefab();
        }
    }

    private void CreateRecipePanel()
    {
        // 1. Root GameObject 생성
        GameObject root = new GameObject("RecipePanel");
        
        // 2. Canvas 추가 (World Space)
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;
        
        GraphicRaycaster raycaster = root.AddComponent<GraphicRaycaster>();
        
        // Canvas 크기 설정
        RectTransform canvasRect = root.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(800, 1000);
        canvasRect.localScale = new Vector3(0.001f, 0.001f, 0.001f); // 0.8m x 1.0m

        // 3. CanvasGroup 추가 (투명도 조절용)
        CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0.9f;

        // 4. Background Panel
        GameObject bgPanel = CreateUIObject("Background", root.transform);
        Image bgImage = bgPanel.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);
        RectTransform bgRect = bgPanel.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // 5. Header Section
        GameObject header = CreateUIObject("HeaderSection", root.transform);
        RectTransform headerRect = header.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0.5f, 1);
        headerRect.sizeDelta = new Vector2(-40, 120);
        headerRect.anchoredPosition = new Vector2(0, -20);

        // Recipe Name Text
        GameObject nameTextObj = CreateTextObject("RecipeNameText", header.transform, 32, TextAlignmentOptions.Center);
        TMP_Text nameText = nameTextObj.GetComponent<TMP_Text>();
        nameText.text = recipeName;
        nameText.fontStyle = FontStyles.Bold;
        nameText.color = Color.white;
        RectTransform nameRect = nameTextObj.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.5f);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.offsetMin = new Vector2(0, 0);
        nameRect.offsetMax = new Vector2(0, 0);

        // Ingredients Text
        GameObject ingredientsObj = CreateTextObject("IngredientsText", header.transform, 20, TextAlignmentOptions.TopLeft);
        TMP_Text ingredientsText = ingredientsObj.GetComponent<TMP_Text>();
        ingredientsText.text = "재료: 떡, 어묵, 대파, 고춧가루, 고추장, 간장, 설탕...";
        ingredientsText.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        RectTransform ingredientsRect = ingredientsObj.GetComponent<RectTransform>();
        ingredientsRect.anchorMin = new Vector2(0, 0);
        ingredientsRect.anchorMax = new Vector2(1, 0.5f);
        ingredientsRect.offsetMin = new Vector2(10, 0);
        ingredientsRect.offsetMax = new Vector2(-10, -5);

        // 6. Steps Container
        GameObject stepsContainer = CreateUIObject("StepsContainer", root.transform);
        RectTransform stepsRect = stepsContainer.GetComponent<RectTransform>();
        stepsRect.anchorMin = new Vector2(0, 0.15f);
        stepsRect.anchorMax = new Vector2(1, 1);
        stepsRect.pivot = new Vector2(0.5f, 1);
        stepsRect.anchoredPosition = new Vector2(0, -150);
        stepsRect.sizeDelta = new Vector2(-40, -180);

        // 참고: VerticalLayoutGroup은 수동으로 추가하세요
        // Editor에서 LayoutGroup 생성 시 에러 발생하므로 주석 처리
        // VerticalLayoutGroup 설정이 필요하면 프리팹에서 수동으로 추가

        // 7. Progress Display
        GameObject progressDisplay = CreateUIObject("ProgressDisplay", root.transform);
        RectTransform progressRect = progressDisplay.GetComponent<RectTransform>();
        progressRect.anchorMin = new Vector2(0, 0);
        progressRect.anchorMax = new Vector2(1, 0);
        progressRect.pivot = new Vector2(0.5f, 0);
        progressRect.sizeDelta = new Vector2(-40, 80);
        progressRect.anchoredPosition = new Vector2(0, 20);

        GameObject progressTextObj = CreateTextObject("ProgressText", progressDisplay.transform, 28, TextAlignmentOptions.Center);
        TMP_Text progressText = progressTextObj.GetComponent<TMP_Text>();
        progressText.text = $"0/{stepCount}";
        progressText.fontStyle = FontStyles.Bold;
        progressText.color = new Color(0.4f, 0.8f, 0.4f, 1f);
        RectTransform progressTextRect = progressTextObj.GetComponent<RectTransform>();
        progressTextRect.anchorMin = Vector2.zero;
        progressTextRect.anchorMax = Vector2.one;
        progressTextRect.offsetMin = Vector2.zero;
        progressTextRect.offsetMax = Vector2.zero;

        // 8. MRTK 컴포넌트 추가
        if (createMRTKComponents)
        {
            // ObjectManipulator - 기본 설정만
            ObjectManipulator manipulator = root.AddComponent<ObjectManipulator>();
            manipulator.AllowFarManipulation = true;

            Debug.Log("✅ MRTK 컴포넌트 추가 완료");
        }

        // 9. StepItem 프리팹이 없으면 자동 생성
        if (!AssetDatabase.LoadAssetAtPath("Assets/Prefabs/UI/RecipeStepItem.prefab", typeof(GameObject)))
        {
            Debug.Log("📝 RecipeStepItem 프리팹이 없어서 자동 생성합니다...");
            CreateRecipeStepItemPrefab();
        }

        // 10. RecipeStepPanel 스크립트 추가 및 참조 연결
        RecipeStepPanel panel = root.AddComponent<RecipeStepPanel>();
        
        // 참조 자동 연결
        panel.recipeNameText = root.transform.Find("HeaderSection/RecipeNameText").GetComponent<TMP_Text>();
        panel.ingredientsText = root.transform.Find("HeaderSection/IngredientsText").GetComponent<TMP_Text>();
        panel.stepsContainer = root.transform.Find("StepsContainer");
        panel.progressDisplay = root.transform.Find("ProgressDisplay");
        panel.canvasGroup = root.GetComponent<CanvasGroup>();
        
        // StepItem 프리팹 참조
        GameObject stepItemPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/UI/RecipeStepItem.prefab", typeof(GameObject)) as GameObject;
        if (stepItemPrefab != null)
            panel.stepItemPrefab = stepItemPrefab;
        
        // 11. DraggableStepPanel 스크립트 추가
        root.AddComponent<DraggableStepPanel>();

        // 12. Prefab 저장
        string prefabPath = "Assets/Prefabs/UI/RecipePanel.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, prefabPath);

        Debug.Log($"✅ RecipePanel 프리팹 생성 완료: {prefabPath}");

        // 13. 생성된 오브젝트 선택
        Selection.activeGameObject = root;
        EditorGUIUtility.PingObject(root);
        
        // Canvas 강제 업데이트
        Canvas.ForceUpdateCanvases();
    }

    private void CreateRecipeStepItemPrefab()
    {
        // 1. Root GameObject
        GameObject stepItem = new GameObject("RecipeStepItem");
        RectTransform rect = stepItem.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(700, 100);

        // 2. Background
        GameObject bg = CreateUIObject("Background", stepItem.transform);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.25f, 0.9f);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // 3. Checkbox (왼쪽)
        GameObject checkboxObj = CreateUIObject("CompletionCheckbox", stepItem.transform);
        Toggle checkbox = checkboxObj.AddComponent<Toggle>();
        RectTransform checkboxRect = checkboxObj.GetComponent<RectTransform>();
        checkboxRect.anchorMin = new Vector2(0, 0.5f);
        checkboxRect.anchorMax = new Vector2(0, 0.5f);
        checkboxRect.pivot = new Vector2(0, 0.5f);
        checkboxRect.sizeDelta = new Vector2(40, 40);
        checkboxRect.anchoredPosition = new Vector2(15, 0);

        // Checkbox Background
        GameObject checkBg = CreateUIObject("Background", checkboxObj.transform);
        Image checkBgImage = checkBg.AddComponent<Image>();
        checkBgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        RectTransform checkBgRect = checkBg.GetComponent<RectTransform>();
        checkBgRect.anchorMin = Vector2.zero;
        checkBgRect.anchorMax = Vector2.one;
        checkBgRect.offsetMin = Vector2.zero;
        checkBgRect.offsetMax = Vector2.zero;

        // Checkbox Checkmark
        GameObject checkmark = CreateUIObject("Checkmark", checkboxObj.transform);
        Image checkmarkImage = checkmark.AddComponent<Image>();
        checkmarkImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);
        RectTransform checkmarkRect = checkmark.GetComponent<RectTransform>();
        checkmarkRect.anchorMin = Vector2.zero;
        checkmarkRect.anchorMax = Vector2.one;
        checkmarkRect.offsetMin = new Vector2(5, 5);
        checkmarkRect.offsetMax = new Vector2(-5, -5);

        checkbox.targetGraphic = checkBgImage;
        checkbox.graphic = checkmarkImage;

        // 4. Step Number (왼쪽 상단)
        GameObject numberObj = CreateTextObject("StepNumberText", stepItem.transform, 24, TextAlignmentOptions.Center);
        TMP_Text numberText = numberObj.GetComponent<TMP_Text>();
        numberText.text = "1";
        numberText.fontStyle = FontStyles.Bold;
        numberText.color = new Color(0.4f, 0.8f, 1f, 1f);
        RectTransform numberRect = numberObj.GetComponent<RectTransform>();
        numberRect.anchorMin = new Vector2(0, 0.5f);
        numberRect.anchorMax = new Vector2(0, 0.5f);
        numberRect.pivot = new Vector2(0, 0.5f);
        numberRect.sizeDelta = new Vector2(50, 40);
        numberRect.anchoredPosition = new Vector2(65, 0);

        // 5. Description (중앙)
        GameObject descObj = CreateTextObject("DescriptionText", stepItem.transform, 20, TextAlignmentOptions.MidlineLeft);
        TMP_Text descText = descObj.GetComponent<TMP_Text>();
        descText.text = "단계 설명이 여기에 들어갑니다";
        descText.color = Color.white;
        RectTransform descRect = descObj.GetComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0, 0);
        descRect.anchorMax = new Vector2(1, 1);
        descRect.offsetMin = new Vector2(125, 10);
        descRect.offsetMax = new Vector2(-110, -10);

        // 6. Step Image (우측)
        GameObject imageObj = CreateUIObject("StepImage", stepItem.transform);
        Image stepImage = imageObj.AddComponent<Image>();
        stepImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        RectTransform imageRect = imageObj.GetComponent<RectTransform>();
        imageRect.anchorMin = new Vector2(1, 0.5f);
        imageRect.anchorMax = new Vector2(1, 0.5f);
        imageRect.pivot = new Vector2(1, 0.5f);
        imageRect.sizeDelta = new Vector2(90, 80);
        imageRect.anchoredPosition = new Vector2(-10, 0);

        // 7. CanvasGroup 추가
        CanvasGroup canvasGroup = stepItem.AddComponent<CanvasGroup>();

        // 8. LayoutElement 추가
        LayoutElement layoutElement = stepItem.AddComponent<LayoutElement>();
        layoutElement.minHeight = 100;
        layoutElement.preferredHeight = 100;

        // 9. RecipeStepItem 스크립트 추가
        stepItem.AddComponent<RecipeStepItem>();

        // 10. Prefab 저장
        string prefabPath = "Assets/Prefabs/UI/RecipeStepItem.prefab";
        PrefabUtility.SaveAsPrefabAsset(stepItem, prefabPath);

        Debug.Log($"✅ RecipeStepItem 프리팹 생성 완료: {prefabPath}");

        Selection.activeGameObject = stepItem;
        EditorGUIUtility.PingObject(stepItem);
    }

    private GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        return obj;
    }

    private GameObject CreateTextObject(string name, Transform parent, int fontSize, TextAlignmentOptions alignment)
    {
        GameObject textObj = CreateUIObject(name, parent);
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.enableWordWrapping = true;
        return textObj;
    }
}
