using UnityEngine;

public class DummyEnemy : MonoBehaviour, IDamageable2D
{
    public float health = 30f;

    public void TakeDamage(float amount, Vector2 hitPoint, Vector2 hitDir)
    {
        print("ouch");
        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage! Health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // You could play an animation, spawn particles, etc.
        Destroy(gameObject);
    }
}