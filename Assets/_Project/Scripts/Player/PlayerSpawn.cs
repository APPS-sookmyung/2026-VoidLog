using UnityEngine;

// 플레이어 붙이는 컴포넌트 - 플레이어 스폰 위치 변경용
public class PlayerSpawn : MonoBehaviour
{
    void Start()
    {

    GameObject spawnPoint = GameObject.Find(SceneTransitionData.spawnPointName);

    if (spawnPoint != null)
    {
        transform.position = spawnPoint.transform.position;
    }
}
}