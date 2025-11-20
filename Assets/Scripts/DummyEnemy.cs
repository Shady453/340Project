using System;
using UnityEngine;
using UnityEngine.AI;

public class DummyEnemy : MonoBehaviour, IDamageable2D
{
    public static event Action<DummyEnemy> Died;
    public float health = 30f;
    
    void Awake()
    {
        var agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
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
        Died?.Invoke(this); 
        // You could play an animation, spawn particles, etc.
        Destroy(gameObject);
    }
}