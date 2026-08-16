using UnityEngine;

//퍼즐2 - 벽 퍼즐 (상호작용 물체 스크립트)
public class Puzzle_Wall : MonoBehaviour
{
    [SerializeField] Canvas wall; // 벽 UI

    
    void Start()
    {
        wall.gameObject.SetActive(false);
    }

    void Update()
    {
        Close();
    }
    public void WallOpen()
    {
        wall.gameObject.SetActive(true);
    }
    public void Close() // ESC 창 닫기
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            wall.gameObject.SetActive(false);
        }
    }
}
