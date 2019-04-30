using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{

    public float moveSpeed = 6.0f;

    private Rigidbody2D rbody;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    Vector2 velocity;
    public Animator animator;

    public GameObject coin;
    private float currentReloadTime;
    public float reloadTime;

    public int coinCount = 0;
    private bool isDead;
    private float isDiedTimeout;

    [SerializeField] private Transform respawnPoint;

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isDiedTimeout = 2.0f;
        currentReloadTime = reloadTime;
    }

    Vector2 input = new Vector2(1.0f, 0.0f);

    void Update()
    {
        if (!isDead)
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

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");


            Vector2 input_int = new Vector2(horizontal, vertical).normalized;
            if (!(input_int.x == 0.0f && input_int.y == 0.0f))
            {
                input.Set(-input_int.x, input_int.y);
                //Debug.Log("update input_int :" + input_int.x + ", " + input_int.y);
            }
            //Debug.Log("input :" + input.x + ", " + input.y);
            float inputAngle = 0.0f;
            float angle = Vector2.SignedAngle(input, new Vector2(0.0f, -1.0f));


            if (coinCount > 0 && currentReloadTime <= 0 && Input.GetKeyDown(KeyCode.Space))
            {
                coinCount -= 1;
                Instantiate(coin, transform.position, Quaternion.Euler(0, 0, angle));
                currentReloadTime = reloadTime;
            }
            else if (currentReloadTime > 0)
            {
                currentReloadTime -= Time.deltaTime;
            }
        }
        else
        {
            if (isDiedTimeout <= 0)
            {
                SceneManager.LoadScene(2); // ToDieScreen
            }
            else
            {
                isDiedTimeout -= Time.deltaTime;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Reset();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            rbody.MovePosition(rbody.position + velocity * Time.fixedDeltaTime);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("ColliderTag2:" + collision.gameObject.tag);
        if (collision.gameObject.tag == "CoinGround") { 
            coinCount += 1;
            Destroy(collision.gameObject);
            Debug.Log("GET COINS coins:" + coinCount);
        }
        if (collision.gameObject.tag == "Grib")
        {
            CRTEffect crt = Camera.main.GetComponent<CRTEffect>();
            crt.onGlitches();
            Destroy(collision.gameObject);
            Debug.Log("onGlitches");
        }
    }

    public void onDamage()
    {
        coinCount -= 1;
        Debug.Log("DAMAGE coins:"+coinCount);
        if (coinCount<0)
        {
            Die();
        }
    }


    public void Die()
    {
        Debug.Log("DIE !!!!!!!!!!!!!!!!!");
        animator.SetBool("isDead", true);

        isDead = true;
    }

    public void Reset()
    {
        animator.SetBool("isDead", false);
        transform.position = respawnPoint.transform.position;
        isDead = false;
    }
}