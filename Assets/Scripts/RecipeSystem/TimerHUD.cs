// using UnityEngine;
// using TMPro;

// public class TimerHUD : MonoBehaviour
// {
//     public TextMeshProUGUI txtTimer;
//     private float timeRemaining;
//     private bool isRunning = false;

//     public void StartTimer(float seconds)
//     {
//         timeRemaining = seconds;
//         isRunning = true;
//     }

//     public void StopTimer()
//     {
//         isRunning = false;
//         txtTimer.text = "00:00";
//     }

//     void Update()
//     {
//         if (isRunning && timeRemaining > 0)
//         {
//             timeRemaining -= Time.deltaTime;
//             int m = Mathf.FloorToInt(timeRemaining / 60);
//             int s = Mathf.FloorToInt(timeRemaining % 60);
//             txtTimer.text = $"{m:00}:{s:00}";

//             if (timeRemaining <= 0)
//             {
//                 txtTimer.text = "시간 끝!";
//                 isRunning = false;
//             }
//         }
//     }
// }


using UnityEngine;
using TMPro;
using System.Collections;

public class TimerHUD : MonoBehaviour
{
    private TextMeshProUGUI txtTimer;
    private Coroutine timerCoroutine;

    void Awake()
    {
        txtTimer = GameObject.Find("txtTimer").GetComponent<TextMeshProUGUI>();
    }

    public void StartTimer(int seconds) // đổi int thành float
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(RunTimer(seconds));
    }

    IEnumerator RunTimer(int totalSeconds) //đổi int thành float
    {
        int remaining = totalSeconds; // đổi int thành float
        while (remaining > 0)
        {
            txtTimer.text = $"{remaining / 60:00}:{remaining % 60:00}";
            yield return new WaitForSeconds(1); // xoá chữ f sau số 1
            remaining--;
        }
        txtTimer.text = "00:00";
    }
}