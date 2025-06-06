using System.Collections;
using DG.Tweening;
using NUnit.Framework.Constraints;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Rigidbody2D rb;
    AnimationScript anim;
    [SerializeField] float speed;

    [Header("Jump Controller")]
    [SerializeField] float jumpForce;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;
    Collision coll;
    [SerializeField] float slideSpeed = 4;
    [SerializeField] float dashWaitingTime = .4f;
    public bool wallGrab;
    public bool canMove;
    public bool canJump;
    public bool wallSlide;
    public bool wallJumped;
    public bool isDashing;
    public float dashSpeed = 20;
    float wallJumpLerp = 10;
    public bool hasDashed;
    public int side = 1;
    public bool groundTouch;

    void Start()
    {
        canJump = true;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
        anim = GetComponent<AnimationScript>();
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
        anim.SetHorizontalMovement(x, y, rb.linearVelocityY);
        
        // wallGrab = coll.onWall && Input.GetKey(KeyCode.LeftShift);

        #region Setting wallGrab and wallSlide

        // Se estiver colidindo com a parede e apertando botao de grab ele ativa o grab e para de deslizar na parede
        if(coll.onWall && Input.GetButton("Fire3") && canMove )
        {
             if(side != coll.wallSide)
                anim.Flip(side*-1);
            wallGrab = true;
            wallSlide = false;
            
        }

        // se soltar botao de grab ou parar de colidir na parede ele 
        if(Input.GetButtonUp("Fire3") || !coll.onWall || !canMove ) // 
        {
            // Debug.Log("Desativou wall grab");
            wallGrab = false;
            wallSlide = false;
        }
        #endregion

        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            canJump = true;


            // GetComponent<BetterJumping>().enabled = true;
        
        }

        if(wallGrab && !isDashing)
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

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if(!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("jump");
            if (coll.onGround && canJump){
                Jump(Vector2.up, false);
            }
            if (coll.onWall && !coll.onGround){
                WallJump();
            }

        }

        if (Input.GetButtonDown("Fire1") && !hasDashed)
        {

            Debug.Log("DASH");
            if(xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        // mexer com particulas, dash e outras coisas

        if (wallGrab || wallSlide || !canMove)
        {
            return;
        }

        if(x > 0)
        {
            side = 1;
            anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            anim.Flip(side);
        }
    }

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;

        side = anim.spr.flipX ? -1 : 1;

        // jumpParticle.Play();
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
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, new Vector2(dir.x * speed, rb.linearVelocity.y), wallJumpLerp * Time.deltaTime);
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
        if (coll.wallSide != side){
            anim.Flip(side * -1);
        }
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
            anim.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.12f));

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

    private void Dash(float x, float y)
    {
        // Camera.main.transform.DOComplete();
        // Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        // FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

        hasDashed = true;

        anim.SetTrigger("dash");

        rb.linearVelocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        rb.linearVelocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        // FindObjectOfType<GhostTrail>().ShowGhost();
        StartCoroutine(GroundDash());
        DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

        // dashParticle.Play();
        // rb.linearVelocityY = 0f;
        rb.gravityScale = 0;
        canJump = false;
        wallJumped = true;
        isDashing = true;

        yield return new WaitForSeconds(dashWaitingTime);

        // dashParticle.Stop();
        rb.gravityScale = 3;
        canJump = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }

    void RigidbodyDrag(float x)
    {
        rb.linearDamping = x; // 
    }
}
