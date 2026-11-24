using UnityEngine;

//플레이어 미니맵 연동
public class PlayerMiniMap : MonoBehaviour
{
    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject worldMap;

    private bool wasPuzzleUIOpen = false;

    // 미니맵 차단 (예: Scene6 미로가 켜져 있는 동안)
    // MiniMapBlocker가 켜질 때 +1, 꺼질 때 -1. 하나라도 켜져 있으면 미니맵/월드맵을 숨긴다.
    private static int blockCount = 0;
    public static bool IsBlocked => blockCount > 0;
    private bool wasBlocked = false;

    public static void AddBlocker() => blockCount++;
    public static void RemoveBlocker() => blockCount = Mathf.Max(0, blockCount - 1);

    // 에디터에서 Domain Reload를 끈 경우에도 플레이마다 초기화
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetBlockCount() => blockCount = 0;

    void Start()
    {
        worldMap.SetActive(false);

        if (SaveManager.Instance != null && SaveManager.Instance.Data.hasOpenedMap)
        {
            miniMap.gameObject.SetActive(true);
        }
        else
        {
            miniMap.gameObject.SetActive(false);
        }
    }
    private void Update()
        {
            // 미니맵 차단 상태 변화 처리
            bool isBlocked = IsBlocked;
            if (isBlocked && !wasBlocked)
            {
                HideMiniMap();
                worldMap.SetActive(false);
            }
            if (!isBlocked && wasBlocked)
            {
                ShowMiniMap();
            }
            wasBlocked = isBlocked;

            // 차단 중에는 퍼즐 UI/M키 처리로 미니맵이 다시 켜지지 않도록 여기서 종료
            if (isBlocked)
            {
                return;
            }

            bool isPuzzleUIOpen = IsPuzzleUIOpen();

            // 퍼즐 UI가 방금 열림
            if (isPuzzleUIOpen && !wasPuzzleUIOpen)
            {
                HideMiniMap();
                worldMap.SetActive(false);
            }

            // 퍼즐 UI가 방금 닫힘
            if (!isPuzzleUIOpen && wasPuzzleUIOpen)
            {
                ShowMiniMap();
            }

            // 현재 상태 저장
            wasPuzzleUIOpen = isPuzzleUIOpen;


            // M키로 월드맵 열기
            if (Input.GetKeyDown(KeyCode.M))
            {
                // 아직 미니맵을 얻지 않았다면 차단
                if (SaveManager.Instance == null ||!SaveManager.Instance.Data.hasOpenedMap)
                {
                    return;
                }

                // 대사 중이면 차단
                if (DialogueManager.Instance != null &&DialogueManager.Instance.getIsDialogueRunning())
                {
                    return;
                }

                // 퍼즐 UI가 열려 있으면 차단
                if (isPuzzleUIOpen)
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
        if (IsBlocked) return; // 차단 중에는 월드맵도 열지 않음

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
        if (IsBlocked) return; // 미로 등으로 차단 중이면 켜지 않음

        if (SaveManager.Instance != null && SaveManager.Instance.Data.hasOpenedMap && !worldMap.gameObject.activeSelf)
        {
            miniMap.gameObject.SetActive(true);
        }
    }
}

