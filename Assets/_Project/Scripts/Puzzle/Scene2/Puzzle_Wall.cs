using UnityEngine;

//퍼즐2 - 벽 퍼즐 (상호작용 물체 스크립트)
public class Puzzle_Wall : MonoBehaviour
{
    [SerializeField] Canvas wall; // 벽 UI
    private PlayerMovement playerMovement;

    void Awake()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    void Start()
    {
        wall.gameObject.SetActive(false);
    }

  
    public void WallOpen()
    {
        wall.gameObject.SetActive(true);
    }
   
}
