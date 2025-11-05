using System;
using UnityEngine;

public class DummyEnemy : MonoBehaviour, IDamageable2D
{
    public static event Action<DummyEnemy> Died;
    public float health = 30f;
    
    private ZombieAiMove mover;
    
    void Awake()
    {
        mover = GetComponent<ZombieAiMove>();
    }

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
        if (mover != null)
            mover.enabled = false;
        
        Died?.Invoke(this); 
        // You could play an animation, spawn particles, etc.
        Destroy(gameObject);
    }
}