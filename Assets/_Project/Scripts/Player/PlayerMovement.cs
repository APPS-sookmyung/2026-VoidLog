using UnityEngine;

// 기본 상하좌우 이동. CanMove가 false면 입력을 무시하고 정지한다
// CanMove()를 통해 주인공 동작 불가능/가능 제어
public class PlayerMovement : MonoBehaviour
{
    private bool CanMove = true; // 이동 기본 설정 
    [SerializeField] private float moveSpeed = 3f; // 기본 스피드 값

    private Rigidbody2D rb;
    private Vector2 moveInput; // Update에서 받은 입력을 저장할 변수

    public bool getCanMove() { return CanMove; } // 현재 이동 가능 여부 확인
    public void setCanMove(bool state) { CanMove = state; } // 이동 가능 여부 변경

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!CanMove) // 이동 불가 상태면 백터 값을 0으로 설정
        {
            moveInput = Vector2.zero;
            return;
        }

        // Update에서는 빠른 키 입력 감지만 처리
        float h = Input.GetAxisRaw("Horizontal"); // x값확인 - A,D 움직일 때
        float v = Input.GetAxisRaw("Vertical");   // y값확인 - W,S 움직일 때
        
        moveInput = new Vector2(h, v).normalized; 
    }

    private void FixedUpdate()
    {
        if (!CanMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // FixedUpdate에서 물리 속도를 일정한 주기로 변경
        rb.linearVelocity = moveInput * moveSpeed; 
    }
}