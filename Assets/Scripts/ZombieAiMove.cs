using UnityEngine;

public class ZombieAiMove : MonoBehaviour
{
     [Header("Targeting")]
    public Transform target;                  // leave empty to auto-find Player by tag
    public string playerTag = "Player";
    public float reacquireInterval = 1f;      // how often to retry finding the player

    [Header("Movement")]
    public float speed = 2f;                  // RoundSystem will multiply this per round
    public float stoppingDistance = 0.2f;     // how close before we stop
    public bool useRigidbody = true;          // toggle if you don't want physics

    [Header("Visual Facing (optional)")]
    public SpriteRenderer spriteRenderer;     // assign if you want flipY handling
    public bool spriteFacesRight = true;      // true if your sprite's "forward" is +X

    Rigidbody2D rb;
    float reacquireTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        // Auto-find player if none assigned
        if (target == null)
        {
            var playerGo = GameObject.FindGameObjectWithTag(playerTag);
            if (playerGo != null) target = playerGo.transform;
        }

        // If using Rigidbody2D, make sure rotation doesn't spin from collisions
        if (rb != null) rb.freezeRotation = true;
    }

    void Update()
    {
        // Reacquire target occasionally in case player was spawned later
        if (target == null)
        {
            reacquireTimer -= Time.deltaTime;
            if (reacquireTimer <= 0f)
            {
                reacquireTimer = reacquireInterval;
                var playerGo = GameObject.FindGameObjectWithTag(playerTag);
                if (playerGo) target = playerGo.transform;
            }
            return;
        }

        Vector2 toTarget = (target.position - transform.position);
        float dist = toTarget.magnitude;

        // Stop when close enough
        if (dist <= stoppingDistance)
        {
            if (useRigidbody && rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        // Desired direction and velocity
        Vector2 dir = toTarget / Mathf.Max(dist, 0.0001f);
        Vector2 vel = dir * speed;

        // Move
        if (useRigidbody && rb != null)
        {
            rb.linearVelocity = vel;
        }
        else
        {
            transform.position += (Vector3)(vel * Time.deltaTime);
        }

        // Face movement
        FaceDirection(dir);
    }

    void FaceDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.0001f) return;

        // Rotate to face the direction (assumes sprite faces +X in its art)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Optional: keep sprite upright by flipping on Y when aiming left
        if (spriteRenderer != null)
        {
            // if sprite art points right, flip when facing left; if it points up, adjust logic accordingly
            bool facingLeft = angle > 90f && angle < 270f;
            spriteRenderer.flipY = facingLeft && spriteFacesRight;
        }
    }
}
