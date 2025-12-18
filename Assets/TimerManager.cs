using UnityEngine;

// 전체 타이머를 관리하고 생성하는 매니저입니다.
public class TimerManager : MonoBehaviour
{
    public GameObject timerPrefab; // TimerEntity 프리팹 연결
    public Transform spawnPoint;   // 생성될 위치

    public void SpawnTimer(float seconds, string label)
    {
        
        // 위치를 약간씩 띄워서 겹치지 않게 생성
        Vector3 randomOffset = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
        
        // 프리팹 생성
        GameObject newTimer = Instantiate(timerPrefab, spawnPoint.position + randomOffset, spawnPoint.rotation);
        
        newTimer.transform.localPosition = Vector3.zero; 
        newTimer.transform.localRotation = Quaternion.identity;
        newTimer.transform.localScale = Vector3.one;
        // 생성된 타이머의 TimerObject 스크립트를 가져와서 시간 설정
        TimerObject timerScript = newTimer.GetComponent<TimerObject>();
        if (timerScript != null)
        {
            timerScript.Initialize(seconds);
        }
    }
    public void ResetTimer()
    {
        // Xóa tất cả timer con đang tồn tại dưới spawnPoint
        foreach (Transform child in spawnPoint)
        {
            Destroy(child.gameObject);
        }
    }

}