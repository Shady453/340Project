using UnityEngine;

public class Bullet2d : MonoBehaviour
{
    
    public float damage = 25f;
    public LayerMask hitLayers;

    void OnTriggerEnter2D(Collider2D other)
    {
        print("pew");
        //if (((1 << other.gameObject.layer) & hitLayers) == 0) return;

        var dmg = other.GetComponent<IDamageable2D>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage, transform.position, transform.right);
            Destroy(gameObject);
        }

    }
    
    
}

