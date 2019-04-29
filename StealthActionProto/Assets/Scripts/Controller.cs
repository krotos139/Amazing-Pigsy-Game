using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{

    public float moveSpeed = 6.0f;

    private Rigidbody2D rbody;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    Vector2 velocity;
    public Animator animator;

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * moveSpeed;
        if (velocity != new Vector2(0, 0))
            animator.SetBool("isMoving", true);
        else
            animator.SetBool("isMoving", false);

        
        boxCollider.offset = spriteRenderer.sprite.bounds.center;
        boxCollider.size = spriteRenderer.sprite.bounds.size;

        /*if (Mathf.Abs(velocity.x) > 0.0f)
        {
            Debug.Log("X");
            boxCollider.offset = new Vector2(0.0039f, -0.0015f);
            boxCollider.size = new Vector2(0.2171f, -0.4086f);
        } else if (Mathf.Abs(velocity.y) > 0.0f)
        {
            Debug.Log("Y");
            boxCollider.offset = new Vector2(-0.0006f, -0.0015f);
            boxCollider.size = new Vector2(0.5222f, 0.3439f);
        }*/
    }

    void FixedUpdate()
    {
        rbody.MovePosition(rbody.position + velocity * Time.fixedDeltaTime);
    }
}