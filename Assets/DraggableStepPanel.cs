using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

/// <summary>
/// 단계 패널을 드래그 가능하게 만드는 컴포넌트
/// - ObjectManipulator를 통해 이미 제공되는 기능 확장
/// - 월드 스페이스에서 자유롭게 이동/회전 가능
/// </summary>
public class DraggableStepPanel : MonoBehaviour
{
    [Header("Drag Settings")]
    [SerializeField] private bool allowDrag = true;
    [SerializeField] private bool allowRotation = true;
    [SerializeField] private bool allowScaling = false; // 스케일은 기본 false

    private ObjectManipulator objectManipulator;
    private BoundsControl boundsControl;

    private void Start()
    {
        // ObjectManipulator 설정 (이미 Prefab에 있음)
        objectManipulator = GetComponent<ObjectManipulator>();
        if (objectManipulator != null)
        {
            // 이동 허용
            objectManipulator.AllowFarManipulation = true;
            
            // 2손가락 회전 활성화
            objectManipulator.TwoHandedManipulationType = 
                TransformFlags.Move | TransformFlags.Rotate;
        }

        // BoundsControl 설정 (이미 Prefab에 있음)
        boundsControl = GetComponent<BoundsControl>();
        if (boundsControl != null)
        {
            boundsControl.ActivationBehavior = 
                BoundsControlActivationType.ActivateByProximityAndPointer;
        }
    }

    /// <summary>
    /// 드래그 기능 활성화/비활성화
    /// </summary>
    public void SetDragEnabled(bool enabled)
    {
        allowDrag = enabled;
        if (objectManipulator != null)
            objectManipulator.enabled = enabled;
    }

    /// <summary>
    /// 회전 기능 활성화/비활성화
    /// </summary>
    public void SetRotationEnabled(bool enabled)
    {
        allowRotation = enabled;
    }

    /// <summary>
    /// 현재 위치에 고정
    /// </summary>
    public void LockPosition()
    {
        if (objectManipulator != null)
            objectManipulator.enabled = false;
    }

    /// <summary>
    /// 위치 고정 해제
    /// </summary>
    public void UnlockPosition()
    {
        if (objectManipulator != null)
            objectManipulator.enabled = true;
    }
}
