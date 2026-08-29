using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    private bool CanMove = true; // 이동 기본 설정 
    [SerializeField] private float moveSpeed = 3f; // 기본 스피드 값

    private Rigidbody2D rb;
    private Vector2 moveInput; // Update에서 받은 입력을 저장할 변수

    public bool getCanMove() { return CanMove; } // 현재 이동 가능 여부 확인
    
    public void setCanMove(bool state) 
    { 
        CanMove = state; 
    
        // 이동 불가로 전환되는 즉시 입력값과 물리 속도를 0으로 강제 초기화
        if (!CanMove)
        {
            moveInput = Vector2.zero;
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.Sleep();
            }
        }
    }

    private void Awake() 
    {
        Instance = this;
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