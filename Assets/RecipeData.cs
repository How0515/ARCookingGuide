// using UnityEngine;
// using System;

// [Serializable]
// public class RecipeStep
// {
//     public string stepTitle;       // 단계 제목 (예: 양파 썰기)
//     [TextArea] public string description;
//     public float timerSeconds;
    
//     public UnityEngine.Video.VideoClip stepVideo;
// }

// using UnityEngine;
// using System;

// [Serializable]
// public class RecipeStep
// {
//     public string tipId;        // tip_id từ CSV
//     public string recipe;       // recipe từ CSV
//     public string stepTitle;    // tip_name → stepTitle
//     [TextArea] public string description; // tip_description → description
//     public string gifFile;      // tên file GIF/ảnh trong Resources/RecipeDB/GIF
//     public float timerSeconds;  // nếu muốn thêm timer, default = 0
// }

// using UnityEngine;
// using System;
// using UnityEngine.Video; // ← thêm dòng này

// // [Serializable]
// [System.Serializable]
// public class RecipeStep
// {
//     public string tipId;
//     public string recipe;
//     public string stepTitle;
//     [TextArea] public string description;
//     public string gifFile;
//     public float timerSeconds;
//     public VideoClip stepVideo; // giờ sẽ nhận dạng được
// }

