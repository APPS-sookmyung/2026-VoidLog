using UnityEngine;
using UnityEngine.Events;
using Unity.Cinemachine;

public class HangerFlowManager : MonoBehaviour
{
    [Header("켜고 끌 루트")]
    [SerializeField] private GameObject hangerRoot;
    [SerializeField] private GameObject mazeRoot;
    [SerializeField] private GameObject mazeCanvas;   // Maze_canvas

    [Header("플레이어 이동")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform mazeStartPoint;

    [Header("플레이어 속도")]
    [SerializeField] private float hangerSpeed = 6f;
    [SerializeField] private float mazeSpeed = 30f;

    [Header("카메라")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private GameObject followCam;    // Player Follow Camera
    [SerializeField] private Vector3 mazeCamPosition = new Vector3(68.25f, -41.25f, -10f);
    [SerializeField] private float mazeCamSize = 45f;

    [Header("미로 시작 후 실행할 것 (선택)")]
    [SerializeField] private UnityEvent onMazeStarted;

    private PlayerMovement playerMovement;
    private bool started = false;

    void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();

        // 격납고에서는 느린 속도로 시작
        if (playerMovement != null) playerMovement.setSpeed(hangerSpeed);
    }

    // DoorLock의 On Interact에서 호출
    public void StartMaze()
    {
        if (started) return;
        started = true;

        hangerRoot.SetActive(false);
        mazeRoot.SetActive(true);
        if (mazeCanvas != null) mazeCanvas.SetActive(true);

        player.position = mazeStartPoint.position;
        Physics2D.SyncTransforms();

        // 미로에서는 빠른 속도로 변경
        if (playerMovement != null) playerMovement.setSpeed(mazeSpeed);

        // 팔로우 카메라와 Brain을 꺼서 Main Camera를 풀어준 뒤, 미로용 값으로 고정
        if (followCam != null) followCam.SetActive(false);
        var brain = mainCam.GetComponent<CinemachineBrain>();
        if (brain != null) brain.enabled = false;

        mainCam.transform.position = mazeCamPosition;
        mainCam.orthographicSize = mazeCamSize;

        onMazeStarted?.Invoke();
    }
}