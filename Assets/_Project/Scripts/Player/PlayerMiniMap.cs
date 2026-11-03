using UnityEngine;

//플레이어 미니맵 연동
public class PlayerMiniMap : MonoBehaviour
{
    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject worldMap;

    void Start()
    {
        worldMap.gameObject.SetActive(false);

        if (SaveManager.Instance.Data.hasOpenedMap)
        {
            miniMap.gameObject.SetActive(true);
        }
        else
        {
            miniMap.gameObject.SetActive(false);
        }
    }
    private void Update() // 지도맵 키패드 열람 시 미니맵 활성화
    {
        if (Input.GetKeyDown(KeyCode.M))
        {

            // 아직 미니맵을 얻지 않았다면 M키 차단
            if (SaveManager.Instance == null ||!SaveManager.Instance.Data.hasOpenedMap)
            {
                return;
            }
            // 대사 중이면 차단
            if (DialogueManager.Instance != null &&
                DialogueManager.Instance.getIsDialogueRunning())
            {
                return;
            }

            // 활성화된 퍼즐 UI가 하나라도 있으면 차단
            if (IsPuzzleUIOpen())
            {
                return;
            }

            ToggleWorldMap();
        }
    }

    private bool IsPuzzleUIOpen()
    {
        GameObject[] puzzleUIs = GameObject.FindGameObjectsWithTag("PuzzleUI");

        return puzzleUIs.Length > 0;
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
        if (SaveManager.Instance.Data.hasOpenedMap && !worldMap.gameObject.activeSelf)
        {
            miniMap.gameObject.SetActive(true);
        }
    }
}

