using UnityEngine;
using TMPro;

public class RecipeBoard : MonoBehaviour
{
    public TextMeshProUGUI recipeTitle;       // 레시피 제목
    public TextMeshProUGUI ingredientsText;   // 재료 목록
    public TextMeshProUGUI stepNumberText;    // 단계 번호
    public TextMeshProUGUI descriptionText;   // 단계 설명

    /// <summary>
    /// 레시피 정보 업데이트
    /// </summary>
    public void UpdateBoard(Recipe recipe, RecipeStep step)
    {
        // 레시피 제목
        if (recipeTitle != null)
        {
            recipeTitle.text = recipe.name;
        }

        // 재료 목록
        if (ingredientsText != null)
        {
            ingredientsText.text = recipe.ingredients;
        }

        // 현재 단계 정보
        if (stepNumberText != null)
        {
            stepNumberText.text = $"Step {step.stepNumber}";
        }

        // 단계 설명
        if (descriptionText != null)
        {
            descriptionText.text = step.description;
        }
    }

    /// <summary>
    /// 단계 설명만 업데이트 (빠른 갱신용)
    /// </summary>
    public void UpdateStepDescription(RecipeStep step)
    {
        if (stepNumberText != null)
        {
            stepNumberText.text = $"Step {step.stepNumber}";
        }

        if (descriptionText != null)
        {
            descriptionText.text = step.description;
        }
    }
}