

// using UnityEngine;
// using Microsoft.MixedReality.Toolkit;
// using Microsoft.MixedReality.Toolkit.Input;

// public class VoiceCommandHandler : MonoBehaviour, IMixedRealitySpeechHandler
// {
//     private RecipeManager recipeManager;

//     void Start()
//     {
//         recipeManager = GetComponent<RecipeManager>();
//     }

//     void OnEnable()
//     {
//         if (CoreServices.InputSystem != null)
//         {
//             CoreServices.InputSystem.RegisterHandler<IMixedRealitySpeechHandler>(this);
//         }
//     }

//     void OnDisable()
//     {
//         if (CoreServices.InputSystem != null)
//         {
//             CoreServices.InputSystem.UnregisterHandler<IMixedRealitySpeechHandler>(this);
//         }
//     }

//     public void OnSpeechKeywordRecognized(SpeechEventData eventData)
//     {
//         if (recipeManager == null) return;

//         string cmd = eventData.Command.Keyword.ToLowerInvariant();

//         if (cmd.Contains("다음") || cmd.Contains("next") || cmd.Contains("tiếp"))
//         {
//             recipeManager.NextStep();
//         }
//         else if (cmd.Contains("이전") || cmd.Contains("previous") || cmd.Contains("trước"))
//         {
//             recipeManager.PreviousStep();
//         }
//         else if (cmd.Contains("팁") || cmd.Contains("tip") || cmd.Contains("mẹo"))
//         {
//             recipeManager.ShowRandomTip();
//         }
//     }
// }