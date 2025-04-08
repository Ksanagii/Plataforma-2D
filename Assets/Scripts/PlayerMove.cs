using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed;

    [Header("Jump Controller")]
    [SerializeField] float jumpForce;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;
    Collision coll;
    [SerializeField] float slideSpeed = 6;
    public bool wallGrab;
    public bool canMove;
    public bool wallSlide;
    public bool wallJumped;
    public bool isDashing;
    float wallJumpLerp = 10;

    public int side = 1;

    void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
    }

    void Start()
    {

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
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x,y);

        Walk(dir);

        
        // wallGrab = coll.onWall && Input.GetKey(KeyCode.LeftShift);

        #region Setting wallGrab and wallSlide
        // Se estiver colidindo com a parede e apertando botao de grab ele ativa o grab e para de deslizar na parede
        if(coll.onWall && Input.GetButton("Fire3") && canMove )
        {
            Debug.Log("Ativou wall grab");
            wallGrab = true;
            wallSlide = false;
        }

        // se soltar botao de grab ou parar de colidir na parede ele 
        if(Input.GetButtonUp("Fire3") || !coll.onWall || !canMove ) // 
        {
            Debug.Log("Desativou wall grab");
            wallGrab = false;
            wallSlide = false;
        }
        #endregion

        if (coll.onGround /* && !isDashing */ )
        {
            wallJumped = false;
            // GetComponent<BetterJumping>().enabled = true;
        }

        if(wallGrab /* && !isDashing */ )
        {
            rb.gravityScale = 0; // tira a gravidade se estou escalando a parede
            if(x > .2f || x < -.2f) // se estou apertando para alguma direcao, tudo abaixo disso sera executado

            rb.linearVelocity = new Vector2(rb.linearVelocityX, 0);

            float speedModifier = y > 0 ? .5f : 1; // speedModifier é 0.5 se y == 0, senão é 1. alterando a velocidade de queda e subida
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, y * (speed * speedModifier)); // escalando para cima para para baixo
        }
        else
        {
            rb.gravityScale = 3;
        }

        if(coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab) // se esta apertando a direção da parede e não escalando, aplica o slide
            {
                wallSlide = true;
                WallSlide();
            }

        }

        if (!coll.onWall || coll.onGround) // assegurando que o wallSlide vai desativar apos sair da parede ou tocar no chao
        {
            wallSlide = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (coll.onGround){
                Jump(Vector2.up, false);
            }
            if (coll.onWall && !coll.onGround){
                WallJump();
            }

        }

        // mexer com particulas, dash e outras coisas

        if (wallGrab || wallSlide || !canMove){
            return;
        }

        if(x > 0)
        {
            side = 1;
            // anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            // anim.Flip(side);
        }
    }

    void Walk(Vector2 dir)
    {
        if (!canMove){
            return;
        }
        if (wallGrab){
            return;
        }
        
        if (!wallJumped)
        {
            rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocityY);
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, (new Vector2(dir.x * speed, rb.linearVelocity.y)), wallJumpLerp * Time.deltaTime);
        }
        // rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocityY);
    }

    void Jump(Vector2 dir, bool wall)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocityX, 0);
        rb.linearVelocity += dir * jumpForce;
    }

    void WallSlide()
    {
        if (!canMove){
            return;
        }

        bool pushingWall = false;
        if((rb.linearVelocityX > 0 && coll.onRightWall) || (rb.linearVelocityX < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.linearVelocityX;

        rb.linearVelocity = new Vector2(push, -slideSpeed);
    }

    private void WallJump()
    {

        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            // anim.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump(Vector2.up / 1.5f + wallDir / 1.5f, true);

        wallJumped = true;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    /*
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
    */
}
