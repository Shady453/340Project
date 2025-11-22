using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;

public class Barricade : MonoBehaviour
{
    // Global list of all barricades in the scene
    public static readonly List<Barricade> All = new List<Barricade>();

    public float maxHealth = 100f;
    public NavMeshSurface navSurface;   // drag your NavMesh object here in the prefab/scene

    float currentHealth;
    SpriteRenderer sr;
    Collider2D col;

    public bool IsBroken => currentHealth <= 0f;

    void OnEnable()
    {
        All.Add(this);
    }

    void OnDisable()
    {
        All.Remove(this);
    }

    void Awake()
    {
        currentHealth = maxHealth;
        sr  = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // Fallback: auto-find nav surface if not set in inspector
        if (navSurface == null)
        {
            navSurface = FindObjectOfType<NavMeshSurface>();
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsBroken) return;

        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Break();
        }
    }

    void Break()
    {
        currentHealth = 0f;

        // Turn off collision and visuals (or swap sprite here instead)
        if (col != null) col.enabled = false;
        if (sr  != null) sr.enabled = false;

        // Rebuild NavMesh so this opening becomes walkable
        if (navSurface != null)
        {
            navSurface.BuildNavMesh();
        }

       
    }
}