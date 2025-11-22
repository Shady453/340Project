using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Contact Kill Settings")]
    public float timeToDieOnContact = 2f;   // seconds in contact before dying

    [Header("References")]
    public UI ui;
    public RoundSystem roundSystem;

    private float contactTimer = 0f;
    private int contactCount = 0;
    private bool isDead = false;

    private Move2D move;
    private Gun gun;
    private SpriteRenderer sr;

    void Awake()
    {
        move = GetComponent<Move2D>();
        gun  = GetComponent<Gun>();
        sr   = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead) return;

        // If at least one enemy is touching, count up
        if (contactCount > 0)
        {
            contactTimer += Time.deltaTime;
            if (contactTimer >= timeToDieOnContact)
            {
                Die();
            }
        }
        else
        {
            // No enemies touching, reset timer
            contactTimer = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            contactCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            contactCount--;
            if (contactCount <= 0)
            {
                contactCount = 0;
                contactTimer = 0f;
            }
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Stop rounds / enemy spawning
        if (roundSystem != null)
        {
            roundSystem.StopGame();
        }

        // Show Game Over UI
        if (ui != null)
        {
            ui.ShowGameoverScreen();
        }

        // Disable controls
        if (move != null) move.enabled = false;
        if (gun  != null) gun.enabled  = false;

        

        Debug.Log("Player died: stayed in contact with a zombie too long.");
    }
}

