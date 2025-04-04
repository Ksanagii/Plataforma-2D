using UnityEngine;

public class Collision : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool onGround;
    [SerializeField] bool onWall;

    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset; // 3 variaveis em 1 linha


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);
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
