using UnityEngine;

public class Collision : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset; // 3 variaveis em 1 linha

    void Awake()
    {

    }

    void Update()
    {
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);
        
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;

        // var position = new Vector2[] { bottomOffset, rightOffset, leftOffset};

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);

    }
}
