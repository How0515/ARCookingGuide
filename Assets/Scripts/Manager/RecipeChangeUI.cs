using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;

/// <summary>
/// 레시피 변경 UI (MRTK 버튼 지원)
/// - Unity Button과 MRTK PressableButton 모두 지원
/// - RecipeInitializer.ChangeRecipe() 호출
/// </summary>
public class RecipeChangeUI : MonoBehaviour
{
    [SerializeField] private RecipeInitializer recipeInitializer;

    private void Start()
    {
        // RecipeInitializer를 찾지 못했으면 자동으로 찾기
        if (recipeInitializer == null)
        {
            recipeInitializer = GetComponent<RecipeInitializer>();
            
            if (recipeInitializer == null)
            {
                // 같은 GameObject에 없으면 부모 GameObject에서 찾기
                recipeInitializer = GetComponentInParent<RecipeInitializer>();
            }

            if (recipeInitializer == null)
            {
                Debug.LogError("❌ RecipeInitializer를 찾을 수 없습니다!");
                return;
            }
        }

        Debug.Log("✅ RecipeChangeUI 준비 완료. 키보드(1,2,3) 또는 public 메서드로 테스트하세요.");
    }

    /// <summary>
    /// 키보드 입력으로 테스트
    /// Play 모드에서 1, 2, 3 키를 누르면 레시피 변경
    /// </summary>
    private void Update()
    {
        // RecipeInitializer가 없으면 다시 찾기
        if (recipeInitializer == null)
        {
            recipeInitializer = GetComponent<RecipeInitializer>();
            if (recipeInitializer == null)
            {
                recipeInitializer = GetComponentInParent<RecipeInitializer>();
            }
            if (recipeInitializer == null)
            {
                Debug.LogWarning("⚠️ RecipeInitializer를 찾을 수 없습니다. Hierarchy 구조 확인하세요.");
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("🔑 1 키 눌림");
            ChangeToRecipe1();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("🔑 2 키 눌림");
            ChangeToRecipe2();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("🔑 3 키 눌림");
            ChangeToRecipe3();
        }
    }

    /// <summary>
    /// 직접 메서드 호출
    /// </summary>
    public void ChangeToRecipe1()
    {
        if (recipeInitializer == null)
        {
            Debug.LogError("❌ RecipeInitializer가 null입니다!");
            return;
        }
        Debug.Log("📝 계란볶음밥 선택");
        recipeInitializer.ChangeRecipe("계란볶음밥");
    }

    public void ChangeToRecipe2()
    {
        if (recipeInitializer == null)
        {
            Debug.LogError("❌ RecipeInitializer가 null입니다!");
            return;
        }
        Debug.Log("📝 김치찌개 선택");
        recipeInitializer.ChangeRecipe("김치찌개");
    }

    public void ChangeToRecipe3()
    {
        if (recipeInitializer == null)
        {
            Debug.LogError("❌ RecipeInitializer가 null입니다!");
            return;
        }
        Debug.Log("📝 불고기 선택");
        recipeInitializer.ChangeRecipe("불고기");
    }
}
