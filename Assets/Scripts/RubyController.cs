using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public float movement = 3.0f;
    Rigidbody2D rb;
    float horizontal;
    float vertical;
    public int maxHP = 100;
    [SerializeField] int HP;
    public int currentHP { get => HP; } //just a function to return HP

    bool isInvincible;
    float invincibleTimer;
    public float timeInvincible = 2.0f;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    public GameObject projectilePrefab;

    AudioSource audioSource;

    public AudioClip projectileClip;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
            {
                isInvincible = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Pressed X");
            RaycastHit2D hit = Physics2D.Raycast(rb.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }

    }

    private void FixedUpdate()
    {
        Vector2 positionToMove = new Vector2(horizontal, vertical) * movement * Time.fixedDeltaTime;
        Vector2 newPos = (Vector2)transform.position + positionToMove;

        rb.MovePosition(newPos);
    }

    public void updateHP(int value) //call to increase or decrease Ruby HP
    {
        if (value < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            invincibleTimer = timeInvincible;
        }
        HP += value;
        HP = Mathf.Clamp(HP, 0, maxHP); //restrict HP to be between 0 and 100
        UIHealthBar.instance.SetValue(currentHP/(float)maxHP);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rb.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);
        animator.SetTrigger("Launch");
        playSound(projectileClip);
    }

    public void playSound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}