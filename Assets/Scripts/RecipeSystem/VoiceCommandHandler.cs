// using Microsoft.MixedReality.Toolkit.Input;
// using UnityEngine;

// public class VoiceCommandHandler : MonoBehaviour, IMixedRealitySpeechHandler
// {
//     private RecipeManager recipeManager;

//     void Start()
//     {
//         recipeManager = FindObjectOfType<RecipeManager>();
//     }

//     public void OnSpeechKeywordRecognized(SpeechEventData eventData)
//     {
//         string cmd = eventData.Command.Keyword.ToLower();

//         switch (cmd)
//         {
//             case "다음":
//             case "next":
//             case "tiếp theo":
//             case "tiếp":
//                 recipeManager.NextStep();
//                 break;

//             case "이전":
//             case "previous":
//             case "trước":
//             case "lùi":
//                 recipeManager.PreviousStep();
//                 break;

//             case "팁":
//             case "tip":
//             case "mẹo":
//             case "팁 보여줘":
//                 recipeManager.ShowRandomTip();
//                 break;
//         }
//     }
// }

// using UnityEngine;
// using Microsoft.MixedReality.Toolkit.Input;

// public class VoiceCommandHandler : MonoBehaviour, IMixedRealitySpeechHandler
// {
//     public RecipeManager recipeManager;

//     void Start()
//     {
//         recipeManager = GetComponent<RecipeManager>();
//         Microsoft.MixedReality.Toolkit.MixedRealityToolkit.Instance.GetService<IMixedRealityInputSystem>()?
//             .SpeechEventSource?.RegisterHandler<IMixedRealitySpeechHandler>(this);
//     }

//     public void OnSpeechKeywordRecognized(SpeechEventData eventData)
//     {
//         string cmd = eventData.Command.Keyword.ToLower();
//         if (cmd.Contains("다음") || cmd.Contains("next") || cmd.Contains("tiếp"))
//             recipeManager.NextStep();
//         else if (cmd.Contains("이전") || cmd.Contains("previous") || cmd.Contains("trước"))
//             recipeManager.PreviousStep();
//         else if (cmd.Contains("팁") || cmd.Contains("tip"))
//             recipeManager.ShowRandomTip();
//     }
// }


using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

public class VoiceCommandHandler : MonoBehaviour, IMixedRealitySpeechHandler
{
    private RecipeManager recipeManager;

    void Start()
    {
        recipeManager = GetComponent<RecipeManager>();
    }

    void OnEnable()
    {
        if (CoreServices.InputSystem != null)
        {
            CoreServices.InputSystem.RegisterHandler<IMixedRealitySpeechHandler>(this);
        }
    }

    void OnDisable()
    {
        if (CoreServices.InputSystem != null)
        {
            CoreServices.InputSystem.UnregisterHandler<IMixedRealitySpeechHandler>(this);
        }
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        if (recipeManager == null) return;

        string cmd = eventData.Command.Keyword.ToLowerInvariant();

        if (cmd.Contains("다음") || cmd.Contains("next") || cmd.Contains("tiếp"))
        {
            recipeManager.NextStep();
        }
        else if (cmd.Contains("이전") || cmd.Contains("previous") || cmd.Contains("trước"))
        {
            recipeManager.PreviousStep();
        }
        else if (cmd.Contains("팁") || cmd.Contains("tip") || cmd.Contains("mẹo"))
        {
            recipeManager.ShowRandomTip();
        }
    }
}