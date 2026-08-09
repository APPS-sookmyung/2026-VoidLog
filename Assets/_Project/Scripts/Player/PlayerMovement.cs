using UnityEngine;

//기본 상하좌우 이동. CanMove가 false면 입력을 무시하고 정지한다
//CanMove()를 통해 주인공 동작 불가능/가능 제어
public class PlayerMovement : MonoBehaviour
{
    public bool CanMove = true;
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    public bool getCanMove(){return CanMove;}
    public void setCanMove(bool state) {CanMove = state;}


    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!CanMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
 
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(h, v).normalized;
 
        rb.linearVelocity = dir * moveSpeed;
    }
}