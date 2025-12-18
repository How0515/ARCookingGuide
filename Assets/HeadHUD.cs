using UnityEngine;
using TMPro;
using UnityEngine.UI; // 이미지(프로그레스바) 사용 시

public class HeadHUD : MonoBehaviour
{
    public TextMeshProUGUI stepIndexText; // "Step 1 / 5"
    public TextMeshProUGUI stepTitleText; // "재료 손질"
    public Image progressBar;             // Fill Amount로 조절할 바
    
    [Header("Follow Settings")]
    public Transform cameraTransform;     // 메인 카메라
    public Vector3 offset = new Vector3(0.5f, 0.3f, 1.0f); // 우측 상단 위치 오프셋
    public float smoothSpeed = 5f;

    void Update()
    {
        // 머리 따라다니게 하기 (Simple Smooth Follow)
        if (cameraTransform != null)
        {
            Vector3 targetPosition = cameraTransform.position + cameraTransform.TransformDirection(offset);
            Vector3 targetForward = cameraTransform.forward;
            
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetForward), Time.deltaTime * smoothSpeed);
        }
    }

    public void UpdateHUD(int current, int total, string title, float progressPercent)
    {
        stepIndexText.text = $"Step {current} / {total}";
        stepTitleText.text = title;
        if (progressBar != null) progressBar.fillAmount = progressPercent;
    }
}