using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;
    public int damage = 10;

    float timer;
    int direction = 1;

    bool broken = true;

    Animator animator;
    Rigidbody2D rigidbody2D;

    public ParticleSystem smokeEffect;

    AudioSource audioSource;
    public AudioClip fixedClip;
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!broken)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }
    void FixedUpdate()
    {
        if (!broken)
        {
            return;
        }

        Vector2 position = rigidbody2D.position;

        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        rigidbody2D.MovePosition(position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RubyController player = collision.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            Debug.Log("Damaged");
            player.updateHP(-damage);
        }
    }

    public void Fix()
    {
        broken = false;
        rigidbody2D.simulated = false;
        animator.SetTrigger("Fixed");
        smokeEffect.Stop();
        Destroy(smokeEffect.gameObject);
        audioSource.Stop();
        playSound(fixedClip);
    }

    public void playSound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
