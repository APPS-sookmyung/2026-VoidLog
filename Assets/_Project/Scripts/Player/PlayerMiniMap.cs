using UnityEngine;

//플레이어 미니맵 연동
public class PlayerMiniMap : MonoBehaviour
{
    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject worldMap;

    void Start()
    {
        worldMap.gameObject.SetActive(false);

        if (GameProgressData.hasOpenedMap)
        {
            miniMap.gameObject.SetActive(true);
        }
        else
        {
            miniMap.gameObject.SetActive(false);
        }
    }
    void Update() // 지도맵 키패드 열람 시 미니맵 활성화
    {
        if (!GameProgressData.hasOpenedMap) return;
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleWorldMap();
        }
        
    }
    public void ToggleWorldMap()
    {
        if (!worldMap.gameObject.activeSelf) // 월드맵 안켜진 상태면 키기
        {
            miniMap.gameObject.SetActive(false);
            worldMap.gameObject.SetActive(true);
        }
        else                                // 월드맵 켜진 상태면 월드맵 끄기
        {
            miniMap.gameObject.SetActive(true);
            worldMap.gameObject.SetActive(false);
        }
    }
    public void HideMiniMap() // 미니맵 숨기기
    {
        miniMap.gameObject.SetActive(false);
    }

    public void ShowMiniMap() // 미니맵 활성화
    {
        if (GameProgressData.hasOpenedMap && !worldMap.gameObject.activeSelf)
        {
            miniMap.gameObject.SetActive(true);
        }
    }
}

