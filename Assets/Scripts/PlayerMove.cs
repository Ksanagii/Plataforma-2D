using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed;

    [Header("Jump Controller")]
    [SerializeField] float jumpForce;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;
    
    bool canJump;


    void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        canJump = true;
    }

    void Update()
    {
        if(rb.linearVelocityY < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocityY > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 dir = new Vector2(x,y);
        
        Walk(dir);
        if(Input.GetButtonDown("Jump") && canJump)
        {
            Jump();
        }

    }

    void Walk(Vector2 dir)
    {
        rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocityY);
    }
    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocityX, 0);
        rb.linearVelocity += Vector2.up * jumpForce; // O up do Vector2 entrega um vector2 completo mas com 1 no Y, simbolizando cima no plano cartesiano
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.CompareTag("Floor"))
        {
            canJump = true;
            Debug.Log(canJump);
        }

    }

    void OnCollisionExit2D(Collision2D col) 
    {
        if(col.gameObject.CompareTag("Floor"))
        {
            canJump = false;
            Debug.Log(canJump);
        }
        
    }
}
