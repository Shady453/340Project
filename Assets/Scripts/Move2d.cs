
using UnityEngine;
using UnityEngine.InputSystem; // new Input System

public class Move2D : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float gamepadDeadzone = 0.2f;
    public bool preferPhysicsMovement = true;

    [Header("Boundary (pick one)")]
    [Tooltip("If set, the playable area will be taken from this sprite's world bounds.")]
    public SpriteRenderer boundarySprite;
    [Tooltip("If set, the playable area will be taken from this BoxCollider2D's world bounds.")]
    public BoxCollider2D boundaryCollider;
    [Tooltip("Extra inset applied to all sides of the boundary (use small positives if you see overlap).")]
    public Vector2 boundaryInset = new Vector2(0.0f, 0.0f);

    private Rigidbody2D rb2D;
    private Vector2 moveInput;
    private Bounds playableBounds;
    private bool hasBounds;
    private SpriteRenderer spriteRenderer;

    private Vector2 halfSize; // half extents of the player (so we keep the whole sprite inside)

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); 

        // Estimate player size from its Renderer or Collider so we keep the whole thing inside the bounds.
        // Prefer Collider2D if present (usually aligns with gameplay size).
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            halfSize = col.bounds.extents;
        }
        else
        {
            Renderer r = GetComponent<Renderer>();
            halfSize = r != null ? r.bounds.extents : new Vector2(0.5f, 0.5f); // fallback
        }

        RecomputePlayableBounds();
    }

    private void OnValidate()
    {
        // Keep inset non-negative
        if (boundaryInset.x < 0f) boundaryInset.x = 0f;
        if (boundaryInset.y < 0f) boundaryInset.y = 0f;
    }

    private void Update()
    {
        ReadInputNewInputSystem();

        if (moveInput.sqrMagnitude > 1f) moveInput.Normalize();

        // Flip sprite depending on horizontal direction
        if (spriteRenderer != null)
        {
            if (moveInput.x < 0f) spriteRenderer.flipX = true;
            else if (moveInput.x > 0f) spriteRenderer.flipX = false;
        }

        if (!UsePhysics())
        {
            Vector3 target = transform.position + (Vector3)(moveInput * speed * Time.deltaTime);
            if (hasBounds) target = ClampToPlayable(target);
            transform.position = target;
        }
    }

    private void FixedUpdate()
    {
        if (UsePhysics())
        {
            Vector2 target = (rb2D.position + moveInput * speed * Time.fixedDeltaTime);
            if (hasBounds) target = (Vector2)ClampToPlayable(target);
            rb2D.MovePosition(target);
        }
    }

    private bool UsePhysics() => preferPhysicsMovement && rb2D != null;

    private void ReadInputNewInputSystem()
    {
        float x = 0f, y = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) x -= 1f;
            if (Keyboard.current.dKey.isPressed) x += 1f;
            if (Keyboard.current.wKey.isPressed) y += 1f;
            if (Keyboard.current.sKey.isPressed) y -= 1f;
        }

        if (Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.leftStick.ReadValue();
            if (stick.magnitude >= gamepadDeadzone)
            {
                x = stick.x;
                y = stick.y;
            }

            Vector2 dpad = Gamepad.current.dpad.ReadValue();
            if (dpad.sqrMagnitude > 0f)
            {
                x = dpad.x;
                y = dpad.y;
            }
        }

        moveInput = new Vector2(x, y);
    }

    /// <summary>
    /// Recalculate the playable bounds from either boundarySprite or boundaryCollider.
    /// Call this if you resize/move the field at runtime.
    /// </summary>
    public void RecomputePlayableBounds()
    {
        hasBounds = false;

        if (boundarySprite != null)
        {
            playableBounds = boundarySprite.bounds;
            hasBounds = true;
        }
        else if (boundaryCollider != null)
        {
            playableBounds = boundaryCollider.bounds;
            hasBounds = true;
        }

        if (hasBounds)
        {
            // Apply inset (shrink evenly from all sides)
            playableBounds.Expand(new Vector3(-2f * boundaryInset.x, -2f * boundaryInset.y, 0f));
        }
    }

    /// <summary>
    /// Clamp a world position so the entire player stays inside the playable bounds.
    /// </summary>
    private Vector3 ClampToPlayable(Vector3 worldPos)
    {
        if (!hasBounds) return worldPos;

        // Compute min/max we allow the player center to reach, factoring in the player's half-size
        float minX = playableBounds.min.x + halfSize.x;
        float maxX = playableBounds.max.x - halfSize.x;
        float minY = playableBounds.min.y + halfSize.y;
        float maxY = playableBounds.max.y - halfSize.y;

        worldPos.x = Mathf.Clamp(worldPos.x, minX, maxX);
        worldPos.y = Mathf.Clamp(worldPos.y, minY, maxY);
        return worldPos;
    }
}