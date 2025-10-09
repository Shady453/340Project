using UnityEngine;

public class Gun : MonoBehaviour
{
    
    [Header("Refs")]
    public Transform firePoint;            // where the shot spawns (tip of barrel)
    public GameObject projectilePrefab;    // prefab with SpriteRenderer + (optionally) Rigidbody2D

    [Header("Shot Settings")]
    public float projectileSpeed = 20f;
    public float lifetime = 3f;            // auto-despawn
    
    public void Shoot()
    {
        // Make the projectile at the barrel tip
        GameObject p = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // Compute world-space direction to the mouse
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        Vector2 dir = (mouseWorld - firePoint.position).normalized;

        // Face the projectile along the direction (x+ “right” points forward)
        p.transform.right = dir;

        // Try physics first; if no RB2D, fall back to manual movement
        if (p.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = dir * projectileSpeed;
        }

        // Clean up after lifetime
        Destroy(p, lifetime);
    }
}
