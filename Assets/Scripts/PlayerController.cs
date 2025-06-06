using Assets.Scripts;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// THIS SCRIPT CAN BE SPLIT INTO TWO - 1) PLAYERCONTROLER AND 2)PLATYERMOVEMENT
/// </summary>
public class PlayerController : MonoBehaviour
{  

    public float jumpForce;
    public Sprite idleSprite;
    public Sprite fallSprite;
    public Sprite jumpSprite;
    public TextMeshProUGUI livesText;


    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private static int lives;
    private float moveInput;
    private bool isPlayerCollidingWithEnemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupPlayer();
        UpdateLivesText();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxis("Horizontal"); //1 or -1
        
        TogglePlayerHorizontalDir();
        UpdatePlayerSprites();

    }

    private void FixedUpdate()
    {
        float moveSpeed = 6f;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if(transform.position.y < ScreenBounds.LetfY && rb.linearVelocityY < 0) 
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            PlayerIsHurt();
        }
    }

    private void LateUpdate()
    {
        ClampPlayerX();
    }

    private void SetupPlayer()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = idleSprite;
        lives = 3;
        isPlayerCollidingWithEnemy = false;
    }

    private void UpdateLivesText()
    {
        livesText.text = $"Lives: {lives}";
    }
    


    private void UpdatePlayerSprites()
    {
        if (rb.linearVelocityY > 0.1f)// increading 
            sr.sprite = jumpSprite;
        else if (rb.linearVelocityY < -0.1f)//  descrinsing
            sr.sprite = fallSprite;
        else
            sr.sprite = idleSprite;
    }

    private void TogglePlayerHorizontalDir()
    {
        if (moveInput > 0) // 1 = towards right and -1 = towards left
            sr.flipX = false;
        else
            sr.flipX = true;
    }

    IEnumerator BlinkEffect()
    {
        if(isPlayerCollidingWithEnemy)
            gameObject.GetComponent<CircleCollider2D>().enabled = false; //I-FRAMES LOGUIC!

            for (int i = 0; i < 5; i++)
            {
                sr.enabled = false;
                yield return new WaitForSeconds(0.1f);

                sr.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }

         gameObject.GetComponent<CircleCollider2D>().enabled = true; ;
    }

   private void ClampPlayerX()
    {
        Vector2 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, ScreenBounds.LeftX, ScreenBounds.RightX);
        transform.position = pos;
    }

    private void PlayerIsHurt()
    {
        lives--;
        UpdateLivesText();
        if (lives <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(BlinkEffect());
        }
    }

    private void Die()
    {
        FindFirstObjectByType<ScoreManager>().SaveHighScore();
        FindFirstObjectByType<GameOverManager>().GameOver();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isPlayerCollidingWithEnemy = false;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            PlayerIsHurt();
            isPlayerCollidingWithEnemy = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            foreach(ContactPoint2D point in collision.contacts)
            {
                if(point.normal.y > 0.5f) //player coming from above
                {
                    StartCoroutine(DelayJump());
                }
            }
                
        }
    }
    
   private  IEnumerator DelayJump()
    {
        sr.sprite = jumpSprite;
        yield return new WaitForSeconds(0.1f);
        rb.linearVelocityY = 0; //reset to avoid players acceleration to oblivion
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

    }
}
